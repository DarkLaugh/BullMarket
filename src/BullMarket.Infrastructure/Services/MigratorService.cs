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
                            new Stock ("us_equity", "NASDAQ", "AAPL", "active", true, true, true, true),
                            new Stock ("us_equity", "NYSE", "AMC", "active", true, true, true, true),
                            new Stock ("us_equity", "NYSE", "BB", "active", true, true, false, false),
                            new Stock ("us_equity", "NYSE", "NOK", "active", true, false, true, true),
                            new Stock ("us_equity", "NASDAQ", "QCOM", "active", true, true, false, true),
                            new Stock ("us_equity", "NASDAQ", "INTC", "active", true, false, false, false),
                            new Stock ("us_equity", "NASDAQ", "TSLA", "active", true, true, true, true)
                        });

                        await appContext.SaveChangesAsync();
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
