using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainabl.Server.Migrations
{
    public partial class ModifyTrainerClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientProfile_TrainerProfile_TrainerProfileId",
                table: "ClientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_ClientProfile_ClientProfileId",
                table: "Workouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_TrainerProfile_TrainerProfileId",
                table: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainerProfile",
                table: "TrainerProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientProfile",
                table: "ClientProfile");

            migrationBuilder.RenameTable(
                name: "TrainerProfile",
                newName: "TrainerProfiles");

            migrationBuilder.RenameTable(
                name: "ClientProfile",
                newName: "ClientProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_ClientProfile_TrainerProfileId",
                table: "ClientProfiles",
                newName: "IX_ClientProfiles_TrainerProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainerProfiles",
                table: "TrainerProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientProfiles",
                table: "ClientProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientProfiles_TrainerProfiles_TrainerProfileId",
                table: "ClientProfiles",
                column: "TrainerProfileId",
                principalTable: "TrainerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_ClientProfiles_ClientProfileId",
                table: "Workouts",
                column: "ClientProfileId",
                principalTable: "ClientProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_TrainerProfiles_TrainerProfileId",
                table: "Workouts",
                column: "TrainerProfileId",
                principalTable: "TrainerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientProfiles_TrainerProfiles_TrainerProfileId",
                table: "ClientProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_ClientProfiles_ClientProfileId",
                table: "Workouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_TrainerProfiles_TrainerProfileId",
                table: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainerProfiles",
                table: "TrainerProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientProfiles",
                table: "ClientProfiles");

            migrationBuilder.RenameTable(
                name: "TrainerProfiles",
                newName: "TrainerProfile");

            migrationBuilder.RenameTable(
                name: "ClientProfiles",
                newName: "ClientProfile");

            migrationBuilder.RenameIndex(
                name: "IX_ClientProfiles_TrainerProfileId",
                table: "ClientProfile",
                newName: "IX_ClientProfile_TrainerProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainerProfile",
                table: "TrainerProfile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientProfile",
                table: "ClientProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientProfile_TrainerProfile_TrainerProfileId",
                table: "ClientProfile",
                column: "TrainerProfileId",
                principalTable: "TrainerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_ClientProfile_ClientProfileId",
                table: "Workouts",
                column: "ClientProfileId",
                principalTable: "ClientProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_TrainerProfile_TrainerProfileId",
                table: "Workouts",
                column: "TrainerProfileId",
                principalTable: "TrainerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
