using AutoMapper;
using BullMarket.Application.Common.Models;
using BullMarket.Application.DTOs.Responses;
using BullMarket.Application.Interfaces.Services;
using BullMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StockService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedListResponse<StockResponse>> GetAllStocksPaginatedAsync(int pageIndex = 0, int pageSize = 10)
        {
            await UpdateAllStockPrices();
            var allStocks = await _context.Stocks.ToListAsync();
            return new PaginatedListResponse<StockResponse>(_mapper.Map<List<StockResponse>>(allStocks), allStocks.Count(), pageIndex, pageSize);
        }

        public async Task<StockResponse> GetStockByIdAsync(Guid stockId)
        {
            var matchingStock = await _context.Stocks.FindAsync(stockId);

            return _mapper.Map<StockResponse>(matchingStock);
        }

        private async Task UpdateAllStockPrices()
        {
            var r = new Random();

            foreach (var stock in await _context.Stocks.ToListAsync())
            {
                stock.PreviousPrice = stock.CurrentPrice;
                stock.CurrentPrice = r.Next(Convert.ToInt32(stock.CurrentPrice) - 5, Convert.ToInt32(stock.CurrentPrice) + 10);
                _context.Update(stock);
            }

            await _context.SaveChangesAsync();
        }
    }
}
