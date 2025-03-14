using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class AddCompanyIdToPostEntity : Migration
    {
		protected override void Up(MigrationBuilder migrationBuilder)
		{

			migrationBuilder.AddColumn<int>(
				name: "CompanyId",
				table: "Posts",
				nullable: false,
				defaultValue: 4);

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_Companies_CompanyId",
				table: "Posts",
				column: "CompanyId",
				principalTable: "Companies",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Posts_Companies_CompanyId",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "CompanyId",
				table: "Posts");
		}
	}
}
