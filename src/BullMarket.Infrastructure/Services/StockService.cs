using AutoMapper;
using AutoMapper.QueryableExtensions;
using BullMarket.Application.DTOs.Requests;
using BullMarket.Application.DTOs.Responses;
using BullMarket.Application.Interfaces.Services;
using BullMarket.Domain.Entities;
using BullMarket.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ClaimsPrincipal _user;
        private readonly UserManager<ApplicationUser> _userManager;

        public StockService(
            ApplicationDbContext context, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _user = httpContextAccessor?.HttpContext?.User;
            _userManager = userManager;
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
            var stock = _mapper.Map<StockResponse>(await _context
                .Stocks
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == stockId));

            stock.Comments = stock.Comments.OrderByDescending(c => c.CreatedOn).ToList();

            return stock;
        }

        public async Task<CommentResponse> AddCommentToStock(CommentRequest commentRequest)
        {
            var commentToSave = _mapper.Map<Comment>(commentRequest);
            var user = await _userManager.FindByNameAsync(commentRequest.Username);
            commentToSave.UserId = user.Id;

            await _context.Comments.AddAsync(commentToSave);
            if (await _context.SaveAuditableChangesAsync(commentRequest.Username, commentToSave.Id) > 0)
            {
                var savedComment = _mapper.Map<CommentResponse>(await _context.Comments.FindAsync(commentToSave.Id));
                return savedComment;
            }

            return null;
        }
    }
}
