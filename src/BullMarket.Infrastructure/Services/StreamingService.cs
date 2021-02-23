using Alpaca.Markets;
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
        private IAlpacaDataStreamingClient _client;
        private IAlpacaDataSubscription<IStreamQuote> _subscription;

        public StreamingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = Alpaca.Markets.Environments.Paper
                 .GetAlpacaDataStreamingClient(new SecretKey(_configuration.GetSection("AlpacaAPI").GetSection("KeyId").Value, 
                                                        _configuration.GetSection("AlpacaAPI").GetSection("SecretKey").Value));

            await _client.ConnectAndAuthenticateAsync();

            _subscription = _client.GetQuoteSubscription("AAPL");

            _subscription.Received += SubscriptionResult_Received;

            _client.Subscribe(_subscription);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Unsubscribe(_subscription);

            await _client.DisconnectAsync();
        }

        private void SubscriptionResult_Received(IStreamQuote obj)
        {
            Console.WriteLine(obj.AskPrice);
        }
    }
}
