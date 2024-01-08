using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class NotificationModel_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewAnswer",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewBestAnswer",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewComment",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "AnswerId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AnswerId",
                table: "Notifications",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Answers_AnswerId",
                table: "Notifications",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Answers_AnswerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_AnswerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "NewAnswer",
                table: "Notifications",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NewBestAnswer",
                table: "Notifications",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NewComment",
                table: "Notifications",
                type: "bit",
                nullable: true);
        }
    }
}
