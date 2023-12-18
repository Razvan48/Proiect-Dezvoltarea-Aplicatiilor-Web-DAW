using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class _127 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberVotes",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberVotes",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CommentId",
                table: "Votes",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Comments_CommentId",
                table: "Votes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Comments_CommentId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_CommentId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "NumberVotes",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "NumberVotes",
                table: "Answers");
        }
    }
}
