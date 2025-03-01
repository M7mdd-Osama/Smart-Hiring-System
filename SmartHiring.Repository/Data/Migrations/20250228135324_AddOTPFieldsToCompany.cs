using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHiring.Repository.Data.Migrations
{
    public partial class AddOTPFieldsToCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationCodeExpires",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ConfirmationCodeExpires",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Companies");
        }
    }
}
