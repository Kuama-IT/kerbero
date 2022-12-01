using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kerbero.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowForRefreshTokenInsideNukiCredential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "SmartLockKeys",
                newName: "Password");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "NukiCredentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ExpiresIn",
                table: "NukiCredentials",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneratedWithUrl",
                table: "NukiCredentials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "NukiCredentials",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefreshable",
                table: "NukiCredentials",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "NukiCredentials",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "NukiCredentials");

            migrationBuilder.DropColumn(
                name: "ExpiresIn",
                table: "NukiCredentials");

            migrationBuilder.DropColumn(
                name: "GeneratedWithUrl",
                table: "NukiCredentials");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "NukiCredentials");

            migrationBuilder.DropColumn(
                name: "IsRefreshable",
                table: "NukiCredentials");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "NukiCredentials");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "SmartLockKeys",
                newName: "Token");
        }
    }
}
