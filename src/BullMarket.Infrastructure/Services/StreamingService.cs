using Alpaca.Markets;
using BullMarket.Infrastructure.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services
{
    public class StreamingService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<StockUpdateHub> _stockUpdateHub;
        private IAlpacaDataStreamingClient _client;
        private IAlpacaDataSubscription<IStreamQuote> _subscription;

        public StreamingService(IConfiguration configuration, IHubContext<StockUpdateHub> stockUpdateHub)
        {
            _configuration = configuration;
            _stockUpdateHub = stockUpdateHub;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = Alpaca.Markets.Environments.Paper
                 .GetAlpacaDataStreamingClient(new SecretKey(_configuration.GetSection("AlpacaAPI").GetSection("KeyId").Value, 
                                                        _configuration.GetSection("AlpacaAPI").GetSection("SecretKey").Value));

            await _client.ConnectAndAuthenticateAsync();

            _subscription = _client.GetQuoteSubscription("AAPL");

            _subscription.Received += async (result) => await SubscriptionResult_Received(result);

            _client.Subscribe(_subscription);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Unsubscribe(_subscription);

            await _client.DisconnectAsync();
        }

        private async Task SubscriptionResult_Received(IStreamQuote obj)
        {
            await _stockUpdateHub.Clients.All.SendAsync("stockUpdate", obj);
        }
    }
}
