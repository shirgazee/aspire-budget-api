using AspireBudgetApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
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
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        private static SheetsService GetSheetsService(string credentialsJson)
        {
            var serviceInitializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromJson(credentialsJson).CreateScoped(Scopes)
            };
            return new SheetsService(serviceInitializer);
        }

        private readonly SheetsService _sheetsService;
        private readonly string _spreadSheetId;
        private readonly ILogger _logger;

        public AspireBudgetApi(string credentialsJson, string spreadSheetId, ILogger logger)
        {
            _sheetsService = GetSheetsService(credentialsJson);
            _spreadSheetId = spreadSheetId;
            _logger = logger ?? NullLogger.Instance;
        }

        public AspireBudgetApi(string credentialsJson, string spreadSheetId) : this(credentialsJson, spreadSheetId, null)
        {
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await GetValuesFromSingleColumn(Options.CategoriesRange);
        }

        public async Task<List<string>> GetAccountsAsync()
        {
            return await GetValuesFromSingleColumn(Options.AccountsRange);
        }

        private async Task<List<string>> GetValuesFromSingleColumn(string range)
        {
            var response = await _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, range).ExecuteAsync();
            var values = response.Values;
            if (values == null || values.Count == 0)
            {
                return new List<string>();
            }

            var result = new List<string>();
            foreach (var row in values)
            {
                result.Add(string.Join("", row.Select(r => r.ToString())));
            }
            return result;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int lastCount = 0)
        {
            var list = await GetTransactionsAndAccountTransfersAsync();

            var result = list.Where(x => x.Category != Options.AccountTransferCategory).ToList();

            // not optimal because we cannot be sure that transactions are in correct date order in google sheet
            if (lastCount > 0 && lastCount < result.Count)
            {
                result = result.OrderByDescending(x => x.Date).Take(lastCount).OrderBy(x => x.Date).ToList();
            }

            return result;
        }

        public async Task<List<AccountTransfer>> GetAccountTranfersAsync(int lastCount = 0)
        {
            var list = await GetTransactionsAndAccountTransfersAsync();

            var transactions = list.Where(x => x.Category == Options.AccountTransferCategory).ToList();
            var result = ParseAccountTransfersFromTransactions(transactions);

            // not optimal because we cannot be sure that transactions are in correct date order in google sheet
            if (lastCount > 0 && lastCount < result.Count)
            {
                result = result.OrderByDescending(x => x.Date).Take(lastCount).OrderBy(x => x.Date).ToList();
            }

            return result;
        }

        private async Task<List<Transaction>> GetTransactionsAndAccountTransfersAsync()
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, Options.TransactionsRange);
            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            var response = await request.ExecuteAsync();
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
                    _logger.LogError(ex, "Transaction instancing error");
                }
            }

            return list;
        }

        private List<AccountTransfer> ParseAccountTransfersFromTransactions(List<Transaction> transactions)
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
                    _logger.LogError($"Did not find matching account tranfer at {t1.Date.ToShortDateString()}");
                    continue;
                }

                result.Add(AccountTransfer.FromTransactions(t1, t2));

                transactions.Remove(t1);
                transactions.Remove(t2);
            }

            if(transactions.Count != 0)
            {
                _logger.LogError("Did not find matching account tranfers, please recheck your google sheet.");
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
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, Options.CategoryTransfersRange);
            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            var response = await request.ExecuteAsync();
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
                    _logger.LogError(ex, "CategoryTransfer instancing error");
                }
            }
            if (month > 0 && month < 13)
            {
                result = result.Where(x => x.Date.Month == month).ToList();
            }
            return result;
        }
        public async Task<bool> SaveCategoryTranferAsync(CategoryTransfer transfer)
        {
            IList<object> row = null;
            try
            {
                row = CategoryTransfer.ToGoogleRow(transfer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google row instancing error");
                return false;
            }

            var valueRange = new ValueRange { Values = new List<IList<object>> { row } };

            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadSheetId, Options.CategoryTransfersRange);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var response = await appendRequest.ExecuteAsync();
            return response.Updates.UpdatedRows == 1;
        }

        public async Task<bool> SaveTransactionAsync(Transaction transaction)
        {
            IList<object> row = null;
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

            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadSheetId, Options.TransactionsRange);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var response = await appendRequest.ExecuteAsync();
            return response.Updates.UpdatedRows == 1;
        }

        public void Dispose()
        {
            _sheetsService.Dispose();
        }
    }
}
