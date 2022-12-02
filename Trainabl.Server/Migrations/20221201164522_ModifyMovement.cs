using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainabl.Server.Migrations
{
    public partial class ModifyMovement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetMuscleGroup",
                table: "Movements",
                newName: "Tags");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Movements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUTC",
                table: "Movements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUTC",
                table: "Movements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PrimaryMuscleGroup",
                table: "Movements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryMuscleGroup",
                table: "Movements",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "CreatedDateUTC",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "LastModifiedUTC",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "PrimaryMuscleGroup",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "SecondaryMuscleGroup",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "Movements",
                newName: "TargetMuscleGroup");
        }
    }
}
