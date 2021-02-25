using Alpaca.Markets;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services.Hubs.Clients
{
    public interface IStockClient
    {
        Task StockUpdate(IStreamQuote data);
    }
}
