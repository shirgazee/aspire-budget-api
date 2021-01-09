using System.Collections.Generic;
using System.Threading.Tasks;
using AspireBudgetApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/configuration")]
    public class ConfigurationController
    {
        private readonly IAspireApi _aspireBidgetApi;

        public ConfigurationController(IAspireApi aspireApi)
        {
            _aspireBidgetApi = aspireApi;
        }

        /// <summary>
        /// Get list of categories
        /// </summary>
        /// <returns></returns>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<string>> GetCategories()
        {
            return await _aspireBidgetApi.GetCategoriesAsync();
        }
        
        /// <summary>
        /// Get list of accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet("accounts")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<string>> GetAccounts()
        {
            return await _aspireBidgetApi.GetAccountsAsync();
        }
    }
}