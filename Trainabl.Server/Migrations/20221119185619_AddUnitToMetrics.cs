using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainabl.Server.Migrations
{
    public partial class AddUnitToMetrics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Metrics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Metrics");
        }
    }
}
