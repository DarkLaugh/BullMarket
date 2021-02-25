using Alpaca.Markets;
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
        where THub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<THub> _hub;
        private readonly IServiceProvider _serviceProvider;
        private IAlpacaDataStreamingClient _client;
        private List<IAlpacaDataSubscription<IStreamQuote>> _subscriptions = new List<IAlpacaDataSubscription<IStreamQuote>>();
        private Dictionary<string, DateTime> _lastBroadcastedMessages = new Dictionary<string, DateTime>();

        public StreamingService(IConfiguration configuration, IHubContext<THub> hub, IServiceProvider serviceProvider)
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

            var stocks = new List<Stock>();

            using (var scope = _serviceProvider.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    var stocksFromDb = await appContext.Stocks.ToListAsync();
                    foreach (var stock in stocksFromDb)
                    {
                        stocks.Add(stock);
                    }
                }
            }

            foreach (var stock in stocks)
            {
                var subscription = _client.GetQuoteSubscription(stock.Symbol);
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
                await _hub.Clients.All.SendAsync("stockUpdate", obj);
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
                    _lastBroadcastedMessages.Add(obj.Symbol, obj.TimeUtc);
                }
                return true;
            }
            return false;
        }
    }
}
