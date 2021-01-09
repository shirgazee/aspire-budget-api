using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspireBudgetApi.Models
{
    public class Transaction
    {
        public DateTime Date { get; private set; }
        public double Outflow { get; private set; }
        public double Inflow { get; private set; }
        public string Category { get; private set; }
        public string Account { get; private set; }
        public string Memo { get; private set; }
        public string Cleared { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        public Transaction(DateTime date, 
            double outflow, 
            double inflow, 
            string category, 
            string account, 
            string memo = null, 
            string cleared = null)
        {
            Date = date;
            Outflow = outflow;
            Inflow = inflow;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Memo = memo;
            Cleared = cleared;
        }

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
                result.Add(transaction.Outflow.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                result.Add("");
            }

            if (transaction.Inflow != 0)
            {
                result.Add(transaction.Inflow.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                result.Add("");
            }

            result.Add(transaction.Category);
            result.Add(transaction.Account);
            result.Add(transaction.Memo ?? "");
            result.Add(transaction.Cleared ?? "");
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
            (
                Options.GoogleStartDate.AddDays(days),
                outflow,
                inflow,
                row[3].ToString().Trim(),
                row[4].ToString().Trim()
            );
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
