using Microsoft.EntityFrameworkCore.Migrations;

namespace BullMarket.Infrastructure.Migrations
{
    public partial class UpdatedCommentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommentContent",
                table: "Comments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentContent",
                table: "Comments");
        }
    }
}
