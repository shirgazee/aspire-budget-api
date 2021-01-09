using System;
using System.Collections.Generic;
using System.Text;

namespace AspireBudgetApi.Models
{
    public class AccountTransfer
    {
        public DateTime Date { get; private set; }
        public double Sum { get; private set; }
        public string AccountFrom { get; private set; }
        public string AccountTo { get; private set; }
        public string Memo { get; private set; }
        public string Cleared { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        public AccountTransfer(
            DateTime date,
            double sum,
            string accountFrom,
            string accountTo,
            string memo = null,
            string cleared = null
            )
        {
            Date = date;
            Sum = sum;
            AccountFrom = accountFrom ?? throw new ArgumentNullException(nameof(accountFrom));
            AccountTo = accountTo ?? throw new ArgumentNullException(nameof(accountTo));
            Memo = memo;
            Cleared = cleared;
        }

        public static AccountTransfer FromTransactions(Transaction t1, Transaction t2)
        {
            if (Math.Abs(t1.Inflow - t2.Outflow) > 0.01 || Math.Abs(t1.Outflow - t2.Inflow) > 0.01)
            {
                throw new ArgumentException("Invalid parameters data");
            }

            if (t1.Category != Options.AccountTransferCategory || t2.Category != Options.AccountTransferCategory)
            {
                throw new ArgumentException("Invalid parameters data");
            }

            return new AccountTransfer(
                t1.Date,
                Math.Max(t1.Inflow, t1.Outflow),
                t1.Outflow > 0 ? t1.Account : t2.Account,
                t1.Inflow > 0 ? t1.Account : t2.Account,
                !string.IsNullOrEmpty(t1.Memo) || !string.IsNullOrEmpty(t2.Memo) ? string.Join("; ", t1.Memo.Trim(), t2.Memo.Trim()) : "",
                t1.Cleared == t2.Cleared ? t1.Cleared : ""
            );
        }

        public static Transaction[] ToTransactions(AccountTransfer transfer)
        {
            var t1 = new Transaction(
                transfer.Date,
                transfer.Sum,
                0,
                Options.AccountTransferCategory,
                transfer.AccountFrom,
                transfer.Memo,
                transfer.Cleared
            );
            var t2 = new Transaction
            (
                transfer.Date,
                0,
                transfer.Sum,
                Options.AccountTransferCategory,
                transfer.AccountTo,
                "",
                transfer.Cleared
            );

            return new[] { t1, t2 };
        }
    }
}
