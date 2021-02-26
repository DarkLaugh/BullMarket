using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullMarket.WebUI.Hubs.Clients
{
    public interface IStockClient
    {
        Task StockUpdate(IStreamQuote data);
    }
}
