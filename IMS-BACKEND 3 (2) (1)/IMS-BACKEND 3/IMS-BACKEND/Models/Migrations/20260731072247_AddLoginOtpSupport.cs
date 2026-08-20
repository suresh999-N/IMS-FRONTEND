using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginOtpSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.RenameTable(
                name: "Otps",
                newName: "otps");

           

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "otps",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "otps",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "otps",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "otps",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_otps",
                table: "otps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_otps_UserId",
                table: "otps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_otps_Users_UserId",
                table: "otps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_otps_Users_UserId",
                table: "otps");

          

            migrationBuilder.DropIndex(
                name: "IX_otps_UserId",
                table: "otps");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "otps");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "otps");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "otps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "otps");

            migrationBuilder.RenameTable(
                name: "otps",
                newName: "Otps");

            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Otps",
                table: "Otps",
                column: "Id");
        }
    }
}
