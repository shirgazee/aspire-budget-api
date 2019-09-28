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

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await GetValuesFromSingleColumn(Options.CategoriesRange);
        }

        public async Task<IEnumerable<string>> GetAccountsAsync()
        {
            return await GetValuesFromSingleColumn(Options.AccountsRange);
        }

        private async Task<IEnumerable<string>> GetValuesFromSingleColumn(string range)
        {
            var response = await _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, range).ExecuteAsync();
            var values = response.Values;
            if (values == null || values.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            var result = new List<string>();
            foreach (var row in values)
            {
                result.Add(string.Join("", row.Select(r => r.ToString())));
            }
            return result;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int lastCount = 0)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, Options.TransactionsRange);
            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            var response = await request.ExecuteAsync();
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                return Enumerable.Empty<Transaction>();
            }
            if (lastCount > 0 && lastCount < values.Count)
            {
                values = values.Skip(values.Count - lastCount).ToList();
            }

            var result = new List<Transaction>();
            foreach (var row in values)
            {
                try
                {
                    var transaction = Transaction.FromGoogleRow(row);
                    result.Add(transaction);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction instancing error");
                }
            }
            return result;
        }

        public async Task<IEnumerable<CategoryTransfer>> GetCategoryTransfersAsync(int month = 0)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, Options.CategoryTransfersRange);
            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            var response = await request.ExecuteAsync();
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                return Enumerable.Empty<CategoryTransfer>();
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
            var valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { row };

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
