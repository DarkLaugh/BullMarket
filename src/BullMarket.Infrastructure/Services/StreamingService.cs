using Alpaca.Markets;
using BullMarket.Application.DTOs.Responses;
using BullMarket.Application.Interfaces.Clients;
using BullMarket.Domain.Entities;
using BullMarket.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services
{
    public class StreamingService<THub> : IHostedService
        where THub : Hub<IStockClient>
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<THub, IStockClient> _hub;
        private readonly IServiceProvider _serviceProvider;
        private IAlpacaDataStreamingClient _client;
        private List<IAlpacaDataSubscription<IStreamQuote>> _subscriptions = new List<IAlpacaDataSubscription<IStreamQuote>>();
        private Dictionary<string, DateTime> _lastBroadcastedMessages = new Dictionary<string, DateTime>();

        public StreamingService(IConfiguration configuration, IHubContext<THub, IStockClient> hub, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _hub = hub;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = Alpaca.Markets.Environments.Paper
                 .GetAlpacaDataStreamingClient(new SecretKey(_configuration.GetSection("AlpacaAPI").GetSection("KeyId").Value,
                                                        _configuration.GetSection("AlpacaAPI").GetSection("SecretKey").Value));

            await _client.ConnectAndAuthenticateAsync();

            var stocks = await GetStocksForStreaming();

            foreach (var stock in stocks)
            {
                var subscription = _client.GetQuoteSubscription(stock);
                _subscriptions.Add(subscription);
                subscription.Received += async (result) => await SubscriptionResult_Received(result);
            }

            _client.Subscribe(_subscriptions);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Unsubscribe(_subscriptions);

            await _client.DisconnectAsync();
        }

        private async Task SubscriptionResult_Received(IStreamQuote obj)
        {
            if(CheckIfLastMessageIsOldEnough(obj))
            {
                await _hub.Clients.All.StockUpdate(new StockChangedResponse
                {
                    Symbol = obj.Symbol,
                    Price = obj.BidPrice
                });
                await UpdateStockPrice(obj.Symbol, obj.AskPrice);
            }
        }

        private bool CheckIfLastMessageIsOldEnough(IStreamQuote obj)
        {
            var lastMessage = _lastBroadcastedMessages.FirstOrDefault(x => x.Key == obj.Symbol);
            if (lastMessage.Key == null || (DateTime.UtcNow - lastMessage.Value).Seconds > 20)
            {
                if (lastMessage.Key == null)
                {
                    _lastBroadcastedMessages.TryAdd(obj.Symbol, DateTime.UtcNow);

                }
                else
                {
                    _lastBroadcastedMessages.Remove(lastMessage.Key);
                    _lastBroadcastedMessages.TryAdd(obj.Symbol, obj.TimeUtc);
                }
                return true;
            }
            return false;
        }

        private async Task<IEnumerable<string>> GetStocksForStreaming()
        {
            var stocks = new List<string>();

            using (var scope = _serviceProvider.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    var stocksFromDb = await appContext.Stocks.Select(s => s.Symbol).ToListAsync();
                    foreach (var stock in stocksFromDb)
                    {
                        stocks.Add(stock);
                    }
                }
            }

            return stocks;
        }

        private async Task UpdateStockPrice(string symbol, decimal price)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    var stockInDb = await appContext.Stocks.FirstOrDefaultAsync(x => x.Symbol == symbol);
                    stockInDb.Price = price;

                    appContext.Update(stockInDb);
                    await appContext.SaveChangesAsync();
                }
            }
        }
    }
}
