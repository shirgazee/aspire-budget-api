using System;
using System.Collections.Generic;
using System.Text;

namespace AspireBudgetApi
{
    public static class Options
    {
        public static readonly string CategoriesRange = "BackendData!A2:A";
        public static readonly string AccountsRange = "BackendData!E2:E";
        public static readonly string TransactionsRange = "Transactions!B8:H";
        public static readonly string CategoryTransfersRange = "Category Transfers!B9:F";
        public static readonly string DashboardRange = "Dashboard!H4:O63";
        public static readonly string AccountTransferCategory = "↕️ Account Transfer";
        public static readonly DateTime GoogleStartDate = new DateTime(1899, 12, 30);
    }
}
