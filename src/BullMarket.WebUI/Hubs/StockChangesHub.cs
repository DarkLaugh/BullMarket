using BullMarket.Application.Common.Models;
using BullMarket.Application.DTOs.Responses;
using BullMarket.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullMarket.WebUI.Hubs
{
    public class StockChangesHub : Hub
    {
        private readonly ITimerService _timerService;
        private readonly IStockService _stockService;

        public StockChangesHub(ITimerService timerService, IStockService stockService)
        {
            _timerService = timerService;
            _stockService = stockService;
        }

        public async Task GetStockUpdates()
        {
            _timerService.Execute(Clients.All.SendAsync("stockInfoUpdated", await _stockService.GetAllStocksPaginatedAsync()));
        }
    }
}
