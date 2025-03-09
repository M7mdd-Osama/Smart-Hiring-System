using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class EditCandidateListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLists_Companies_CompanyId",
                table: "CandidateLists");

            migrationBuilder.DropIndex(
                name: "IX_CandidateLists_CompanyId",
                table: "CandidateLists");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CandidateLists");

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "CandidateLists",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateLists_ManagerId",
                table: "CandidateLists",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLists_AspNetUsers_ManagerId",
                table: "CandidateLists",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLists_AspNetUsers_ManagerId",
                table: "CandidateLists");

            migrationBuilder.DropIndex(
                name: "IX_CandidateLists_ManagerId",
                table: "CandidateLists");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "CandidateLists");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "CandidateLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateLists_CompanyId",
                table: "CandidateLists",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLists_Companies_CompanyId",
                table: "CandidateLists",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
