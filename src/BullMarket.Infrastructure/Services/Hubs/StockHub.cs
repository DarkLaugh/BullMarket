using BullMarket.Infrastructure.Services.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services.Hubs
{
    public class StockHub : Hub<IStockClient>
    {
        public StockHub()
        {
        }
    }
}
