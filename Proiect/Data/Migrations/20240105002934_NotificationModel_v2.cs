using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class NotificationModel_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Answers_AnswerId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Comments_CommentId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_AnswerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CommentId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "Notifications",
                newName: "DateDay");

            migrationBuilder.AlterColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "DateMonth",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateMonth",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewAnswer",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewBestAnswer",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewComment",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "DateDay",
                table: "Notifications",
                newName: "CommentId");

            migrationBuilder.AlterColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnswerId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AnswerId",
                table: "Notifications",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentId",
                table: "Notifications",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Answers_AnswerId",
                table: "Notifications",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Comments_CommentId",
                table: "Notifications",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
