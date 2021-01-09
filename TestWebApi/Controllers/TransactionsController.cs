using System.Collections.Generic;
using System.Threading.Tasks;
using AspireBudgetApi;
using AspireBudgetApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Models;

namespace TestWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/transactions")]
    public class TransactionsController
    {
        private readonly IAspireApi _aspireBidgetApi;

        public TransactionsController(IAspireApi aspireApi)
        {
            _aspireBidgetApi = aspireApi;
        }
        
        /// <summary>
        /// Get transactions
        /// </summary>
        /// <param name="lastCount"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Transaction>> Get(int lastCount = 100)
        {
            return await _aspireBidgetApi.GetTransactionsAsync(lastCount);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(SaveTransactionRequest request)
        {
            var categories = await _aspireBidgetApi.GetCategoriesAsync();
            var accounts = await _aspireBidgetApi.GetAccountsAsync();
            
            //TODO: worth adding problem details
            if (!accounts.Contains(request.Account))
            {
                return new BadRequestResult();
            }
            if (!categories.Contains(request.Category))
            {
                return new BadRequestResult();
            }
            if (!string.IsNullOrEmpty(request.Cleared) && request.Cleared != Options.ClearedSymbolPending &&
                request.Cleared != Options.ClearedSymbolReconciliation &&
                request.Cleared != Options.ClearedSymbolSettled)
            {
                return new BadRequestResult();
            }
            
            var result = await _aspireBidgetApi.SaveTransactionAsync(new Transaction(
                request.Date,
                request.Outflow,
                request.Inflow,
                request.Category,
                request.Account,
                request.Memo,
                request.Cleared
                ));
            return result ? new OkResult() : new BadRequestResult();
        }
    }
}