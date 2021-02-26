using BullMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BullMarket.Infrastructure.Persistence.Configurations
{
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClassName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Exchange)
                .HasMaxLength(10)
                .IsRequired();
            builder.Property(x => x.Status)
                .HasMaxLength(8)
                .IsRequired();
            builder.Property(x => x.Symbol)
                .HasMaxLength(10)
                .IsRequired();
            builder.Property(x => x.Price)
                .HasColumnType("decimal(8,2)");
        }
    }
}
