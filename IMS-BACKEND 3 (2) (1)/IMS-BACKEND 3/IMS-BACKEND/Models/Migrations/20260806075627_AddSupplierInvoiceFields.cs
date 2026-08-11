using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierInvoiceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierInvoice",
                table: "goods_receipts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SupplierInvoiceDate",
                table: "goods_receipts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                table: "goods_receipt_items",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "line_total",
                table: "goods_receipt_items",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tax",
                table: "goods_receipt_items",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierInvoice",
                table: "goods_receipts");

            migrationBuilder.DropColumn(
                name: "SupplierInvoiceDate",
                table: "goods_receipts");

            migrationBuilder.DropColumn(
                name: "discount",
                table: "goods_receipt_items");

            migrationBuilder.DropColumn(
                name: "line_total",
                table: "goods_receipt_items");

            migrationBuilder.DropColumn(
                name: "tax",
                table: "goods_receipt_items");
        }
    }
}
