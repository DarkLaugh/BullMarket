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
            var allStocks = await _context.Stocks.ToListAsync();
            return new PaginatedListResponse<StockResponse>(_mapper.Map<List<StockResponse>>(allStocks), allStocks.Count(), pageIndex, pageSize);
        }

        public async Task<StockResponse> GetStockByIdAsync(Guid stockId)
        {
            return _mapper.Map<StockResponse>(await _context.Stocks.FindAsync(stockId));
        }
    }
}
