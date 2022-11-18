using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainabl.Server.Migrations
{
    public partial class UpdateWorkoutsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainerId",
                table: "Workouts");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Workouts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Workouts",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Workouts");

            migrationBuilder.AddColumn<Guid>(
                name: "TrainerId",
                table: "Workouts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
