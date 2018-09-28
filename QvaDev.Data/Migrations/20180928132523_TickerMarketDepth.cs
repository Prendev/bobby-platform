using Microsoft.EntityFrameworkCore.Migrations;

namespace QvaDev.Data.Migrations
{
    public partial class TickerMarketDepth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarketDepth",
                table: "Tickers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketDepth",
                table: "Tickers");
        }
    }
}
