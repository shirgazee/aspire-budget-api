using System;

namespace AspireBudgetApi
{
    public static class Options
    {
        // public static readonly string CategoriesRange = "BackendData!A2:A";
        // public static readonly string AccountsRange = "BackendData!E2:E";
        // public static readonly string TransactionsRange = "Transactions!B8:H";
        // public static readonly string CategoryTransfersRange = "Category Transfers!B9:F";
        // public static readonly string DashboardRange = "Dashboard!H4:O63";
        // public static readonly string AccountTransferCategory = "↕️ Account Transfer";
        public static readonly DateTime GoogleStartDate = new DateTime(1899, 12, 30);
    }

    /// <summary>
    /// Named ranges with a result in form of a value
    /// </summary>
    public static class ValueRangeOptions
    {
        public static readonly string Version = "v_Version";
        public static readonly string Today = "v_Today";
        public static readonly string ReportableCategorySymbol = "v_ReportableCategorySymbol";
        public static readonly string NonReportableCategorySymbol = "v_NonReportableCategorySymbol";
        public static readonly string DebtAccountSymbol = "v_DebtAccountSymbol";
        public static readonly string CategoryGroupSymbol = "v_CategoryGroupSymbol";
        public static readonly string ApprovedSymbol = "v_ApprovedSymbol";
        public static readonly string PendingSymbol = "v_PendingSymbol";
        public static readonly string BreakSymbol = "v_BreakSymbol";
        public static readonly string AccountTransfer = "v_AccountTransfer";
        public static readonly string BalanceAdjustment = "v_BalanceAdjustment";
        public static readonly string StartingBalance = "v_StartingBalance";
    }

    /// <summary>
    /// Named ranges with a result in form of a column
    /// </summary>
    public static class ColumnRangeOptions
    {
        public static readonly string TransactionsDates = "trx_Dates";
        public static readonly string TransactionsOutflows = "trx_Outflows";
        public static readonly string TransactionsInflows = "trx_Inflows";
        public static readonly string TransactionsCategories = "trx_Categories";
        public static readonly string TransactionsAccounts = "trx_Accounts";
        public static readonly string TransactionsStatuses = "trx_Statuses";
        public static readonly string NetWorthDates = "ntw_Dates";
        public static readonly string NetWorthAmounts = "ntw_Amounts";
        public static readonly string NetWorthCategories = "ntw_Categories";
        public static readonly string CategoryTransfersDates = "cts_Dates";
        public static readonly string CategoryTransfersAmounts = "cts_Amounts";
        public static readonly string CategoryTransfersFromCategories = "cts_FromCategories";
        public static readonly string CategoryTransfersToCategories = "cts_ToCategories";
        public static readonly string ConfigurationAccounts = "cfg_Accounts";
        public static readonly string ConfigurationCards = "cfg_Cards";
    }

    /// <summary>
    /// Named ranges with a result in form of a table
    /// </summary>
    public static class TableRangeOptions
    {
        public static readonly string ConfigurationData = "r_ConfigurationData";
        public static readonly string DashboardData = "r_DashboardData";
    }
}
