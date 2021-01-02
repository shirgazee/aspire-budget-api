using AspireBudgetApi.Models;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspireBudgetApi
{
    public class AspireBudgetApi : IDisposable
    {
        private readonly SheetsServiceWrapper _sheetsService;
        private readonly INamedRangeService _namedRangeService;
        private readonly ILogger _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credentialsJson"></param>
        /// <param name="spreadSheetId"></param>
        /// <param name="namedRangeService"></param>
        /// <param name="logger"></param>
        public AspireBudgetApi(string credentialsJson, 
            string spreadSheetId, 
            INamedRangeService namedRangeService, 
            ILogger logger = null)
        {
            _sheetsService = new SheetsServiceWrapper(credentialsJson, spreadSheetId);
            _namedRangeService = namedRangeService;
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Get budget categories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var categoriesA1Range = _namedRangeService.GetA1Range(TableRangeOptions.ConfigurationData);
            var reportableCategorySymbolA1Range = _namedRangeService.GetA1Range(ValueRangeOptions.ReportableCategorySymbol);
            var debtAccountSymbolA1Range = _namedRangeService.GetA1Range(ValueRangeOptions.ReportableCategorySymbol);

            var categoriesResponse = await _sheetsService.GetValues(categoriesA1Range);
            var reportableCategoryResponse = await _sheetsService.GetValues(reportableCategorySymbolA1Range);
            var debtAccountSymbolResponse = await _sheetsService.GetValues(debtAccountSymbolA1Range);
            
            var table = categoriesResponse.Values;
            if (table == null || table.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            var reportableCategorySymbol = (string)reportableCategoryResponse.Values.First().First();
            var debtAccountSymbol= (string)debtAccountSymbolResponse.Values.First().First();

            var categories =
                from row in table
                let rowSymbol = row[0].ToString()
                where rowSymbol == reportableCategorySymbol || rowSymbol == debtAccountSymbol
                select row[1].ToString();

            return categories;
        }

        /// <summary>
        /// Get budget accounts
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAccountsAsync()
        {
            var accountsA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.ConfigurationAccounts);
            return await GetValuesFromSingleColumn(accountsA1Range);
        }

        private async Task<IEnumerable<string>> GetValuesFromSingleColumn(string range)
        {
            var response = await _sheetsService.GetValues(range);
            var values = response.Values;
            if (values == null || values.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            return values.Select(row => string.Join("", row.Select(r => r.ToString())));
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int lastCount = 0)
        {
            var transactions = await GetTransactionsAndAccountTransfersAsync();

            var accountTransferCategoryA1Range = _namedRangeService.GetA1Range(ValueRangeOptions.AccountTransfer);
            var accountTransferCategoryResponse = await _sheetsService.GetValues(accountTransferCategoryA1Range);
            var accountTransferCategory = (string)accountTransferCategoryResponse.Values.First().First();
            
            var result = transactions.Where(x => x.Category != accountTransferCategory).ToArray();

            // not optimal because we cannot be sure that transactions are in correct date order in google sheet
            if (lastCount > 0 && lastCount < result.Length)
            {
                result = result.OrderByDescending(x => x.Date).Take(lastCount).OrderBy(x => x.Date).ToArray();
            }

            return result;
        }

        public async Task<List<AccountTransfer>> GetAccountTransfersAsync(int lastCount = 0)
        {
            var transactionsAndTransfers = await GetTransactionsAndAccountTransfersAsync();
            
            var accountTransferCategoryA1Range = _namedRangeService.GetA1Range(ValueRangeOptions.AccountTransfer);
            var accountTransferCategoryResponse = await _sheetsService.GetValues(accountTransferCategoryA1Range);
            var accountTransferCategory = (string)accountTransferCategoryResponse.Values.First().First();

            var transactions = transactionsAndTransfers.Where(x => x.Category == accountTransferCategory).ToList();
            var result = GetAccountTransfersFromTransactions(transactions);

            // not optimal because we cannot be sure that transactions are in correct date order in google sheet
            if (lastCount > 0 && lastCount < result.Count)
            {
                result = result.OrderByDescending(x => x.Date).Take(lastCount).OrderBy(x => x.Date).ToList();
            }

            return result;
        }

        private async Task<List<Transaction>> GetTransactionsAndAccountTransfersAsync()
        {
            var transactionsDatesA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.TransactionsDates);
            var transactionsOutflowsA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.TransactionsOutflows);
            var transactionsInflowsA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.TransactionsInflows);
            var transactionsCategoriesA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.TransactionsCategories);
            var transactionsAccountsA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.TransactionsAccounts);
            var transactionsStatusesA1Range = _namedRangeService.GetA1Range(ColumnRangeOptions.TransactionsStatuses);
            
            var transactionsRangeResponse = await _sheetsService.GetValues(accountTransferCategoryA1Range);
            var transactionsRange = (string)accountTransferCategoryResponse.Values.First().First();
            
            var response = await _sheetsService.GetValues(Options.TransactionsRange);
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                return new List<Transaction>();
            }

            var list = new List<Transaction>();
            foreach (var row in values)
            {
                try
                {
                    var transaction = Transaction.FromGoogleRow(row);
                    list.Add(transaction);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(Transaction)} instancing error");
                }
            }

            return list;
        }

        private List<AccountTransfer> GetAccountTransfersFromTransactions(List<Transaction> transactions)
        {
            var result = new List<AccountTransfer>();
            while (transactions.Count > 1)
            {
                var t1 = transactions.First();
                var t2 = transactions.FirstOrDefault(x =>
                    x.Date == t1.Date && Math.Abs(x.Inflow - t1.Outflow) < 0.01 &&
                    Math.Abs(x.Outflow - t1.Inflow) < 0.01);

                if (t2 == null)
                {
                    _logger.LogError($"Did not find matching account transfer at {t1.Date.ToShortDateString()}");
                    continue;
                }

                result.Add(AccountTransfer.FromTransactions(t1, t2));

                transactions.Remove(t1);
                transactions.Remove(t2);
            }

            if(transactions.Count != 0)
            {
                _logger.LogError("Did not find matching account transfers, please check your google sheet.");
            }

            return result;
        }

        public async Task<bool> SaveAccountTransferAsync(AccountTransfer transfer)
        {
            var result = true;
            var transactions = AccountTransfer.ToTransactions(transfer);
            foreach (var transaction in transactions)
            {
                result = result && await SaveTransactionAsync(transaction);
            }

            return result;
        }

        public async Task<List<CategoryTransfer>> GetCategoryTransfersAsync(int month = 0)
        {
            var response = await _sheetsService.GetValues(Options.CategoryTransfersRange);
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                return new List<CategoryTransfer>();
            }

            var result = new List<CategoryTransfer>();
            foreach (var row in values)
            {
                try
                {
                    var transfer = CategoryTransfer.FromGoogleRow(row);
                    result.Add(transfer);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(CategoryTransfer)} instancing error");
                }
            }
            if (month > 0 && month < 13)
            {
                result = result.Where(x => x.Date.Month == month).ToList();
            }
            return result;
        }

        public async Task<bool> SaveCategoryTransferAsync(CategoryTransfer transfer)
        {
            IList<object> row;
            try
            {
                row = CategoryTransfer.ToGoogleRow(transfer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting data to google sheets format");
                return false;
            }

            var valueRange = new ValueRange { Values = new List<IList<object>> { row } };
            
            var response = await _sheetsService.AppendValues(valueRange, Options.CategoryTransfersRange);
            return response == 1;
        }

        public async Task<bool> SaveTransactionAsync(Transaction transaction)
        {
            IList<object> row;
            try
            {
                row = Transaction.ToGoogleRow(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google row instancing error");
                return false;
            }

            var valueRange = new ValueRange { Values = new List<IList<object>> { row } };
            
            var response = await _sheetsService.AppendValues(valueRange, Options.TransactionsRange);
            return response == 1;
        }

        public async Task<List<DashboardRow>> GetDashboardAsync()
        {
            var a1Range = _namedRangeService.GetA1Range(TableRangeOptions.DashboardData);
            var response = await _sheetsService.GetValues(a1Range);
            var values = response.Values;

            var result = new List<DashboardRow>();
            if (values == null || values.Count == 0)
            {
                return result;
            }

            foreach (var row in values)
            {
                try
                {
                    var dashboardRow = DashboardRow.FromGoogleRow(row);
                    result.Add(dashboardRow);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "DashboardRow instancing error");
                }
            }

            return result;
        }

        public void Dispose()
        {
            _sheetsService.Dispose();
        }
    }
}
