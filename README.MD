## .Net Standard Api for your Aspire Budget spreadsheet

Current version of this library supports **Aspire Budget v3.3**.

At first you need to turn on the Google Sheets API and obtain credentials. Follow the link and repeat steps under "Configuring Google infrastracture": ([https://medium.com/@equisept/net-core-google-sheets-api-read-write-5edd919868e3](https://medium.com/@equisept/net-core-google-sheets-api-read-write-5edd919868e3 "https://medium.com/@equisept/net-core-google-sheets-api-read-write-5edd919868e3")).

In short 😁, you will: 
- register as a developer in Google Developer Console;
- create your project;
- add Google Sheets API;
- create a service account to access your spreadsheet;
- obtain credentials to use Google Sheets API (in form of a json file).

Don't forget to share your spreadsheet with your new service account.

## Usage
Direct instantiation:\
`jsonCredentials` variable is a content of your Google Sheets API credentials file.
```csharp
using (var api = new AspireBudgetApi.AspireBudgetApi(jsonCredentials, googleSheetId))
{
	var categories = await api.GetCategoriesAsync();
}
```
or using DI container:

`AspireApiOptions` class in this example stores credentials and sheet ID.
```csharp
services.AddScoped<IAspireApi, AspireApi>(x =>
{
    var options = x.GetRequiredService<IOptions<AspireApiOptions>>();
    var logger = x.GetRequiredService<ILogger<IAspireApi>>(); // optional
    var json = Encoding.UTF8.GetString(Convert.FromBase64String(options.Value.ApiCredentialsBase64));
    return new AspireApi(json, options.Value.SheetId, logger);
});
```
You can find a working example in `TestWebApi` project.

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

## Using TestWebApi
**Launch from IDE (requires .NET 5 SDK):**\
Update `AspireApiOptions` section in appsettings.json file with your Google Sheet Id and base64-encoded content of your API credentials json file.\
Then run TestWebApi project. It will open up a new browser window with Swagger url.

```json
"AspireApiOptions" : {
    "SheetId": "",
    "ApiCredentialsBase64": ""
  }
```

**Launch from docker-compose:**\
Update `docker-compose.yml` with your your Google Sheet Id and base64-encoded content of your API credentials json file.
Then run `docker-compose up`, API will be available at [http://localhost:5010/swagger/index.html]().
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ASPIREAPIOPTIONS__SHEETID={here}
  - ASPIREAPIOPTIONS__APICREDENTIALSBASE64={here}
```
