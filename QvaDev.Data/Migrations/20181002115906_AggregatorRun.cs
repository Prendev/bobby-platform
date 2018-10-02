using Microsoft.EntityFrameworkCore.Migrations;

namespace QvaDev.Data.Migrations
{
    public partial class AggregatorRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Run",
                table: "Aggregators",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Run",
                table: "Aggregators");
        }
    }
}
