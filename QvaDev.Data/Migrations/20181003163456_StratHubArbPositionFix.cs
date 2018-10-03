using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QvaDev.Data.Migrations
{
    public partial class StratHubArbPositionFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StratHubArbPositions",
                table: "StratHubArbPositions");

            migrationBuilder.DropIndex(
                name: "IX_StratHubArbPositions_PositionId",
                table: "StratHubArbPositions");

            migrationBuilder.DropIndex(
                name: "IX_StratHubArbPositions_StratHubArbId",
                table: "StratHubArbPositions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StratHubArbPositions");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StratHubArbPositions_PositionId_StratHubArbId",
                table: "StratHubArbPositions",
                columns: new[] { "PositionId", "StratHubArbId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StratHubArbPositions",
                table: "StratHubArbPositions",
                columns: new[] { "StratHubArbId", "PositionId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_StratHubArbPositions_PositionId_StratHubArbId",
                table: "StratHubArbPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StratHubArbPositions",
                table: "StratHubArbPositions");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StratHubArbPositions",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StratHubArbPositions",
                table: "StratHubArbPositions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StratHubArbPositions_PositionId",
                table: "StratHubArbPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StratHubArbPositions_StratHubArbId",
                table: "StratHubArbPositions",
                column: "StratHubArbId");
        }
    }
}
