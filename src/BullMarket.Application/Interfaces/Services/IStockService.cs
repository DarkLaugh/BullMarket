using BullMarket.Application.Common.Models;
using BullMarket.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BullMarket.Application.Interfaces.Services
{
    public interface IStockService
    {
        Task<IEnumerable<StockResponse>> GetAllStocksAsync();
        Task<string[]> GetStockSymbols();
        Task<StockResponse> GetStockByIdAsync(Guid stockId);
    }
}
