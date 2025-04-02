using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class EditAgencyApplicantsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AgencyApplicants",
                table: "AgencyApplicants");

            migrationBuilder.DropIndex(
                name: "IX_AgencyApplicants_AgencyId_ApplicantId",
                table: "AgencyApplicants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AgencyApplicants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgencyApplicants",
                table: "AgencyApplicants",
                columns: new[] { "AgencyId", "ApplicantId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AgencyApplicants",
                table: "AgencyApplicants");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AgencyApplicants",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgencyApplicants",
                table: "AgencyApplicants",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyApplicants_AgencyId_ApplicantId",
                table: "AgencyApplicants",
                columns: new[] { "AgencyId", "ApplicantId" },
                unique: true);
        }
    }
}
