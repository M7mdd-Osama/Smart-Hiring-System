using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class AddPostIdAttributeAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_PostId",
                table: "Notes",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Posts_PostId",
                table: "Notes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Posts_PostId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_PostId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Notes");
        }
    }
}
