using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class AddInterviewStatusEnumAndScoreToInterviewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Interviews");

            migrationBuilder.AddColumn<int>(
                name: "InterviewStatus",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Interviews",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewStatus",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Interviews");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Interviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
