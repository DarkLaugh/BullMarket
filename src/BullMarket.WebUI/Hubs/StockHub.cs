using BullMarket.Application.DTOs.Requests;
using BullMarket.Application.Interfaces.Clients;
using BullMarket.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BullMarket.WebUI.Hubs
{
    public class StockHub : Hub<IStockClient>
    {
        private IStockService _stockService { get; }

        public StockHub(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task AddStockComment(CommentRequest commentRequest)
        {
            var savedComment = await _stockService.AddCommentToStock(commentRequest);

            if (savedComment != null)
            {
                await this.Clients.All.AddedComment(savedComment);
            }
        }
    }
}
