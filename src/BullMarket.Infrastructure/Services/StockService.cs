using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<string[]> GetStockSymbols()
        {
            var result = await _context
                .Stocks
                .Select(s => s.Symbol)
                .ToArrayAsync();

            return result;
        }

        public async Task<IEnumerable<StockResponse>> GetAllStocksAsync()
        {
            var allStocks = await _context
                .Stocks
                .ProjectTo<StockResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return allStocks;
        }

        public async Task<StockResponse> GetStockByIdAsync(Guid stockId)
        {
            return _mapper.Map<StockResponse>(await _context.Stocks.FindAsync(stockId));
        }
    }
}
