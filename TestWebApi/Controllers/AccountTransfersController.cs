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
    [Route("/api/v{version:apiVersion}/account-transfers")]
    public class AccountTransfersController
    {
        private readonly IAspireApi _aspireBidgetApi;

        public AccountTransfersController(IAspireApi aspireApi)
        {
            _aspireBidgetApi = aspireApi;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccountTransfer>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<AccountTransfer>> Get(int lastCount = 100)
        {
            return await _aspireBidgetApi.GetAccountTransfersAsync(lastCount);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(SaveAccountTransferRequest request)
        {
            var accounts = await _aspireBidgetApi.GetAccountsAsync();
            
            //TODO: worth adding problem details
            if (!accounts.Contains(request.AccountFrom) && !accounts.Contains(request.AccountTo))
            {
                return new BadRequestResult();
            }
            if (!string.IsNullOrEmpty(request.Cleared) && request.Cleared != Options.ClearedSymbolPending &&
                request.Cleared != Options.ClearedSymbolReconciliation &&
                request.Cleared != Options.ClearedSymbolSettled)
            {
                return new BadRequestResult();
            }

            var result = await _aspireBidgetApi.SaveAccountTransferAsync(new AccountTransfer(
                request.Date,
                request.Sum,
                request.AccountFrom,
                request.AccountTo,
                request.Memo,
                request.Cleared
                ));
            return result ? new OkResult() : new BadRequestResult();
        }
    }
}