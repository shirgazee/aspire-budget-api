using System.Collections.Generic;
using System.Threading.Tasks;
using AspireBudgetApi.Models;

namespace AspireBudgetApi
{
    public interface IAspireApi
    {
        Task<List<string>> GetCategoriesAsync();
        Task<List<string>> GetAccountsAsync();
        Task<List<Transaction>> GetTransactionsAsync(int lastCount = 0);
        Task<List<AccountTransfer>> GetAccountTransfersAsync(int lastCount = 0);
        Task<bool> SaveAccountTransferAsync(AccountTransfer transfer);
        Task<bool> SaveTransactionAsync(Transaction transaction);
        Task<List<CategoryTransfer>> GetCategoryTransfersAsync(int month = 0);
        Task<bool> SaveCategoryTransferAsync(CategoryTransfer transfer);
        Task<List<DashboardRow>> GetDashboardAsync();
        Task ClearTransactionsAndAccountTransfers();
    }
}