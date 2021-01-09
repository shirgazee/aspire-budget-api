using System;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestWebApi
{
    /// <summary>
    /// Clear transactions from the google sheet every x minutes
    /// </summary>
    public class RollbackBackgroundService : BackgroundService
    {
        private readonly ILogger<RollbackBackgroundService> _logger;
        private readonly IServiceProvider _services;
        
        public RollbackBackgroundService(IServiceProvider services, 
            ILogger<RollbackBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    $"{nameof(RollbackBackgroundService)} running.");

                using (var scope = _services.CreateScope())
                {
                    var aspireApi = scope.ServiceProvider.GetRequiredService<IAspireApi>();

                    await aspireApi.ClearTransactionsAndAccountTransfers();
                }
            
                await Task.Delay(10 * 60 * 1000, stoppingToken);
            }
        }
    }
}