using Alpaca.Markets;
using BullMarket.Application.DTOs.Responses;
using System.Threading.Tasks;

namespace BullMarket.Application.Interfaces.Clients
{
    public interface IStockClient
    {
        Task StockUpdate(IStreamQuote data);
        Task AddedComment(CommentResponse comment);
    }
}
