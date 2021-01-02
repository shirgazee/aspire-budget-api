using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspireBudgetApi.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace AspireBudgetApi
{
    /// <summary>
    /// Google Sheets wrapper with predefined settings
    /// </summary>
    public class SheetsServiceWrapper : IDisposable
    {
        private static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};

        private readonly SheetsService _sheetsService;
        private readonly string _spreadSheetId;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credentialsJson"></param>
        /// <param name="spreadSheetId"></param>
        public SheetsServiceWrapper(string credentialsJson, string spreadSheetId)
        {
            var serviceInitializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromJson(credentialsJson).CreateScoped(Scopes)
            };
            _sheetsService = new SheetsService(serviceInitializer);

            _spreadSheetId = spreadSheetId;
        }

        /// <summary>
        /// Get values within range in A1 notation
        /// </summary>
        /// <param name="a1Range"></param>
        /// <returns></returns>
        public async Task<ValueRange> GetValues(string a1Range)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadSheetId, a1Range);
            request.ValueRenderOption =
                SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            var response = await request.ExecuteAsync();
            return response;
        }

        /// <summary>
        /// Append values to range
        /// </summary>
        /// <param name="body"></param>
        /// <param name="a1Range"></param>
        /// <returns></returns>
        public async Task<int?> AppendValues(ValueRange body, string a1Range)
        {
            var appendRequest =
                _sheetsService.Spreadsheets.Values.Append(body, _spreadSheetId, a1Range);
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var response = await appendRequest.ExecuteAsync();
            return response.Updates.UpdatedRows;
        }

        /// <summary>
        /// Get A1 notation for named ranges
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetNamedRangesInA1Notation(string[] ranges)
        {
            var request = _sheetsService.Spreadsheets.Get(_spreadSheetId);
            request.Ranges = ranges;
            request.Fields = "namedRanges";
            request.IncludeGridData = false;

            var response = await request.ExecuteAsync();

            var sheetsIdsByTitles =
                response.Sheets.ToDictionary(sheet => sheet.Properties.SheetId ?? 0, sheet => sheet.Properties.Title);

            var namedRangesToA1Notation = new Dictionary<string, string>();
            foreach (var namedRange in response.NamedRanges)
            {
                var a1RangeNotation = SheetsHelpers.GetA1RangeNotation(sheetsIdsByTitles[namedRange.Range.SheetId ?? 0],
                    namedRange.Range);
                namedRangesToA1Notation.Add(namedRange.Name, a1RangeNotation);
            }

            return namedRangesToA1Notation;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _sheetsService?.Dispose();
        }
    }
}