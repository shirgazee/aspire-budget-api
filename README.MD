## .Net Standard Api for your Aspire Budget spreadsheet

Current version of this library supports **Aspire Budget v2.8**.

At first you need to turn on the Google Sheets API and obtain credentials. Follow the link and repeat steps under "Configuring Google infrastracture": ([https://medium.com/@equisept/net-core-google-sheets-api-read-write-5edd919868e3](https://medium.com/@equisept/net-core-google-sheets-api-read-write-5edd919868e3 "https://medium.com/@equisept/net-core-google-sheets-api-read-write-5edd919868e3")).

In short 😁, you will: 
- register as a developer in Google Developer Console;
- create your project;
- add Google Sheets API;
- create a service account to access your spreadsheet;
- obtain credentials to use Google Sheets API (in form of a json file).

Don't forget to share your spreadsheet with your new service account.

## Usage

```csharp
using (var api = new AspireBudgetApi.AspireBudgetApi(jsonCredentials, googleSheetId))
{
	var categories = await api.GetCategoriesAsync();
}
```
**jsonCredentials** is a content of your Google Sheets API credentials file.


## List of available methods

Accounts and categories.
```csharp
List<string> categories = await api.GetCategoriesAsync();
List<string> accounts = await api.GetAccountsAsync();
```

Transactions: all or last n. Saving transactions.
```csharp
List<Transaction> transactions = await api.GetTransactionsAsync(lastCount:100);
bool saveResult = await api.SaveTransactionAsync(transaction);
```

Category transfers: all or filtered by selected month. Saving transfers.
```csharp
List<CategoryTransfer> categoryTranfers = await api.GetCategoryTransfersAsync(month:9);
bool result = await api.SaveCategoryTranferAsync(categoryTransfer);
```

Account transfers: all or last n. Saving account transfers.
```csharp
List<AccountTransfer> accountTranfers = await api.GetAccountTranfersAsync();
bool saveResult = await api.SaveAccountTransferAsync(accountTransfer);
```
