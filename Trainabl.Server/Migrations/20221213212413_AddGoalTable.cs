using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainabl.Server.Migrations
{
    public partial class AddGoalTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalType = table.Column<int>(type: "int", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    Metric = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comparator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetValue = table.Column<double>(type: "float", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeadlineUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_ClientProfiles_ClientProfileId",
                        column: x => x.ClientProfileId,
                        principalTable: "ClientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_ClientProfileId",
                table: "Goals",
                column: "ClientProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");
        }
    }
}
