using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddGrnNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_grn_number",
                table: "goods_receipts",
                column: "grn_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_goods_receipts_grn_number",
                table: "goods_receipts");

            migrationBuilder.DropColumn(
                name: "grn_number",
                table: "goods_receipts");
        }
    }
}
