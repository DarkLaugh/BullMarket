using BullMarket.Application.Common.Models;
using BullMarket.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BullMarket.Application.Interfaces.Services
{
    public interface IStockService
    {
        Task<List<StockResponse>> GetAllStocksPaginatedAsync(int pageIndex = 0, int pageSize = 10);
        Task<string[]> GetStockSymbols();
        Task<StockResponse> GetStockByIdAsync(Guid stockId);
    }
}
