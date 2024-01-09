using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class newcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Awards_DiscussionId",
                table: "Awards");

            migrationBuilder.AddColumn<bool>(
                name: "didAward",
                table: "Discussions",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Awards_DiscussionId",
                table: "Awards",
                column: "DiscussionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Awards_DiscussionId",
                table: "Awards");

            migrationBuilder.DropColumn(
                name: "didAward",
                table: "Discussions");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_DiscussionId",
                table: "Awards",
                column: "DiscussionId",
                unique: true,
                filter: "[DiscussionId] IS NOT NULL");
        }
    }
}
