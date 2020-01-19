using AspireBudgetApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{ Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") }.json", optional: true);
            IConfigurationRoot configuration = builder.Build();

            var apiCredentialsFile = configuration.GetSection("apiCredentialsFile").Value;
            var sheetId = configuration.GetSection("sheetId").Value;
            string json = File.ReadAllText(apiCredentialsFile, Encoding.UTF8);

            using (var api = new AspireBudgetApi.AspireBudgetApi(json, sheetId))
            {
                #region GetCategories

                //var categories = api.GetCategoriesAsync().Result;
                //foreach(var category in categories)
                //{
                //    Console.WriteLine(category);
                //}

                #endregion

                #region GetAccounts

                //var accounts = api.GetAccountsAsync().Result;
                //foreach (var account in accounts)
                //{
                //    Console.WriteLine(account);
                //}

                #endregion

                #region GetTransactions

                //var transactions = api.GetTransactionsAsync(100).Result.ToList();
                //for(int i =0; i < transactions.Count; i++)
                //{
                //    var transaction = transactions[i];
                //    Console.WriteLine($"#{i} {transaction.Date} {transaction.Inflow} {transaction.Outflow} {transaction.Category} {transaction.Account} {transaction.Memo}");
                //}

                #endregion

                #region SaveTransaction

                //var testTransaction = new Transaction
                //{
                //    Date = DateTime.Today,
                //    Outflow = 1,
                //    Category = YOUR_CATEGORY,
                //    Account = YOUR_ACCOUT,
                //    Memo = "Test",
                //    Cleared = "🆗"
                //};
                //var saveResult = api.SaveTransactionAsync(testTransaction).Result;

                #endregion

                #region GetCategoryTransfers

                //var categoryTranfers = api.GetCategoryTransfersAsync(9).Result;
                //foreach(var tranfer in categoryTranfers)
                //{
                //    Console.WriteLine($"{tranfer.Date}  {tranfer.Amount} {tranfer.FromCategory} {tranfer.ToCategory} {tranfer.Memo}");
                //}

                #endregion

                #region SaveCategoryTransfer

                //var categoryTransfer = new CategoryTransfer()
                //{
                //    Date = DateTime.Today,
                //    Amount = 10000,
                //    FromCategory = "Available to budget",
                //    ToCategory = "Продукты",
                //    Memo = "Test"
                //};
                //var result = api.SaveCategoryTransferAsync(categoryTransfer).Result;
                //Console.WriteLine(result);

                #endregion

                #region GetAccountTransfers

                //var accountTranfers = api.GetAccountTransfersAsync().Result;
                //foreach (var accountTransfer in accountTranfers)
                //{
                //    Console.WriteLine($"{accountTransfer.Date} {accountTransfer.AccountFrom} {accountTransfer.AccountTo} {accountTransfer.Sum} {accountTransfer.Memo} {accountTransfer.Cleared}");
                //}

                #endregion

                #region SaveAccountTransfer

                //var accountTransfer = new AccountTransfer()
                //{
                //    Date = DateTime.Today,
                //    AccountFrom = YOUR_ACCOUNT,
                //    AccountTo = YOUR_ACCOUNT,
                //    Sum = 10000,
                //    Cleared = "🆗",
                //    Memo = "Test"
                //};
                //var saveResult = api.SaveAccountTransferAsync(accountTransfer).Result;
                //Console.WriteLine(saveResult);

                #endregion

                #region GetDashboard

                //var dashboard = api.GetDashboardAsync().Result;
                //foreach (var row in dashboard)
                //{
                //    if (row.Type == DashboardRowType.Group)
                //    {
                //        Console.WriteLine($"** {row.Name} **");
                //    }
                //    else
                //    {
                //        Console.WriteLine($"-- {row.Name}. Av.: {row.Available}, Spent: {row.Spent}, Targ. {row.Target}, Goal: {row.Goal}, Budg.: {row.Budgeted}");
                //    }
                //}

                #endregion
            }

            Console.ReadKey();
        }
    }
}
