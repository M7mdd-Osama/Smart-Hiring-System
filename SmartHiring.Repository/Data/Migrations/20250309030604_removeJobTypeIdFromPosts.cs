using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class removeJobTypeIdFromPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_JobTypes_JobTypeId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_JobTypeId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "JobTypeId",
                table: "Posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobTypeId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_JobTypeId",
                table: "Posts",
                column: "JobTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_JobTypes_JobTypeId",
                table: "Posts",
                column: "JobTypeId",
                principalTable: "JobTypes",
                principalColumn: "Id");
        }
    }
}
