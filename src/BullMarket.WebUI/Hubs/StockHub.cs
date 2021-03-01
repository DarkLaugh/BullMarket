using BullMarket.Application.DTOs.Requests;
using BullMarket.Application.Interfaces.Clients;
using BullMarket.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BullMarket.WebUI.Hubs
{
    [Authorize]
    public class StockHub : Hub<IStockClient>
    {
        private IStockService _stockService { get; }

        public StockHub(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task AddStockComment(CommentRequest commentRequest)
        {
            commentRequest.Username = Context.User.Identity.Name;
            var savedComment = await _stockService.AddCommentToStock(commentRequest);

            if (savedComment != null)
            {
                await this.Clients.All.AddedComment(savedComment);
            }
        }
    }
}
