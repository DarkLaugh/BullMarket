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
                            new Stock ("GameStop Corp.", "GME", 30.05F, 12.00F, 100000),
                            new Stock ("AMC Entertainment Holdings, Inc.", "AMC", 5.59F, 9.00F, 100000),
                            new Stock ("BlackBerry Limited", "BB", 13.04F, 5.01F, 100000),
                            new Stock ("Nokia Corporation", "NOK", 4.17F, 1.97F, 100000),
                            new Stock ("QUALCOMM Incorporated", "QCOM", 147.98F, 100.27F, 100000),
                            new Stock ("Intel Corporation", "INTC", 61.81F, 58.35F, 100000),
                            new Stock ("Apple Inc.", "AAPL", 135.37F, 142.98F, 100000),
                            new Stock ("Tesla, Inc.", "TSLA", 816.12F, 810.85F, 100000)
                        });

                        await appContext.SaveChangesAsync();
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
