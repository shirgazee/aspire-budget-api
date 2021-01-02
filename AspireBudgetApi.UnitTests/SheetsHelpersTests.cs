using System;
using AspireBudgetApi.Helpers;
using Google.Apis.Sheets.v4.Data;
using Xunit;

namespace AspireBudgetApi.UnitTests
{
    public class SheetsHelpersTests
    {
        [Theory]
        [InlineData(1, "A")]
        [InlineData(2, "B")]
        [InlineData(26, "Z")]
        [InlineData(27, "AA")]
        [InlineData(26 * 26 + 26 + 1, "AAA")]
        [InlineData(26 * 26 + 2 * 26 + 1, "ABA")]
        [InlineData(26 * 26 * 26 + 26 * 26 + 26, "ZZZ")]
        public void GetColumnTitle_Default_ReturnsTitleLetters(int columnIndex, string expectedTitle)
        {
            // Act
            var title = SheetsHelpers.GetColumnTitle(columnIndex);

            // Assert
            Assert.Equal(expectedTitle, title);
        }

        [Theory]
        [InlineData("BackendData", 41, 42, 1, 2, "'BackendData'!AP2:AP2")]
        [InlineData("Transactions", 1, 2, 8, 3000, "'Transactions'!B9:B3000")]
        [InlineData("Dashboard", 5, 16, 5, 105, "'Dashboard'!F6:P105")]
        [InlineData("Category Transfers", 1, 2, 8, 2999, "'Category Transfers'!B9:B2999")]
        public void GetA1RangeNotation_Default_ReturnsA1RangeNotation(string sheetName,
            int startColumnIndex,
            int endColumnIndex,
            int startRowIndex,
            int endRowIndex,
            string expectedA1Notation)
        {
            // Arrange
            var range = new GridRange
            {
                StartColumnIndex = startColumnIndex,
                EndColumnIndex = endColumnIndex,
                StartRowIndex = startRowIndex,
                EndRowIndex = endRowIndex
            };

            // Act
            var title = SheetsHelpers.GetA1RangeNotation(sheetName, range);

            // Assert
            Assert.Equal(expectedA1Notation, title);
        }
    }
}