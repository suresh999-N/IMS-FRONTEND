using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReturnModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchase_returns",
                columns: table => new
                {
                    return_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    return_number = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grn_id = table.Column<int>(type: "int", nullable: true),
                    po_id = table.Column<int>(type: "int", nullable: true),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    warehouse_id = table.Column<int>(type: "int", nullable: true),
                    return_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    return_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    return_reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reference_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_reversal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    total_return_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    approved_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approved_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    rejected_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rejected_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dispatched_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dispatched_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_returns", x => x.return_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "purchase_debit_notes",
                columns: table => new
                {
                    debit_note_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    debit_note_number = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    issue_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_debit_notes", x => x.debit_note_id);
                    table.ForeignKey(
                        name: "FK_purchase_debit_notes_purchase_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_debit_notes_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "purchase_return_attachments",
                columns: table => new
                {
                    attachment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_path = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_size = table.Column<long>(type: "bigint", nullable: true),
                    uploaded_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    uploaded_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PurchaseReturnReturnId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_return_attachments", x => x.attachment_id);
                    table.ForeignKey(
                        name: "FK_purchase_return_attachments_purchase_returns_PurchaseReturnR~",
                        column: x => x.PurchaseReturnReturnId,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "purchase_return_inspections",
                columns: table => new
                {
                    inspection_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    inspection_number = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    inspector_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    inspection_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    condition = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    result = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recommended_action = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_return_inspections", x => x.inspection_id);
                    table.ForeignKey(
                        name: "FK_purchase_return_inspections_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_return_inspections_purchase_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_return_inspections_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "purchase_return_items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    unit_id = table.Column<int>(type: "int", nullable: true),
                    received_qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    returned_qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    available_qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    return_qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_percent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    condition = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_return_items", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_purchase_return_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_return_items_purchase_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "purchase_return_shipments",
                columns: table => new
                {
                    shipment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shipment_number = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    warehouse_id = table.Column<int>(type: "int", nullable: true),
                    carrier_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tracking_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shipment_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expected_delivery_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    delivered_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    packages = table.Column<int>(type: "int", nullable: true),
                    qty = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    freight = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_return_shipments", x => x.shipment_id);
                    table.ForeignKey(
                        name: "FK_purchase_return_shipments_purchase_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_return_shipments_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_return_shipments_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "warehouse_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supplier_exchanges",
                columns: table => new
                {
                    exchange_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    exchange_number = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    original_product_id = table.Column<int>(type: "int", nullable: true),
                    replacement_product_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    expected_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    received_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_exchanges", x => x.exchange_id);
                    table.ForeignKey(
                        name: "FK_supplier_exchanges_products_original_product_id",
                        column: x => x.original_product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_exchanges_products_replacement_product_id",
                        column: x => x.replacement_product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_exchanges_purchase_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_exchanges_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supplier_refunds",
                columns: table => new
                {
                    refund_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    refund_number = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    debit_note_id = table.Column<int>(type: "int", nullable: true),
                    refund_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    refund_method = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reference_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_refunds", x => x.refund_id);
                    table.ForeignKey(
                        name: "FK_supplier_refunds_purchase_debit_notes_debit_note_id",
                        column: x => x.debit_note_id,
                        principalTable: "purchase_debit_notes",
                        principalColumn: "debit_note_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_supplier_refunds_purchase_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "purchase_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_refunds_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_debit_notes_debit_note_number",
                table: "purchase_debit_notes",
                column: "debit_note_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_debit_notes_return_id",
                table: "purchase_debit_notes",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_debit_notes_supplier_id",
                table: "purchase_debit_notes",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_attachments_PurchaseReturnReturnId",
                table: "purchase_return_attachments",
                column: "PurchaseReturnReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_inspections_inspection_number",
                table: "purchase_return_inspections",
                column: "inspection_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_inspections_product_id",
                table: "purchase_return_inspections",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_inspections_return_id",
                table: "purchase_return_inspections",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_inspections_supplier_id",
                table: "purchase_return_inspections",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_items_product_id",
                table: "purchase_return_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_items_return_id",
                table: "purchase_return_items",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_shipments_return_id",
                table: "purchase_return_shipments",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_shipments_shipment_number",
                table: "purchase_return_shipments",
                column: "shipment_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_shipments_supplier_id",
                table: "purchase_return_shipments",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_return_shipments_warehouse_id",
                table: "purchase_return_shipments",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_returns_return_number",
                table: "purchase_returns",
                column: "return_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_exchanges_exchange_number",
                table: "supplier_exchanges",
                column: "exchange_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_exchanges_original_product_id",
                table: "supplier_exchanges",
                column: "original_product_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_exchanges_replacement_product_id",
                table: "supplier_exchanges",
                column: "replacement_product_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_exchanges_return_id",
                table: "supplier_exchanges",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_exchanges_supplier_id",
                table: "supplier_exchanges",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_refunds_debit_note_id",
                table: "supplier_refunds",
                column: "debit_note_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_refunds_refund_number",
                table: "supplier_refunds",
                column: "refund_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_refunds_return_id",
                table: "supplier_refunds",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_refunds_supplier_id",
                table: "supplier_refunds",
                column: "supplier_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchase_return_attachments");

            migrationBuilder.DropTable(
                name: "purchase_return_inspections");

            migrationBuilder.DropTable(
                name: "purchase_return_items");

            migrationBuilder.DropTable(
                name: "purchase_return_shipments");

            migrationBuilder.DropTable(
                name: "supplier_exchanges");

            migrationBuilder.DropTable(
                name: "supplier_refunds");

            migrationBuilder.DropTable(
                name: "purchase_debit_notes");

            migrationBuilder.DropTable(
                name: "purchase_returns");
        }
    }
}
