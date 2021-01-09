using System;

namespace AspireBudgetApi
{
    public static class Options
    {
        public static readonly string CategoriesRange = "BackendData!B2:B";
        public static readonly string AccountsRange = "BackendData!I2:I";
        public static readonly string TransactionsRange = "Transactions!B9:H";
        public static readonly string CategoryTransfersRange = "Category Transfers!B8:F";
        public static readonly string DashboardRange = "Dashboard!H6:O106";
        public static readonly string AccountTransferCategory = "↕️ Account Transfer";
        public static readonly DateTime GoogleStartDate = new DateTime(1899, 12, 30);
        public static readonly string ClearedSymbolSettled = "✅";
        public static readonly string ClearedSymbolPending = "🅿️";
        public static readonly string ClearedSymbolReconciliation = "*️⃣";
    }
}
