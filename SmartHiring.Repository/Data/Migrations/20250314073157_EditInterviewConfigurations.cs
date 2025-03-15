using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class EditInterviewConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_AspNetUsers_HRId",
                table: "Interviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_AspNetUsers_HRId",
                table: "Interviews",
                column: "HRId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_AspNetUsers_HRId",
                table: "Interviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_AspNetUsers_HRId",
                table: "Interviews",
                column: "HRId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
