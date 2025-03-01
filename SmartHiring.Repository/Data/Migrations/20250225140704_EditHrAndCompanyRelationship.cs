using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class EditHrAndCompanyRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_AdminId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_AdminId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Companies",
                newName: "HRId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_HRId",
                table: "Companies",
                column: "HRId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_HRId",
                table: "Companies",
                column: "HRId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_HRId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_HRId",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "HRId",
                table: "Companies",
                newName: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AdminId",
                table: "Companies",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_AdminId",
                table: "Companies",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
