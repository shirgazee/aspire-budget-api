using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspireBudgetApi;
using AspireBudgetApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IAspireApi _aspireBidgetApi;

        public DashboardController(IAspireApi aspireApi)
        {
            _aspireBidgetApi = aspireApi;
        }

        /// <summary>
        /// Get list of dashboard rows with groups and categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DashboardRow>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<DashboardRow>> Get()
        {
            return await _aspireBidgetApi.GetDashboardAsync();
        }
    }
}