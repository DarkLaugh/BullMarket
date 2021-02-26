using BullMarket.Domain.Entities;
using BullMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services
{
    public class MigratorService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MigratorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    await appContext.Database.MigrateAsync();
                    if (!await appContext.Stocks.AnyAsync())
                    {
                        await appContext.Stocks.AddRangeAsync(new List<Stock>
                        {
                            new Stock ("us_equity", "NASDAQ", "AAPL", "active", true, true, true, true, 1.23m),
                            new Stock ("us_equity", "NYSE", "AMC", "active", true, true, true, true, 12.38m),
                            new Stock ("us_equity", "NYSE", "BB", "active", true, true, false, false, 11.22m),
                            new Stock ("us_equity", "NYSE", "NOK", "active", true, false, true, true, 6.78m),
                            new Stock ("us_equity", "NASDAQ", "QCOM", "active", true, true, false, true, 56.12m),
                            new Stock ("us_equity", "NASDAQ", "INTC", "active", true, false, false, false, 101.10m),
                            new Stock ("us_equity", "NASDAQ", "TSLA", "active", true, true, true, true, 154.78m)
                        });

                        await appContext.SaveChangesAsync();
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
