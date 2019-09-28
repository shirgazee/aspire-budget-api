using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspireBudgetApi.Models
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public double Outflow { get; set; }
        public double Inflow { get; set; }
        public string Category { get; set; }
        public string Account { get; set; }
        public string Memo { get; set; }
        public string Cleared { get; set; }

        public static List<object> ToGoogleRow(Transaction transaction)
        {
            if (transaction.Date == null || transaction.Date < Options.GoogleStartDate)
            {
                throw new ArgumentException($"Incorrect date in {nameof(Transaction)}.{nameof(ToGoogleRow)} initializer");
            }
            if(string.IsNullOrWhiteSpace(transaction.Category))
            {
                throw new ArgumentException($"No category specified in {nameof(Transaction)}.{nameof(ToGoogleRow)} initializer");
            }
            if (string.IsNullOrWhiteSpace(transaction.Account))
            {
                throw new ArgumentException($"No account specified in {nameof(Transaction)}.{nameof(ToGoogleRow)} initializer");
            }

            var result = new List<object>();
            result.Add(transaction.Date.ToString("yyyy-MM-dd"));
            if (transaction.Outflow != 0)
            {
                result.Add(transaction.Outflow.ToString());
            }
            else
            {
                result.Add("");
            }
            if (transaction.Inflow != 0)
            {
                result.Add(transaction.Inflow.ToString());
            }
            else
            {
                result.Add("");
            }
            result.Add(transaction.Category.ToString());
            result.Add(transaction.Account.ToString());
            result.Add(transaction.Memo?.ToString() ?? "");
            result.Add(transaction.Cleared?.ToString() ?? "");
            return result;
        }

        public static Transaction FromGoogleRow(IList<object> row)
        {
            if (string.IsNullOrEmpty(row[0].ToString()))
            {
                throw new ArgumentException($"No data in row for {nameof(Transaction)}.{nameof(FromGoogleRow)} initializer");
            }
            int.TryParse(row[0].ToString(), out var days);
            double.TryParse(row[1].ToString(), out var outflow);
            double.TryParse(row[2].ToString(), out var inflow);
            var result = new Transaction
            {
                Date = Options.GoogleStartDate.AddDays(days),
                Outflow = outflow,
                Inflow = inflow,
                Category = row[3].ToString(),
                Account = row[4].ToString(),
            };
            if(row.Count > 5)
            {
                result.Memo = row[5].ToString();
            }
            if (row.Count > 6)
            {
                result.Cleared = row[6].ToString();
            }
            return result;
        }
    }
}
