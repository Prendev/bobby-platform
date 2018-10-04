using Microsoft.EntityFrameworkCore.Migrations;

namespace QvaDev.Data.Migrations
{
    public partial class StratHubArbHighRiskSignalDiffInPip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinOpenTimeInMinutes",
                table: "StratHubArbs",
                newName: "RestingPeriodInMinutes");

            migrationBuilder.AddColumn<decimal>(
                name: "HighRiskSignalDiffInPip",
                table: "StratHubArbs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighRiskSignalDiffInPip",
                table: "StratHubArbs");

            migrationBuilder.RenameColumn(
                name: "RestingPeriodInMinutes",
                table: "StratHubArbs",
                newName: "MinOpenTimeInMinutes");
        }
    }
}
