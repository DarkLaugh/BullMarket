using BullMarket.Application.Common.Models;
using BullMarket.Application.DTOs.Responses;
using System;
using System.Threading.Tasks;

namespace BullMarket.Application.Interfaces.Services
{
    public interface IStockService
    {
        Task<PaginatedListResponse<StockResponse>> GetAllStocksPaginatedAsync(int pageIndex = 0, int pageSize = 10);
        Task<StockResponse> GetStockByIdAsync(Guid stockId);
    }
}
