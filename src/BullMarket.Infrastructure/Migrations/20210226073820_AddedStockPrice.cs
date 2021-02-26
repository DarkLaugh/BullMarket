using Microsoft.EntityFrameworkCore.Migrations;

namespace BullMarket.Infrastructure.Migrations
{
    public partial class AddedStockPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Stocks",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Stocks");
        }
    }
}
