using BullMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BullMarket.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Stock> Stocks { get; set; }
    }
}
