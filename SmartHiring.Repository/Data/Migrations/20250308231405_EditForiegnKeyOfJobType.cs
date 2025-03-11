using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class EditForiegnKeyOfJobType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_JobTypes_LocationId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Posts",
                newName: "JobTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_LocationId",
                table: "Posts",
                newName: "IX_Posts_JobTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_JobTypes_JobTypeId",
                table: "Posts",
                column: "JobTypeId",
                principalTable: "JobTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_JobTypes_JobTypeId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "JobTypeId",
                table: "Posts",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_JobTypeId",
                table: "Posts",
                newName: "IX_Posts_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_JobTypes_LocationId",
                table: "Posts",
                column: "LocationId",
                principalTable: "JobTypes",
                principalColumn: "Id");
        }
    }
}
