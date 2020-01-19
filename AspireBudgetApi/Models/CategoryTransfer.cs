using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AspireBudgetApi.Models
{
    public class CategoryTransfer
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string FromCategory { get; set; }
        public string ToCategory { get; set; }
        public string Memo { get; set; }

        public static List<object> ToGoogleRow(CategoryTransfer transfer)
        {
            if (transfer.Date == null || transfer.Date < Options.GoogleStartDate)
            {
                throw new ArgumentException($"Incorrect date in {nameof(Transaction)}.{nameof(ToGoogleRow)} initializer");
            }

            if(string.IsNullOrWhiteSpace(transfer.FromCategory))
            {
                throw new ArgumentException($"No FromCategory specified in {nameof(Transaction)}.{nameof(ToGoogleRow)} initializer");
            }

            if (string.IsNullOrWhiteSpace(transfer.ToCategory))
            {
                throw new ArgumentException($"No ToCategory specified in {nameof(Transaction)}.{nameof(ToGoogleRow)} initializer");
            }

            var result = new List<object>
            {
                transfer.Date.ToString("yyyy-MM-dd"),
                transfer.Amount.ToString(CultureInfo.InvariantCulture),
                transfer.FromCategory.ToString(),
                transfer.ToCategory.ToString(),
                transfer.Memo?.ToString() ?? ""
            };

            return result;
        }

        public static CategoryTransfer FromGoogleRow(IList<object> row)
        {
            if (string.IsNullOrEmpty(row[0].ToString()))
            {
                throw new ArgumentException($"No data in row for {nameof(CategoryTransfer)}.{nameof(FromGoogleRow)} initializer");
            }

            int.TryParse(row[0].ToString(), out var days);
            double.TryParse(row[1].ToString(), out var amount);
            var result = new CategoryTransfer
            {
                Date = Options.GoogleStartDate.AddDays(days),
                Amount = amount,
                FromCategory = row[2].ToString(),
                ToCategory = row[3].ToString(),
            };

            if(row.Count == 5)
            {
                result.Memo = row[4].ToString();
            }

            return result;
        }
    }
}
