using IMSBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Globalization;
using System.IO;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================
        // SALES REPORT
        // =====================================
        [HttpGet("sales")]
        public async Task<IActionResult> SalesReport()
        {
            var data = await (
                from invoice in _context.Invoices
                join c in _context.Customers
                    on invoice.CustomerId equals c.CustomerId
                select new
                {
                    SoId = invoice.InvoiceId,
                    InvoiceId = invoice.InvoiceId,
                    customer = c.Name,
                    SoNumber = invoice.InvoiceNumber,
                    InvoiceNumber = invoice.InvoiceNumber,
                    OrderDate = invoice.InvoiceDate,
                    InvoiceDate = invoice.InvoiceDate,
                    invoice.TotalAmount,
                    invoice.Status
                }
            ).ToListAsync();

            return Ok(data);
        }

        // =====================================
        // PURCHASE REPORT
        // =====================================
        [HttpGet("purchases")]
        public async Task<IActionResult> PurchaseReport()
        {
            var data = await (
                from p in _context.PurchaseOrders
                join s in _context.Suppliers
                    on p.SupplierId equals s.SupplierId
                select new
                {
                    p.PoId,
                    supplier = s.Name,
                    p.PoNumber,
                    p.OrderDate,
                    p.TotalAmount,
                    p.Status
                }
            ).ToListAsync();

            return Ok(data);
        }

        // =====================================
        // INVOICE REPORT
        // =====================================
        [HttpGet("invoices")]
        public async Task<IActionResult> InvoiceReport()
        {
            var data = await (
                from i in _context.Invoices
                join c in _context.Customers
                    on i.CustomerId equals c.CustomerId
                select new
                {
                    i.InvoiceId,
                    customer = c.Name,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.PaidAmount,
                    i.BalanceAmount,
                    i.Status
                }
            ).ToListAsync();

            return Ok(data);
        }

        // =====================================
        // STOCK REPORT
        // =====================================
        [HttpGet("stock")]
        public async Task<IActionResult> StockReport()
        {
            var data = await (
                from s in _context.Stocks
                join p in _context.Products
                    on s.ProductId equals p.ProductId
                join w in _context.Warehouses
                    on s.WarehouseId equals w.WarehouseId
                join c in _context.Categories
                    on p.CategoryId equals c.CategoryId into categoryGroup
                from category in categoryGroup.DefaultIfEmpty()
                join sub in _context.SubCategories
                    on p.SubCategoryId equals sub.SubCategoryId into subCategoryGroup
                from subCategory in subCategoryGroup.DefaultIfEmpty()
                join b in _context.Brands
                    on p.BrandId equals b.BrandId into brandGroup
                from brand in brandGroup.DefaultIfEmpty()
                select new
                {
                    s.StockId,
                    image = p.ImageUrl,
                    product = p.Name,
                    category = category != null ? category.Name : "",
                    subCategory = subCategory != null ? subCategory.Name : "",
                    brand = brand != null ? brand.Name : "",
                    price = p.Price,
                    stock = s.Quantity,
                    status = p.Status,
                    warehouse = w.Name,
                    s.Quantity,
                    s.ReservedQuantity,
                    s.AvailableQuantity
                }
            ).ToListAsync();

            return Ok(data);
        }

        // =====================================
        // CUSTOMER BALANCE REPORT
        // =====================================
        [HttpGet("customer-balances")]
        public async Task<IActionResult> CustomerBalanceReport()
        {
            var data = await _context.Customers
                .Select(c => new
                {
                    c.CustomerId,
                    c.Name,
                    c.Company,
                    c.CreditLimit,
                    c.OutstandingBalance,
                    c.Status
                })
                .ToListAsync();

            return Ok(data);
        }


        // =========================
        // EXPORT SALES REPORT
        // =========================
        [HttpGet("export-sales")]
        public async Task<IActionResult> ExportSalesReport()
        {
            var sales = await (
                from invoice in _context.Invoices
                join customer in _context.Customers
                    on invoice.CustomerId equals customer.CustomerId

                select new
                {
                    invoice.InvoiceNumber,
                    Customer = customer.Name,
                    invoice.InvoiceDate,
                    invoice.TotalAmount,
                    invoice.PaidAmount,
                    invoice.BalanceAmount,
                    invoice.Status
                }
            ).ToListAsync();

            using var workbook = new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add("Sales Report");

            // ================= HEADERS =================
            worksheet.Cell(1, 1).Value = "Invoice No";
            worksheet.Cell(1, 2).Value = "Customer";
            worksheet.Cell(1, 3).Value = "Invoice Date";
            worksheet.Cell(1, 4).Value = "Total Amount";
            worksheet.Cell(1, 5).Value = "Paid Amount";
            worksheet.Cell(1, 6).Value = "Balance";
            worksheet.Cell(1, 7).Value = "Status";

            // ================= DATA =================
            int row = 2;

            foreach (var item in sales)
            {
                worksheet.Cell(row, 1).Value = item.InvoiceNumber;
                worksheet.Cell(row, 2).Value = item.Customer;
                worksheet.Cell(row, 3).Value = item.InvoiceDate.ToString();
                worksheet.Cell(row, 4).Value = item.TotalAmount;
                worksheet.Cell(row, 5).Value = item.PaidAmount;
                worksheet.Cell(row, 6).Value = item.BalanceAmount;
                worksheet.Cell(row, 7).Value = item.Status;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SalesReport.xlsx");
        }

        [HttpGet("export-sales-pdf")]
        public async Task<IActionResult> ExportSalesReportPdf()
        {
            var sales = await (
                from invoice in _context.Invoices
                join customer in _context.Customers
                    on invoice.CustomerId equals customer.CustomerId
                select new
                {
                    invoice.InvoiceNumber,
                    Customer = customer.Name,
                    invoice.InvoiceDate,
                    invoice.TotalAmount,
                    invoice.PaidAmount,
                    invoice.BalanceAmount,
                    invoice.Status
                }
            ).ToListAsync();

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, PageSize.A4.Rotate());

            document.SetMargins(28, 28, 28, 28);
            AddReportTitle(document, "Sales Report", $"Generated {DateTime.Now:dd MMM yyyy, hh:mm tt}");

            var table = new Table(new float[] { 1.4f, 1.8f, 1.2f, 1.2f, 1.2f, 1.2f, 1.1f })
                .UseAllAvailableWidth();

            AddHeaderCells(table, new[] { "Invoice No", "Customer", "Invoice Date", "Total", "Paid", "Balance", "Status" });

            foreach (var item in sales)
            {
                AddBodyCell(table, item.InvoiceNumber);
                AddBodyCell(table, item.Customer);
                AddBodyCell(table, FormatReportDate(item.InvoiceDate));
                AddBodyCell(table, FormatReportNumber(item.TotalAmount));
                AddBodyCell(table, FormatReportNumber(item.PaidAmount));
                AddBodyCell(table, FormatReportNumber(item.BalanceAmount));
                AddBodyCell(table, item.Status);
            }

            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "SalesReport.pdf");
        }



        // =========================
        // EXPORT STOCK REPORT
        // =========================
        [HttpGet("export-stock")]
        public async Task<IActionResult> ExportStockReport()
        {
            var stocks = await (
                from stock in _context.Stocks
                join product in _context.Products
                    on stock.ProductId equals product.ProductId
                join warehouse in _context.Warehouses
                    on stock.WarehouseId equals warehouse.WarehouseId
                join category in _context.Categories
                    on product.CategoryId equals category.CategoryId into categoryGroup
                from category in categoryGroup.DefaultIfEmpty()
                join subCategory in _context.SubCategories
                    on product.SubCategoryId equals subCategory.SubCategoryId into subCategoryGroup
                from subCategory in subCategoryGroup.DefaultIfEmpty()
                join brand in _context.Brands
                    on product.BrandId equals brand.BrandId into brandGroup
                from brand in brandGroup.DefaultIfEmpty()

                select new
                {
                    Image = product.ImageUrl,
                    Product = product.Name,
                    Category = category != null ? category.Name : "",
                    SubCategory = subCategory != null ? subCategory.Name : "",
                    Brand = brand != null ? brand.Name : "",
                    product.Price,
                    ProductStock = stock.Quantity,
                    product.Status,
                    Warehouse = warehouse.Name,
                    stock.Quantity,
                    stock.ReservedQuantity,
                    stock.AvailableQuantity
                }
            ).ToListAsync();

            using var workbook = new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add("Stock Report");

            // ================= HEADERS =================

            worksheet.Cell(1, 1).Value = "Product Image";
            worksheet.Cell(1, 2).Value = "Product";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "SubCategory";
            worksheet.Cell(1, 5).Value = "Brand";
            worksheet.Cell(1, 6).Value = "Price";
            worksheet.Cell(1, 7).Value = "Stock";
            worksheet.Cell(1, 8).Value = "Status";
            worksheet.Cell(1, 9).Value = "Warehouse";
            worksheet.Cell(1, 10).Value = "Quantity";
            worksheet.Cell(1, 11).Value = "Reserved";
            worksheet.Cell(1, 12).Value = "Available";

            // ================= DATA =================

            int row = 2;

            foreach (var item in stocks)
            {
                worksheet.Cell(row, 1).Value = item.Image ?? "";
                worksheet.Cell(row, 2).Value = item.Product;
                worksheet.Cell(row, 3).Value = item.Category;
                worksheet.Cell(row, 4).Value = item.SubCategory;
                worksheet.Cell(row, 5).Value = item.Brand;
                worksheet.Cell(row, 6).Value = item.Price;
                worksheet.Cell(row, 7).Value = item.ProductStock;
                worksheet.Cell(row, 8).Value = item.Status;
                worksheet.Cell(row, 9).Value = item.Warehouse;
                worksheet.Cell(row, 10).Value = item.Quantity;
                worksheet.Cell(row, 11).Value = item.ReservedQuantity;
                worksheet.Cell(row, 12).Value = item.AvailableQuantity;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockReport.xlsx");
        }






        [HttpGet("export-stock-pdf")]
        public async Task<IActionResult> ExportStockReportPdf()
        {
            var stocks = await (
                from stock in _context.Stocks
                join product in _context.Products
                    on stock.ProductId equals product.ProductId
                join warehouse in _context.Warehouses
                    on stock.WarehouseId equals warehouse.WarehouseId
                join category in _context.Categories
                    on product.CategoryId equals category.CategoryId into categoryGroup
                from category in categoryGroup.DefaultIfEmpty()
                join subCategory in _context.SubCategories
                    on product.SubCategoryId equals subCategory.SubCategoryId into subCategoryGroup
                from subCategory in subCategoryGroup.DefaultIfEmpty()
                join brand in _context.Brands
                    on product.BrandId equals brand.BrandId into brandGroup
                from brand in brandGroup.DefaultIfEmpty()

                select new
                {
                    Product = product.Name,
                    Category = category != null ? category.Name : "",
                    SubCategory = subCategory != null ? subCategory.Name : "",
                    Brand = brand != null ? brand.Name : "",
                    product.Price,
                    product.Status,
                    Warehouse = warehouse.Name,
                    stock.Quantity,
                    stock.ReservedQuantity,
                    stock.AvailableQuantity
                }
            ).ToListAsync();

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, PageSize.A4.Rotate());

            document.SetMargins(28, 28, 28, 28);
            AddReportTitle(document, "Stock Report", $"Generated {DateTime.Now:dd MMM yyyy, hh:mm tt}");

            var table = new Table(new float[] { 1.9f, 1.3f, 1.3f, 1.1f, 1f, 1.1f, 0.9f, 0.9f, 0.9f })
                .UseAllAvailableWidth();

            AddHeaderCells(table, new[] { "Product", "Category", "SubCategory", "Brand", "Price", "Warehouse", "On Hand", "Reserved", "Available" });

            foreach (var item in stocks)
            {
                AddBodyCell(table, item.Product);
                AddBodyCell(table, item.Category);
                AddBodyCell(table, item.SubCategory);
                AddBodyCell(table, item.Brand);
                AddBodyCell(table, FormatReportNumber(item.Price));
                AddBodyCell(table, item.Warehouse);
                AddBodyCell(table, item.Quantity.ToString());
                AddBodyCell(table, item.ReservedQuantity.ToString());
                AddBodyCell(table, item.AvailableQuantity.ToString());
            }

            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "StockReport.pdf");
        }

        private static void AddReportTitle(Document document, string title, string subtitle)
        {
            document.Add(new Paragraph("IMS Reports")
                .SetFontSize(10)
                .SetFontColor(ColorConstants.DARK_GRAY));
            document.Add(new Paragraph(title)
                .SetFontSize(20)
                .SetFontColor(new DeviceRgb(15, 23, 42)));
            document.Add(new Paragraph(subtitle)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY));
            document.Add(new Paragraph(" "));
        }

        private static void AddHeaderCells(Table table, string[] headers)
        {
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell()
                    .SetBackgroundColor(new DeviceRgb(241, 245, 249))
                    .SetFontColor(new DeviceRgb(51, 65, 85))
                    .SetFontSize(9)
                    .Add(new Paragraph(header)));
            }
        }

        private static void AddBodyCell(Table table, string? value)
        {
            table.AddCell(new Cell()
                .SetFontSize(8.5f)
                .SetFontColor(new DeviceRgb(30, 41, 59))
                .Add(new Paragraph(value ?? "")));
        }

        private static string FormatReportDate(object? value)
        {
            return value switch
            {
                null => "",
                DateTime dateTime => dateTime.ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                DateOnly dateOnly => dateOnly.ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                _ => value.ToString() ?? ""
            };
        }

        private static string FormatReportNumber(object? value)
        {
            return value switch
            {
                null => "0.00",
                IFormattable formattable => formattable.ToString("N2", CultureInfo.InvariantCulture),
                _ => value.ToString() ?? "0.00"
            };
        }
    }
}
