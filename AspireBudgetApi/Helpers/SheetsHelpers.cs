using System;
using System.Text;
using Google.Apis.Sheets.v4.Data;

namespace AspireBudgetApi.Helpers
{
    public static class SheetsHelpers
    {
        /// <summary>
        /// Get A1 Notation for a range
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static string GetA1RangeNotation(string sheetName, GridRange range)
        {
            var startColumnLetterIndex = (int)range.StartColumnIndex! + 1;
            var startRowIndex = range.StartRowIndex! + 1;

            return $"'{sheetName}'!{GetColumnTitle(startColumnLetterIndex)}{startRowIndex}:{GetColumnTitle((int)range.EndColumnIndex!)}{range.EndRowIndex}";
        }

        /// <summary>
        /// Get column name (A, B, C, AA, AB...) by it's integer index
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetColumnTitle(int column)
        {
            // https://stackoverflow.com/a/41817847
            if (column < 1) return String.Empty;
            return GetColumnTitle((column - 1) / 26) + (char)('A' + (column - 1) % 26);
        }
    }
}