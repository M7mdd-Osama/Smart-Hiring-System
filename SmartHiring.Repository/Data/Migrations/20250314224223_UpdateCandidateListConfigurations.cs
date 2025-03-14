using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class UpdateCandidateListConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLists_AspNetUsers_ManagerId",
                table: "CandidateLists");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLists_Posts_PostId",
                table: "CandidateLists");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLists_AspNetUsers_ManagerId",
                table: "CandidateLists",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLists_Posts_PostId",
                table: "CandidateLists",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLists_AspNetUsers_ManagerId",
                table: "CandidateLists");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLists_Posts_PostId",
                table: "CandidateLists");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLists_AspNetUsers_ManagerId",
                table: "CandidateLists",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLists_Posts_PostId",
                table: "CandidateLists",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
