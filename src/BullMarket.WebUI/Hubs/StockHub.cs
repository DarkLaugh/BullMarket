using BullMarket.WebUI.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullMarket.WebUI.Hubs
{
    public class StockHub : Hub<IStockClient>
    {
    }
}
