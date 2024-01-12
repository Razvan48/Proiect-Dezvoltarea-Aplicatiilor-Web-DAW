using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class _123xx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscussionId",
                table: "Codespaces",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCode",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Codespaces_DiscussionId",
                table: "Codespaces",
                column: "DiscussionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Codespaces_Discussions_DiscussionId",
                table: "Codespaces",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Codespaces_Discussions_DiscussionId",
                table: "Codespaces");

            migrationBuilder.DropIndex(
                name: "IX_Codespaces_DiscussionId",
                table: "Codespaces");

            migrationBuilder.DropColumn(
                name: "DiscussionId",
                table: "Codespaces");

            migrationBuilder.DropColumn(
                name: "IsCode",
                table: "Answers");
        }
    }
}
