using ClosedXML.Excel;
using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Globalization;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Reports")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // CORE REPORTS
        // These endpoints intentionally return the complete filtered
        // collection (no pagination), because the current frontend
        // derives the remaining report tabs from these five datasets.
        // ============================================================

        [HttpGet("sales")]
        public async Task<IActionResult> SalesReport([FromQuery] ReportQuery query)
        {
            var data = await SalesReportRows(query, includeCancelled: false);

            var rows = data.Select(invoice => new
            {
                id = invoice.InvoiceId,
                soId = invoice.SoId ?? invoice.InvoiceId,
                invoiceId = invoice.InvoiceId,
                customerId = invoice.CustomerId,
                customer = invoice.Customer,
                customerName = invoice.CustomerName,
                soNumber = invoice.InvoiceNumber,
                invoiceNumber = invoice.InvoiceNumber,
                orderDate = invoice.InvoiceDate,
                invoiceDate = invoice.InvoiceDate,
                totalAmount = invoice.TotalAmount,
                paidAmount = invoice.PaidAmount,
                balanceAmount = invoice.BalanceAmount,
                status = invoice.Status,
                warehouseId = invoice.WarehouseId,
                warehouseName = invoice.WarehouseName,
                warehouse = invoice.WarehouseName,
                items = invoice.Items.Select(item => new
                {
                    productId = item.ProductId,
                    productName = item.ProductName,
                    product = item.ProductName,
                    sku = item.SKU,
                    categoryId = item.CategoryId,
                    category = item.CategoryName,
                    quantity = item.Quantity,
                    price = item.Price,
                    total = item.Total,
                    taxAmount = item.TaxAmount,
                    warehouseId = item.WarehouseId,
                    warehouseName = item.WarehouseName,
                    warehouse = item.WarehouseName
                }).ToList()
            }).ToList();

            return Ok(rows);
        }

        [HttpGet("purchases")]
        public async Task<IActionResult> PurchaseReport([FromQuery] ReportQuery query)
        {
            var purchases = await PurchaseRows(query);

            var rows = purchases.Select(purchase => new
            {
                id = purchase.PoId,
                poId = purchase.PoId,
                poNumber = purchase.PoNumber,
                supplierId = purchase.SupplierId,
                supplier = purchase.Supplier,
                supplierName = purchase.Supplier,
                orderDate = purchase.OrderDate,
                totalAmount = purchase.TotalAmount,
                status = purchase.Status,
                warehouseId = purchase.WarehouseId,
                warehouseName = purchase.WarehouseName,
                warehouse = purchase.WarehouseName,
                items = purchase.Items.Select(item => new
                {
                    productId = item.ProductId,
                    productName = item.ProductName,
                    product = item.ProductName,
                    warehouseId = item.WarehouseId,
                    warehouseName = item.WarehouseName,
                    warehouse = item.WarehouseName
                }).ToList()
            }).ToList();

            return Ok(rows.OrderByDescending(x => x.orderDate).ToList());
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> InvoiceReport([FromQuery] ReportQuery query)
        {
            // Invoice report uses the same warehouse resolution as Sales:
            // invoice.warehouse_id first, then product.warehouse_id from
            // invoice items when invoice.warehouse_id is null.
            var result = await SalesReportRows(query, includeCancelled: false);

            var rows = result.Select(x => new
            {
                id = x.invoiceId,
                invoiceId = x.invoiceId,
                customerId = x.customerId,
                customer = x.customer,
                customerName = x.customerName,
                invoiceNumber = x.invoiceNumber,
                invoiceDate = x.invoiceDate,
                totalAmount = x.totalAmount,
                paidAmount = x.paidAmount,
                balanceAmount = x.balanceAmount,
                status = x.status,
                warehouseId = x.warehouseId,
                warehouseName = x.warehouseName,
                warehouse = x.warehouse,
                items = x.items
            });

            return Ok(rows.OrderByDescending(x => x.invoiceDate).ToList());
        }

        [HttpGet("stock")]
        public async Task<IActionResult> StockReport([FromQuery] ReportQuery query)
        {
            var data = await GetStockRows(query);

            var rows = data.Select(stock => new
            {
                id = stock.StockId,
                stockId = stock.StockId,
                productId = stock.ProductId,
                product = stock.Product,
                productName = stock.Product,
                name = stock.Product,
                sku = stock.SKU,
                categoryId = stock.CategoryId,
                category = stock.Category,
                categoryName = stock.Category,
                warehouseId = stock.WarehouseId,
                warehouseName = stock.Warehouse,
                warehouse = stock.Warehouse,
                price = stock.Price,
                costPrice = stock.CostPrice,
                quantity = stock.Quantity,
                reservedQuantity = stock.ReservedQuantity,
                availableQuantity = stock.AvailableQuantity,
                status = stock.Status,
                reorderLevel = stock.ReorderLevel
            }).ToList();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                rows = rows.Where(x =>
                    x.product.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.sku.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.warehouse.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.category.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(rows);
        }

        [HttpGet("customer-balances")]
        public async Task<IActionResult> CustomerBalanceReport([FromQuery] ReportQuery query)
        {
            // With no warehouse selected, preserve the existing customer-level
            // balance semantics.
            if (!HasWarehouseFilter(query))
            {
                var data = _context.Customers.AsNoTracking().Select(customer => new
                {
                    id = customer.CustomerId,
                    customerId = customer.CustomerId,
                    name = customer.Name,
                    customer = customer.Name,
                    customerName = customer.Name,
                    company = customer.Company,
                    creditLimit = customer.CreditLimit,
                    outstandingBalance = customer.OutstandingBalance,
                    status = customer.Status,
                    warehouseId = (int?)null,
                    warehouseName = "",
                    warehouse = ""
                });

                if (!IsAll(query.Status))
                    data = data.Where(x => x.status == query.Status);

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var search = query.Search.Trim();
                    data = data.Where(x =>
                        x.name.Contains(search) ||
                        (x.company ?? "").Contains(search));
                }

                return Ok(await data.OrderByDescending(x => x.outstandingBalance).ToListAsync());
            }

            // A customer does not have warehouse_id in the DB. For a
            // warehouse-specific balance, calculate it from invoices assigned
            // to the requested warehouse, with product-warehouse fallback.
            var sales = await SalesReportRows(query, includeCancelled: false);

            var customerIds = sales
                .Where(x => x.customerId.HasValue)
                .GroupBy(x => new
                {
                    x.customerId,
                    x.customer,
                    x.warehouseId,
                    x.warehouseName
                })
                .Select(g => new
                {
                    customerId = g.Key.customerId!.Value,
                    name = g.Key.customer,
                    company = "",
                    creditLimit = 0m,
                    outstandingBalance = g.Sum(x => x.balanceAmount),
                    status = "active",
                    warehouseId = g.Key.warehouseId,
                    warehouseName = g.Key.warehouseName,
                    warehouse = g.Key.warehouseName
                })
                .Where(x => x.outstandingBalance != 0)
                .ToList();

            if (!IsAll(query.Status))
                customerIds = customerIds
                    .Where(x => string.Equals(x.status, query.Status, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return Ok(customerIds.Select((x, index) => new
            {
                id = x.customerId,
                x.customerId,
                x.name,
                customer = x.name,
                customerName = x.name,
                x.company,
                x.creditLimit,
                x.outstandingBalance,
                x.status,
                x.warehouseId,
                x.warehouseName,
                x.warehouse
            }).OrderByDescending(x => x.outstandingBalance).ToList());
        }

        // ============================================================
        // FILTER SOURCES
        // ============================================================

        [HttpGet("filters/warehouses")]
        public async Task<IActionResult> GetWarehouses()
        {
            return Ok(await _context.Warehouses.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.WarehouseId,
                    warehouseId = x.WarehouseId,
                    name = x.Name,
                    warehouseName = x.Name
                })
                .ToListAsync());
        }

        [HttpGet("filters/categories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _context.Categories.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.CategoryId,
                    categoryId = x.CategoryId,
                    name = x.Name
                })
                .ToListAsync());
        }

        [HttpGet("filters/products")]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _context.Products.AsNoTracking()
                .Where(x => !x.IsDeleted && !x.IsArchived)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.ProductId,
                    productId = x.ProductId,
                    name = x.Name,
                    productName = x.Name,
                    sku = x.SKU,
                    warehouseId = x.WarehouseId,
                    categoryId = x.CategoryId
                })
                .ToListAsync());
        }

        [HttpGet("filters/customers")]
        public async Task<IActionResult> GetCustomers()
        {
            return Ok(await _context.Customers.AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.CustomerId,
                    customerId = x.CustomerId,
                    name = x.Name,
                    customerName = x.Name
                })
                .ToListAsync());
        }

        [HttpGet("filters/suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            return Ok(await _context.Suppliers.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.SupplierId,
                    supplierId = x.SupplierId,
                    name = x.Name,
                    supplierName = x.Name
                })
                .ToListAsync());
        }

        // ============================================================
        // ANALYTICS USED BY THE REPORTS MODULE
        // ============================================================

        [HttpGet("transaction-trend")]
        public async Task<IActionResult> TransactionTrend([FromQuery] ReportQuery query)
        {
            var sales = (await SalesReportRows(query, false))
                .GroupBy(x => x.invoiceDate?.ToString("yyyy-MM"))
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new
                {
                    month = g.Key!,
                    sales = g.Sum(x => x.totalAmount)
                })
                .ToList();

            var purchases = (await PurchaseRows(query))
                .GroupBy(x => x.orderDate?.ToString("yyyy-MM"))
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new
                {
                    month = g.Key!,
                    purchases = g.Sum(x => x.totalAmount)
                })
                .ToList();

            var months = sales.Select(x => x.month)
                .Concat(purchases.Select(x => x.month))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Ok(months.Select(month => new
            {
                month,
                monthName = DateTime.ParseExact(month + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMM"),
                sales = sales.FirstOrDefault(x => x.month == month)?.sales ?? 0m,
                purchases = purchases.FirstOrDefault(x => x.month == month)?.purchases ?? 0m
            }));
        }

        [HttpGet("stock-availability")]
        public async Task<IActionResult> StockAvailability([FromQuery] ReportQuery query)
        {
            var stock = await GetStockRows(query);

            return Ok(new
            {
                inStock = stock.Count(x => x.AvailableQuantity > 10),
                lowStock = stock.Count(x => x.AvailableQuantity > 0 && x.AvailableQuantity <= x.ReorderLevel),
                outOfStock = stock.Count(x => x.AvailableQuantity <= 0)
            });
        }

        [HttpGet("top-customers")]
        public async Task<IActionResult> TopCustomers([FromQuery] ReportQuery query)
        {
            var rows = await SalesReportRows(query, false);

            var result = rows
                .Where(x => x.customerId.HasValue)
                .GroupBy(x => new { x.customerId, x.customer, x.warehouseId, x.warehouseName })
                .Select(g => new
                {
                    customerId = g.Key.customerId,
                    customerName = g.Key.customer,
                    totalOrders = g.Count(),
                    totalSalesValue = g.Sum(x => x.totalAmount),
                    outstandingAmount = g.Sum(x => x.balanceAmount),
                    lastPurchaseDate = g.Max(x => x.invoiceDate),
                    warehouseId = g.Key.warehouseId,
                    warehouseName = g.Key.warehouseName,
                    warehouse = g.Key.warehouseName
                })
                .OrderByDescending(x => x.totalSalesValue);

            return Ok(result);
        }

        [HttpGet("top-suppliers")]
        public async Task<IActionResult> TopSuppliers([FromQuery] ReportQuery query)
        {
            var rows = await PurchaseRows(query);

            var result = rows
                .Where(x => x.supplierId.HasValue)
                .GroupBy(x => new { x.supplierId, x.supplier, x.warehouseId, x.warehouseName })
                .Select(g => new
                {
                    supplierId = g.Key.supplierId,
                    supplierName = g.Key.supplier,
                    totalPurchases = g.Count(),
                    purchaseValue = g.Sum(x => x.totalAmount),
                    warehouseId = g.Key.warehouseId,
                    warehouseName = g.Key.warehouseName,
                    warehouse = g.Key.warehouseName
                })
                .OrderByDescending(x => x.purchaseValue);

            return Ok(result);
        }

        [HttpGet("customer-outstanding")]
        public async Task<IActionResult> CustomerOutstanding([FromQuery] ReportQuery query)
        {
            var rows = await SalesReportRows(query, false);

            var result = rows
                .Where(x => x.balanceAmount > 0)
                .Select(x => new
                {
                    id = x.invoiceId,
                    customerId = x.customerId,
                    customerName = x.customer,
                    invoiceNumber = x.invoiceNumber,
                    invoiceDate = x.invoiceDate,
                    dueDate = x.invoiceDate,
                    invoiceAmount = x.totalAmount,
                    paidAmount = x.paidAmount,
                    balanceAmount = x.balanceAmount,
                    warehouseId = x.warehouseId,
                    warehouseName = x.warehouseName,
                    warehouse = x.warehouseName,
                    agingStatus = GetAgingStatus(x.invoiceDate)
                });

            return Ok(result.OrderByDescending(x => x.balanceAmount).ToList());
        }

        [HttpGet("supplier-outstanding")]
        public async Task<IActionResult> SupplierOutstanding([FromQuery] ReportQuery query)
        {
            var purchases = await PurchaseRows(query);

            var payments = await _context.SupplierPayments.AsNoTracking()
                .Where(x => !x.IsCancelled && x.SupplierId.HasValue)
                .GroupBy(x => x.SupplierId!.Value)
                .Select(g => new
                {
                    supplierId = g.Key,
                    paid = g.Sum(x => x.Amount ?? 0m)
                })
                .ToDictionaryAsync(x => x.supplierId, x => x.paid);

            var result = purchases
                .GroupBy(x => new { x.supplierId, x.supplier, x.warehouseId, x.warehouseName })
                .Select(g =>
                {
                    var supplierId = g.Key.supplierId;
                    var total = g.Sum(x => x.totalAmount);
                    var paid = supplierId.HasValue && payments.TryGetValue(supplierId.Value, out var value) ? value : 0m;

                    return new
                    {
                        supplierId,
                        supplierName = g.Key.supplier,
                        totalPurchaseAmount = total,
                        paidAmount = paid,
                        outstandingAmount = total - paid,
                        warehouseId = g.Key.warehouseId,
                        warehouseName = g.Key.warehouseName,
                        warehouse = g.Key.warehouseName
                    };
                })
                .Where(x => x.outstandingAmount > 0)
                .OrderByDescending(x => x.outstandingAmount)
                .ToList();

            return Ok(result);
        }

        [HttpGet("inventory-valuation")]
        public async Task<IActionResult> InventoryValuation([FromQuery] ReportQuery query)
        {
            var stock = await GetStockRows(query);

            return Ok(stock.Select(x => new
            {
                id = x.stockId,
                productId = x.productId,
                productName = x.product,
                sku = x.sku,
                categoryId = x.categoryId,
                category = x.category,
                warehouseId = x.warehouseId,
                warehouseName = x.warehouse,
                warehouse = x.warehouse,
                quantityAvailable = x.AvailableQuantity,
                averageCost = x.CostPrice,
                totalStockValue = x.AvailableQuantity * x.CostPrice,
                lastPurchaseDate = (DateTime?)null
            }));
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock([FromQuery] ReportQuery query)
        {
            var stock = await GetStockRows(query);

            return Ok(stock.Select(x =>
            {
                var minimum = x.ReorderLevel;
                var status = x.AvailableQuantity <= minimum
                    ? "Critical"
                    : x.AvailableQuantity <= minimum + 5
                        ? "Warning"
                        : "Healthy";

                return new
                {
                    id = x.stockId,
                    productId = x.productId,
                    productName = x.product,
                    sku = x.sku,
                    categoryId = x.categoryId,
                    category = x.category,
                    availableStock = x.availableQuantity,
                    minimumStockLevel = minimum,
                    reorderQuantity = minimum * 2,
                    warehouseId = x.warehouseId,
                    warehouseName = x.warehouse,
                    warehouse = x.warehouse,
                    status
                };
            }));
        }

        [HttpGet("fast-moving")]
        public async Task<IActionResult> FastMoving([FromQuery] ReportQuery query)
        {
            var rows = await SalesReportRows(query, false);

            var result = rows
                .SelectMany(x => x.items.Select(item => new
                {
                    x.invoiceDate,
                    item.ProductId,
                    item.ProductName,
                    item.SKU,
                    item.Quantity,
                    item.Total,
                    x.warehouseId,
                    x.warehouseName
                }))
                .GroupBy(x => new
                {
                    x.ProductId,
                    x.ProductName,
                    x.SKU,
                    x.warehouseId,
                    x.warehouseName
                })
                .Select(g => new
                {
                    productId = g.Key.ProductId,
                    productName = g.Key.ProductName,
                    sku = g.Key.SKU,
                    unitsSold = g.Sum(x => x.Quantity),
                    salesValue = g.Sum(x => x.Total),
                    warehouseId = g.Key.warehouseId,
                    warehouseName = g.Key.warehouseName,
                    warehouse = g.Key.warehouseName,
                    movementStatus = g.Sum(x => x.Quantity) >= 10 ? "Fast" : "Watch"
                })
                .OrderByDescending(x => x.unitsSold);

            return Ok(result);
        }

        [HttpGet("slow-moving")]
        public async Task<IActionResult> SlowMoving([FromQuery] ReportQuery query)
        {
            var products = await _context.Products.AsNoTracking()
                .Where(x => !x.IsDeleted && !x.IsArchived)
                .Select(x => new
                {
                    x.ProductId,
                    x.Name,
                    x.SKU,
                    x.CategoryId,
                    x.Status,
                    x.CostPrice,
                    x.WarehouseId
                })
                .ToListAsync();

            var sales = await SalesReportRows(query, false);
            var lastSoldByProduct = sales
                .SelectMany(x => x.items.Select(item => new { item.ProductId, x.invoiceDate }))
                .Where(x => x.ProductId.HasValue && x.invoiceDate.HasValue)
                .GroupBy(x => x.ProductId!.Value)
                .ToDictionary(g => g.Key, g => g.Max(x => x.invoiceDate));

            var result = products.Select(product =>
            {
                lastSoldByProduct.TryGetValue(product.ProductId, out var lastSold);
                var days = lastSold.HasValue
                    ? (DateTime.Today - lastSold.Value.Date).Days
                    : 999;

                return new
                {
                    productId = product.ProductId,
                    productName = product.Name,
                    sku = product.SKU,
                    lastSoldDate = lastSold,
                    daysSinceLastSale = days,
                    stockAvailable = 0m,
                    stockValue = 0m,
                    warehouseId = product.WarehouseId,
                    warehouseName = "",
                    warehouse = "",
                    movementStatus = days > 90 ? "Slow" : "Watch",
                    status = product.Status
                };
            });

            return Ok(result.OrderByDescending(x => x.daysSinceLastSale).ToList());
        }

        [HttpGet("profitability")]
        public async Task<IActionResult> Profitability([FromQuery] ReportQuery query)
        {
            var rows = await SalesReportRows(query, false);

            var result = rows
                .SelectMany(x => x.items.Select(item => new
                {
                    item.ProductId,
                    item.ProductName,
                    item.SKU,
                    item.Quantity,
                    item.Total,
                    x.warehouseId,
                    x.warehouseName
                }))
                .GroupBy(x => new
                {
                    x.ProductId,
                    x.ProductName,
                    x.SKU,
                    x.warehouseId,
                    x.warehouseName
                })
                .Select(g =>
                {
                    var salesValue = g.Sum(x => x.Total);
                    var costPrice = _context.Products
                        .Where(p => p.ProductId == g.Key.ProductId)
                        .Select(p => p.CostPrice ?? 0m)
                        .FirstOrDefault();
                    var costValue = g.Sum(x => x.Quantity) * costPrice;
                    var profit = salesValue - costValue;

                    return new
                    {
                        productId = g.Key.ProductId,
                        productName = g.Key.ProductName,
                        sku = g.Key.SKU,
                        salesValue,
                        costValue,
                        grossProfit = profit,
                        profitMargin = salesValue == 0 ? 0 : (profit / salesValue) * 100,
                        warehouseId = g.Key.warehouseId,
                        warehouseName = g.Key.warehouseName,
                        warehouse = g.Key.warehouseName
                    };
                })
                .OrderByDescending(x => x.grossProfit)
                .ToList();

            return Ok(result);
        }

        [HttpGet("gst-tax")]
        public async Task<IActionResult> GstTax([FromQuery] ReportQuery query)
        {
            var sales = await SalesReportRows(query, false);
            var purchases = await PurchaseRows(query);

            var months = sales.Select(x => new
            {
                date = x.invoiceDate,
                total = x.totalAmount,
                tax = x.items.Sum(i => i.TaxAmount)
            })
                .Concat(purchases.Select(x => new
                {
                    date = x.orderDate,
                    total = x.totalAmount,
                    tax = 0m
                }))
                .Where(x => x.date.HasValue)
                .GroupBy(x => x.date!.Value.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    month = DateTime.ParseExact(g.Key + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMM yyyy"),
                    taxableSales = g.Where(x => x.tax > 0 || sales.Any()).Sum(x => x.total),
                    outputGst = g.Sum(x => x.tax),
                    taxablePurchases = g.Where(x => x.tax == 0).Sum(x => x.total),
                    inputGst = 0m,
                    netGstPayable = g.Sum(x => x.tax)
                })
                .ToList();

            return Ok(months);
        }

        [HttpGet("forecasting")]
        public async Task<IActionResult> Forecasting([FromQuery] ReportQuery query)
        {
            var fast = await FastMovingRows(query);
            var stock = await GetStockRows(query);

            var reorderCandidates = stock
                .Where(x => x.AvailableQuantity <= x.ReorderLevel)
                .ToList();

            var topProduct = fast.FirstOrDefault()?.ProductName;

            return Ok(new[]
            {
                new
                {
                    id = 1,
                    insight = "Expected Sales Trend",
                    prediction = !string.IsNullOrWhiteSpace(topProduct)
                        ? $"{topProduct} is expected to continue leading sales."
                        : "Sales trend will improve after more transactions.",
                    priority = "Medium",
                    status = "Watch"
                },
                new
                {
                    id = 2,
                    insight = "Expected Stock Shortage",
                    prediction = reorderCandidates.Count > 0
                        ? $"{reorderCandidates[0].Product} may face shortage soon."
                        : "No major shortage predicted.",
                    priority = reorderCandidates.Count > 0 ? "High" : "Low",
                    status = reorderCandidates.Count > 0 ? "Critical" : "Healthy"
                },
                new
                {
                    id = 3,
                    insight = "Reorder Prediction",
                    prediction = reorderCandidates.Count > 0
                        ? $"{reorderCandidates.Count} items need reorder planning."
                        : "Reorder levels are stable.",
                    priority = reorderCandidates.Count > 0 ? "High" : "Low",
                    status = reorderCandidates.Count > 0 ? "Action Needed" : "Healthy"
                },
                new
                {
                    id = 4,
                    insight = "Slow-moving Stock Warning",
                    prediction = "Review slow-moving items and plan discounts or supplier returns.",
                    priority = "Medium",
                    status = "Watch"
                }
            });
        }

        [HttpGet("warehouse-performance")]
        public async Task<IActionResult> WarehousePerformance([FromQuery] ReportQuery query)
        {
            var warehouses = await _context.Warehouses.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            if (HasWarehouseFilter(query))
            {
                var selected = await ResolveWarehouseIds(query);
                warehouses = warehouses.Where(x => selected.Contains(x.WarehouseId)).ToList();
            }

            var stock = await GetStockRows(query);
            var sales = await SalesReportRows(query, false);
            var purchases = await PurchaseRows(query);

            var result = warehouses.Select(w => new
            {
                id = w.WarehouseId,
                warehouseId = w.WarehouseId,
                warehouseName = w.Name,
                stockValue = stock.Where(x => x.WarehouseId == w.WarehouseId)
                     .Sum(x => x.Quantity * x.CostPrice),
                totalProducts = stock.Count(x => x.WarehouseId == w.WarehouseId),
                lowStockItems = stock.Count(x =>
                    x.WarehouseId == w.WarehouseId &&
                    x.AvailableQuantity <= x.ReorderLevel),
                damagedItems = 0,
                salesDispatches = sales.Count(x => x.WarehouseId == w.WarehouseId),
                purchaseReceipts = purchases.Count(x => x.WarehouseId == w.WarehouseId)
            }).ToList();

            return Ok(result);
        }

        // ============================================================
        // SUMMARY
        // ============================================================

        [HttpGet("summary")]
        public async Task<IActionResult> Summary([FromQuery] ReportQuery query)
        {
            var sales = await SalesReportRows(query, false);
            var purchases = await PurchaseRows(query);
            var stock = await GetStockRows(query);

            var totalSales = sales.Sum(x => x.totalAmount);
            var totalPurchases = purchases.Sum(x => x.totalAmount);
            var inventoryValue = stock.Sum(x => x.Quantity * x.CostPrice);
            var lowStock = stock.Count(x => x.AvailableQuantity <= x.ReorderLevel);
            var receivables = sales.Sum(x => x.balanceAmount);
            var topSelling = sales
                .SelectMany(x => x.items)
                .GroupBy(x => x.ProductName)
                .OrderByDescending(g => g.Sum(x => x.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault() ?? "No sales yet";

            return Ok(new
            {
                totalSales,
                totalPurchases,
                inventoryValue,
                lowStockItems = lowStock,
                receivables,
                payables = 0m,
                profit = totalSales - totalPurchases,
                topSellingItem = topSelling
            });
        }

        // ============================================================
        // EXPORTS - current frontend directly uses these four endpoints
        // ============================================================

        [HttpGet("export-sales")]
        public async Task<IActionResult> ExportSales([FromQuery] ReportQuery query)
        {
            var data = await SalesReportRows(query, false);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Sales Report");

            var headers = new[]
            {
                "Order No", "Customer", "Order Date", "Warehouse",
                "Total Amount", "Paid Amount", "Balance", "Status"
            };

            for (var i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            for (var i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                var item = data[i];

                sheet.Cell(row, 1).Value = item.invoiceNumber ?? "";
                sheet.Cell(row, 2).Value = item.customer ?? "";
                sheet.Cell(row, 3).Value = FormatDate(item.invoiceDate);
                sheet.Cell(row, 4).Value = item.warehouseName ?? "";
                sheet.Cell(row, 5).Value = item.totalAmount;
                sheet.Cell(row, 6).Value = item.paidAmount;
                sheet.Cell(row, 7).Value = item.balanceAmount;
                sheet.Cell(row, 8).Value = item.status ?? "";
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SalesReport.xlsx");
        }

        [HttpGet("export-stock")]
        public async Task<IActionResult> ExportStock([FromQuery] ReportQuery query)
        {
            var data = await GetStockRows(query);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Stock Report");

            var headers = new[]
            {
                "Product", "SKU", "Category", "Warehouse",
                "On Hand", "Reserved", "Available", "Price", "Cost Price", "Status"
            };

            for (var i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            for (var i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                var item = data[i];

                sheet.Cell(row, 1).Value = item.product ?? "";
                sheet.Cell(row, 2).Value = item.sku ?? "";
                sheet.Cell(row, 3).Value = item.category ?? "";
                sheet.Cell(row, 4).Value = item.warehouse ?? "";
                sheet.Cell(row, 5).Value = item.quantity;
                sheet.Cell(row, 6).Value = item.reservedQuantity;
                sheet.Cell(row, 7).Value = item.availableQuantity;
                sheet.Cell(row, 8).Value = item.price;
                sheet.Cell(row, 9).Value = item.costPrice;
                sheet.Cell(row, 10).Value = item.status ?? "";
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockReport.xlsx");
        }

        [HttpGet("export-sales-pdf")]
        public async Task<IActionResult> ExportSalesPdf([FromQuery] ReportQuery query)
        {
            var data = await SalesReportRows(query, false);

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, PageSize.A4.Rotate());

            document.SetMargins(28, 28, 28, 28);
            AddPdfTitle(document, "Sales Report");

            var table = new Table(new float[] { 1.4f, 1.8f, 1.2f, 1.5f, 1.2f, 1.2f, 1.2f, 1.1f })
                .UseAllAvailableWidth();

            AddPdfHeaders(table, new[]
            {
                "Order No", "Customer", "Date", "Warehouse",
                "Total", "Paid", "Balance", "Status"
            });

            foreach (var item in data)
            {
                AddPdfCell(table, item.invoiceNumber);
                AddPdfCell(table, item.customer);
                AddPdfCell(table, FormatDate(item.invoiceDate));
                AddPdfCell(table, item.warehouseName);
                AddPdfCell(table, FormatNumber(item.totalAmount));
                AddPdfCell(table, FormatNumber(item.paidAmount));
                AddPdfCell(table, FormatNumber(item.balanceAmount));
                AddPdfCell(table, item.status);
            }

            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "SalesReport.pdf");
        }

        [HttpGet("export-stock-pdf")]
        public async Task<IActionResult> ExportStockPdf([FromQuery] ReportQuery query)
        {
            var data = await GetStockRows(query);

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, PageSize.A4.Rotate());

            document.SetMargins(28, 28, 28, 28);
            AddPdfTitle(document, "Stock Report");

            var table = new Table(new float[] { 1.8f, 1.2f, 1.2f, 1.4f, 1f, 1f, 1f, 1f, 1f, 1f })
                .UseAllAvailableWidth();

            AddPdfHeaders(table, new[]
            {
                "Product", "SKU", "Category", "Warehouse",
                "On Hand", "Reserved", "Available", "Price", "Cost", "Status"
            });

            foreach (var item in data)
            {
                AddPdfCell(table, item.product);
                AddPdfCell(table, item.sku);
                AddPdfCell(table, item.category);
                AddPdfCell(table, item.warehouse);
                AddPdfCell(table, item.quantity.ToString(CultureInfo.InvariantCulture));
                AddPdfCell(table, item.reservedQuantity.ToString(CultureInfo.InvariantCulture));
                AddPdfCell(table, item.availableQuantity.ToString(CultureInfo.InvariantCulture));
                AddPdfCell(table, FormatNumber(item.price));
                AddPdfCell(table, FormatNumber(item.costPrice));
                AddPdfCell(table, item.status);
            }

            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "StockReport.pdf");
        }

        // ============================================================
        // INTERNAL DATA HELPERS
        // ============================================================

        private async Task<List<SalesReportRow>> SalesReportRows(
            ReportQuery query,
            bool includeCancelled)
        {
            var invoices = await (
                from invoice in _context.Invoices.AsNoTracking()
                join customer in _context.Customers.AsNoTracking()
                    on invoice.CustomerId equals customer.CustomerId into customerGroup
                from customer in customerGroup.DefaultIfEmpty()
                join warehouse in _context.Warehouses.AsNoTracking()
                    on invoice.WarehouseId equals warehouse.WarehouseId into warehouseGroup
                from warehouse in warehouseGroup.DefaultIfEmpty()
                where includeCancelled || !invoice.IsCancelled
                select new
                {
                    invoice.InvoiceId,
                    invoice.SoId,
                    invoice.CustomerId,
                    CustomerName = customer != null ? customer.Name : "",
                    invoice.InvoiceNumber,
                    invoice.InvoiceDate,
                    invoice.TotalAmount,
                    invoice.PaidAmount,
                    invoice.BalanceAmount,
                    invoice.Status,
                    InvoiceWarehouseId = invoice.WarehouseId,
                    InvoiceWarehouseName = warehouse != null ? warehouse.Name : ""
                }
            ).ToListAsync();

            var ids = invoices.Select(x => x.InvoiceId).ToList();

            // Resolve historical invoice warehouses from stock movements when
            // invoices.warehouse_id is NULL. This is required for invoices
            // whose stock was deducted from a specific warehouse.
            var movementWarehouses = ids.Count == 0
                ? new List<InvoiceMovementWarehouseRow>()
                : await (
                    from movement in _context.StockMovements.AsNoTracking()
                    join warehouse in _context.Warehouses.AsNoTracking()
                        on movement.WarehouseId equals warehouse.WarehouseId
                    where movement.ReferenceId.HasValue
                        && ids.Contains(movement.ReferenceId.Value)
                        && movement.ReferenceType != null
                        && movement.ReferenceType.ToLower() == "invoice"
                    select new InvoiceMovementWarehouseRow
                    {
                        InvoiceId = movement.ReferenceId!.Value,
                        ProductId = movement.ProductId,
                        WarehouseId = movement.WarehouseId,
                        WarehouseName = warehouse.Name,
                        MovementId = movement.MovementId
                    }
                )
                .OrderByDescending(x => x.MovementId)
                .ToListAsync();

            var movementWarehouseByInvoice = movementWarehouses
                .GroupBy(x => x.InvoiceId)
                .ToDictionary(g => g.Key, g => g.First());

            var itemRows = ids.Count == 0
                ? new List<SalesItemRow>()
                : await (
                    from item in _context.InvoiceItems.AsNoTracking()
                    join product in _context.Products.AsNoTracking()
                        on item.ProductId equals product.ProductId into productGroup
                    from product in productGroup.DefaultIfEmpty()
                    join category in _context.Categories.AsNoTracking()
                        on product.CategoryId equals category.CategoryId into categoryGroup
                    from category in categoryGroup.DefaultIfEmpty()
                    join warehouse in _context.Warehouses.AsNoTracking()
                        on product.WarehouseId equals warehouse.WarehouseId into warehouseGroup
                    from warehouse in warehouseGroup.DefaultIfEmpty()
                    where item.InvoiceId.HasValue && ids.Contains(item.InvoiceId.GetValueOrDefault())
                    select new SalesItemRow
                    {
                        InvoiceId = item.InvoiceId!.Value,
                        ProductId = item.ProductId,
                        ProductName = product != null ? product.Name : "",
                        SKU = product != null ? product.SKU : "",
                        CategoryId = product != null ? product.CategoryId : null,
                        CategoryName = category != null ? category.Name : "",
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Total = item.Total,
                        TaxAmount = item.TaxAmount,
                        WarehouseId = product != null ? product.WarehouseId : null,
                        WarehouseName = warehouse != null ? warehouse.Name : ""
                    }
                ).ToListAsync();

            // Prefer the warehouse recorded by the stock movement for each
            // invoice item. This keeps item-level warehouse data accurate
            // even when products are shared across warehouses.
            var movementWarehouseByInvoiceProduct = movementWarehouses
                .GroupBy(x => new { x.InvoiceId, x.ProductId })
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var item in itemRows)
            {
                if (item.ProductId.HasValue &&
                    movementWarehouseByInvoiceProduct.TryGetValue(
                        new { item.InvoiceId, ProductId = item.ProductId.Value },
                        out var movementWarehouse))
                {
                    item.WarehouseId = movementWarehouse.WarehouseId;
                    item.WarehouseName = movementWarehouse.WarehouseName;
                }
            }

            var byInvoice = itemRows
                .GroupBy(x => x.InvoiceId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var rows = invoices.Select(invoice =>
            {
                byInvoice.TryGetValue(invoice.InvoiceId, out var items);
                items ??= new List<SalesItemRow>();

                var fallback = items.FirstOrDefault(x => x.WarehouseId.HasValue);
                movementWarehouseByInvoice.TryGetValue(invoice.InvoiceId, out var movementWarehouse);

                return new SalesReportRow
                {
                    InvoiceId = invoice.InvoiceId,
                    SoId = invoice.SoId,
                    CustomerId = invoice.CustomerId,
                    Customer = invoice.CustomerName,
                    CustomerName = invoice.CustomerName,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    TotalAmount = invoice.TotalAmount,
                    PaidAmount = invoice.PaidAmount,
                    BalanceAmount = invoice.BalanceAmount,
                    Status = invoice.Status,
                    // Warehouse priority:
                    // 1. Invoice warehouse
                    // 2. Stock movement warehouse (historical invoice sales)
                    // 3. Product warehouse fallback
                    WarehouseId = invoice.InvoiceWarehouseId
                        ?? movementWarehouse?.WarehouseId
                        ?? fallback?.WarehouseId,
                    WarehouseName = !string.IsNullOrWhiteSpace(invoice.InvoiceWarehouseName)
                        ? invoice.InvoiceWarehouseName
                        : movementWarehouse?.WarehouseName
                            ?? fallback?.WarehouseName
                            ?? "",
                    Items = items
                };
            }).ToList();

            return ApplySalesFilters(rows, query).ToList();
        }

        private async Task<List<PurchaseReportRow>> PurchaseRows(ReportQuery query)
        {
            var purchases = await (
                from purchase in _context.PurchaseOrders.AsNoTracking()
                join supplier in _context.Suppliers.AsNoTracking()
                    on purchase.SupplierId equals supplier.SupplierId into supplierGroup
                from supplier in supplierGroup.DefaultIfEmpty()
                where !purchase.IsCancelled
                select new PurchaseReportRow
                {
                    PoId = purchase.PoId,
                    PoNumber = purchase.PoNumber,
                    SupplierId = purchase.SupplierId,
                    Supplier = supplier != null ? (supplier.Name ?? string.Empty) : "",
                    OrderDate = purchase.OrderDate,
                    TotalAmount = purchase.TotalAmount ?? 0m,
                    Status = purchase.Status
                }
            ).ToListAsync();

            var ids = purchases.Select(x => x.PoId).ToList();

            var grns = ids.Count == 0
                ? new List<PurchaseWarehouseRow>()
                : await _context.GoodsReceipts.AsNoTracking()
                    .Where(x => x.PoId.HasValue && ids.Contains(x.PoId.Value) && !x.IsCancelled)
                    .Join(
                        _context.Warehouses.AsNoTracking(),
                        g => g.WarehouseId,
                        w => (int?)w.WarehouseId,
                        (g, w) => new PurchaseWarehouseRow
                        {
                            PoId = g.PoId!.Value,
                            WarehouseId = g.WarehouseId,
                            WarehouseName = w.Name,
                            ReceiptDate = g.ReceiptDate
                        })
                    .ToListAsync();

            var products = ids.Count == 0
                ? new List<PurchaseProductRow>()
                : await (
                    from item in _context.PurchaseOrderItems.AsNoTracking()
                    join product in _context.Products.AsNoTracking()
                        on item.ProductId equals product.ProductId into productGroup
                    from product in productGroup.DefaultIfEmpty()
                    where item.PoId.HasValue && ids.Contains(item.PoId.Value)
                    select new PurchaseProductRow
                    {
                        PoId = item.PoId!.Value,
                        ProductId = item.ProductId,
                        ProductName = product != null ? product.Name : "",
                        WarehouseId = product != null ? product.WarehouseId : null
                    }
                ).ToListAsync();

            var grnByPo = grns
                .GroupBy(x => x.PoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.ReceiptDate ?? DateTime.MinValue).FirstOrDefault());

            var productByPo = products
                .GroupBy(x => x.PoId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var purchase in purchases)
            {
                grnByPo.TryGetValue(purchase.PoId, out var grn);
                productByPo.TryGetValue(purchase.PoId, out var productRows);

                purchase.WarehouseId = grn?.WarehouseId
                    ?? productRows?.FirstOrDefault(x => x.WarehouseId.HasValue)?.WarehouseId;

                var fallbackProductWarehouse = productRows?.FirstOrDefault(x => x.WarehouseId.HasValue);
                purchase.WarehouseName = grn?.WarehouseName ?? fallbackProductWarehouse?.WarehouseName ?? "";

                purchase.Items = productRows ?? new List<PurchaseProductRow>();
            }

            return ApplyPurchaseFilters(purchases, query).ToList();
        }

        private async Task<List<StockRow>> GetStockRows(ReportQuery query)
        {
            var data =
                from stock in _context.Stocks.AsNoTracking()
                join product in _context.Products.AsNoTracking()
                    on stock.ProductId equals product.ProductId
                join warehouse in _context.Warehouses.AsNoTracking()
                    on stock.WarehouseId equals warehouse.WarehouseId
                join category in _context.Categories.AsNoTracking()
                    on product.CategoryId equals category.CategoryId into categoryGroup
                from category in categoryGroup.DefaultIfEmpty()
                where !product.IsDeleted && !product.IsArchived
                select new StockRow
                {
                    StockId = stock.StockId,
                    ProductId = product.ProductId,
                    Product = product.Name,
                    SKU = product.SKU,
                    CategoryId = product.CategoryId,
                    Category = category != null ? category.Name : "",
                    WarehouseId = warehouse.WarehouseId,
                    Warehouse = warehouse.Name,
                    Quantity = stock.Quantity,
                    ReservedQuantity = stock.ReservedQuantity,
                    AvailableQuantity = stock.AvailableQuantity,
                    Price = product.Price ?? 0m,
                    CostPrice = product.CostPrice ?? 0m,
                    Status = product.Status,
                    ReorderLevel = product.ReorderLevel ?? 10
                };

            if (HasWarehouseFilter(query))
                data = ApplyWarehouseFilter(data, query);

            var categoryFilter = IsAll(query.CategoryId) ? query.Category : query.CategoryId;
            if (TryGetIntFilter(categoryFilter, out var categoryId))
                data = data.Where(x => x.CategoryId == categoryId);

            var productFilter = IsAll(query.ProductId) ? query.Product : query.ProductId;
            if (TryGetIntFilter(productFilter, out var productId))
                data = data.Where(x => x.ProductId == productId);

            if (!IsAll(query.Status))
                data = data.Where(x => x.Status == query.Status);

            return await data.OrderBy(x => x.Product).ThenBy(x => x.Warehouse).ToListAsync();
        }

        private async Task<List<FastMovingRow>> FastMovingRows(ReportQuery query)
        {
            var sales = await SalesReportRows(query, false);

            return sales
                .SelectMany(x => x.Items.Select(item => new
                {
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.Total
                }))
                .GroupBy(x => new { x.ProductId, x.ProductName })
                .Select(g => new FastMovingRow
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    UnitsSold = g.Sum(x => x.Quantity),
                    SalesValue = g.Sum(x => x.Total)
                })
                .OrderByDescending(x => x.UnitsSold)
                .ToList();
        }

        // ============================================================
        // FILTER HELPERS
        // ============================================================

        private IEnumerable<SalesReportRow> ApplySalesFilters(
            IEnumerable<SalesReportRow> rows,
            ReportQuery query)
        {
            if (query.FromDate.HasValue)
                rows = rows.Where(x => x.InvoiceDate.HasValue && x.InvoiceDate.Value >= query.FromDate.Value.Date);

            if (query.ToDate.HasValue)
            {
                var end = query.ToDate.Value.Date.AddDays(1);
                rows = rows.Where(x => x.InvoiceDate.HasValue && x.InvoiceDate.Value < end);
            }

            var customerFilter = IsAll(query.CustomerId) ? query.Customer : query.CustomerId;
            if (TryGetIntFilter(customerFilter, out var customerId))
                rows = rows.Where(x => x.CustomerId == customerId);

            if (!IsAll(query.Status))
                rows = rows.Where(x => string.Equals(x.Status, query.Status, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                rows = rows.Where(x =>
                    (x.InvoiceNumber ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (x.Customer ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (HasWarehouseFilter(query))
            {
                var allowedIds = ResolveWarehouseIdsSync(query);

                rows = rows.Where(x =>
                    (x.WarehouseId.HasValue && allowedIds.Contains(x.WarehouseId.Value)) ||
                    x.Items.Any(i => i.WarehouseId.HasValue && allowedIds.Contains(i.WarehouseId.Value)));
            }

            return rows;
        }

        private IEnumerable<PurchaseReportRow> ApplyPurchaseFilters(
            IEnumerable<PurchaseReportRow> rows,
            ReportQuery query)
        {
            if (query.FromDate.HasValue)
                rows = rows.Where(x => x.OrderDate.HasValue && x.OrderDate.Value >= query.FromDate.Value.Date);

            if (query.ToDate.HasValue)
            {
                var end = query.ToDate.Value.Date.AddDays(1);
                rows = rows.Where(x => x.OrderDate.HasValue && x.OrderDate.Value < end);
            }

            var supplierFilter = IsAll(query.SupplierId) ? query.Supplier : query.SupplierId;
            if (TryGetIntFilter(supplierFilter, out var supplierId))
                rows = rows.Where(x => x.SupplierId == supplierId);

            if (!IsAll(query.Status))
                rows = rows.Where(x => string.Equals(x.Status, query.Status, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                rows = rows.Where(x =>
                    (x.PoNumber ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (x.Supplier ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (HasWarehouseFilter(query))
            {
                var allowedIds = ResolveWarehouseIdsSync(query);
                rows = rows.Where(x =>
                    (x.WarehouseId.HasValue && allowedIds.Contains(x.WarehouseId.Value)) ||
                    x.Items.Any(i => i.WarehouseId.HasValue && allowedIds.Contains(i.WarehouseId.Value)));
            }

            return rows;
        }

        private static bool IsAll(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ||
                   value.Equals("all", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryGetIntFilter(string? value, out int result)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        private static bool HasWarehouseFilter(ReportQuery query)
        {
            return !IsAll(query.WarehouseId) || !IsAll(query.Warehouse);
        }

        private static IQueryable<StockRow> ApplyWarehouseFilter(
            IQueryable<StockRow> data,
            ReportQuery query)
        {
            if (TryGetIntFilter(query.WarehouseId, out var id))
                return data.Where(x => x.WarehouseId == id);

            if (TryGetIntFilter(query.Warehouse, out id))
                return data.Where(x => x.WarehouseId == id);

            var name = !IsAll(query.WarehouseId) ? query.WarehouseId : query.Warehouse;

            if (!IsAll(name))
            {
                name = name!.Trim();
                return data.Where(x => x.Warehouse.Contains(name));
            }

            return data;
        }

        private async Task<HashSet<int>> ResolveWarehouseIds(ReportQuery query)
        {
            var ids = new HashSet<int>();

            foreach (var value in new[] { query.WarehouseId, query.Warehouse })
            {
                if (IsAll(value))
                    continue;

                if (int.TryParse(value, out var id))
                    ids.Add(id);
                else
                {
                    var name = value!.Trim();
                    var matches = await _context.Warehouses.AsNoTracking()
                        .Where(x => !x.IsDeleted && x.Name.Contains(name))
                        .Select(x => x.WarehouseId)
                        .ToListAsync();

                    foreach (var match in matches)
                        ids.Add(match);
                }
            }

            return ids;
        }

        private HashSet<int> ResolveWarehouseIdsSync(ReportQuery query)
        {
            var ids = new HashSet<int>();

            foreach (var value in new[] { query.WarehouseId, query.Warehouse })
            {
                if (IsAll(value))
                    continue;

                if (int.TryParse(value, out var id))
                {
                    ids.Add(id);
                }
                else
                {
                    var matches = _context.Warehouses.AsNoTracking()
                        .Where(x => !x.IsDeleted && x.Name.Contains(value!.Trim()))
                        .Select(x => x.WarehouseId)
                        .ToList();

                    foreach (var match in matches)
                        ids.Add(match);
                }
            }

            return ids;
        }

        private static string GetAgingStatus(DateTime? date)
        {
            if (!date.HasValue)
                return "0-30 Days";

            var days = Math.Max(0, (DateTime.Today - date.Value.Date).Days);

            if (days <= 30) return "0-30 Days";
            if (days <= 60) return "31-60 Days";
            if (days <= 90) return "61-90 Days";
            return "90+ Days";
        }

        private static string FormatDate(DateTime? date)
            => date?.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) ?? "";

        private static string FormatNumber(decimal value)
            => value.ToString("N2", CultureInfo.InvariantCulture);

        private static void AddPdfTitle(Document document, string title)
        {
            document.Add(new Paragraph("IMS Reports")
                .SetFontSize(10)
                .SetFontColor(ColorConstants.DARK_GRAY));

            document.Add(new Paragraph(title)
                .SetFontSize(20)
                .SetFontColor(new DeviceRgb(15, 23, 42)));

            document.Add(new Paragraph(
                $"Generated {DateTime.Now:dd MMM yyyy, hh:mm tt}")
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY));

            document.Add(new Paragraph(" "));
        }

        private static void AddPdfHeaders(Table table, string[] headers)
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

        private static void AddPdfCell(Table table, string? value)
        {
            table.AddCell(new Cell()
                .SetFontSize(8.5f)
                .SetFontColor(new DeviceRgb(30, 41, 59))
                .Add(new Paragraph(value ?? "")));
        }

        // ============================================================
        // INTERNAL TYPES
        // ============================================================

        public class ReportQuery
        {
            // Names match the current frontend query object exactly.
            public string? From { get; set; }
            public string? To { get; set; }

            // Supports both the current frontend "warehouse" parameter
            // and the requested warehouseId parameter.
            public string? Warehouse { get; set; }
            public string? WarehouseId { get; set; }

            public string? Category { get; set; }
            public string? CategoryId { get; set; }

            public string? Product { get; set; }
            public string? ProductId { get; set; }

            public string? Customer { get; set; }
            public string? CustomerId { get; set; }

            public string? Supplier { get; set; }
            public string? SupplierId { get; set; }

            public string? ReportType { get; set; }
            public string? Status { get; set; }
            public string? Search { get; set; }

            // Convenience parsed dates. ASP.NET binds the string fields
            // sent by the frontend and these expose them as DateTime values.
            public DateTime? FromDate => ParseDate(From);
            public DateTime? ToDate => ParseDate(To);

            private static DateTime? ParseDate(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                return DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out var date)
                    ? date
                    : null;
            }
        }

        private sealed class SalesItemRow
        {
            public int InvoiceId { get; set; }
            public int? ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public string SKU { get; set; } = "";
            public int? CategoryId { get; set; }
            public string CategoryName { get; set; } = "";
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total { get; set; }
            public decimal TaxAmount { get; set; }
            public int? WarehouseId { get; set; }
            public string WarehouseName { get; set; } = "";
        }

        private sealed class InvoiceMovementWarehouseRow
        {
            public int InvoiceId { get; set; }
            public int ProductId { get; set; }
            public int WarehouseId { get; set; }
            public string WarehouseName { get; set; } = "";
            public int MovementId { get; set; }
        }

        private sealed class SalesReportRow
        {
            public int InvoiceId { get; set; }
            public int invoiceId => InvoiceId;
            public int? SoId { get; set; }
            public int? CustomerId { get; set; }
            public string Customer { get; set; } = "";
            public string CustomerName { get; set; } = "";
            public string? InvoiceNumber { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal BalanceAmount { get; set; }
            public string? Status { get; set; }
            public int? WarehouseId { get; set; }
            public string WarehouseName { get; set; } = "";
            public string warehouse => WarehouseName;
            public List<SalesItemRow> Items { get; set; } = new();

            // Internal aliases used by the projection/report helpers.
            public int InvoiceIdAlias => InvoiceId;
            public int? customerId => CustomerId;
            public string customer => Customer;
            public string customerName => CustomerName;
            public string? invoiceNumber => InvoiceNumber;
            public DateTime? invoiceDate => InvoiceDate;
            public decimal totalAmount => TotalAmount;
            public decimal paidAmount => PaidAmount;
            public decimal balanceAmount => BalanceAmount;
            public string? status => Status;
            public int? warehouseId => WarehouseId;
            public string warehouseName => WarehouseName;
            public List<SalesItemRow> items => Items;
        }

        private sealed class PurchaseBaseRow
        {
            public int PoId { get; set; }
            public string? PoNumber { get; set; }
            public int? SupplierId { get; set; }
            public string SupplierName { get; set; } = "";
            public DateTime? OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string? Status { get; set; }
        }

        private sealed class PurchaseWarehouseRow
        {
            public int PoId { get; set; }
            public int? WarehouseId { get; set; }
            public string WarehouseName { get; set; } = "";
            public DateTime? ReceiptDate { get; set; }
        }

        private sealed class PurchaseProductRow
        {
            public int PoId { get; set; }
            public int? ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int? WarehouseId { get; set; }
            public string WarehouseName { get; set; } = "";
        }

        private sealed class PurchaseReportRow
        {
            public int PoId { get; set; }
            public string? PoNumber { get; set; }
            public int? SupplierId { get; set; }
            public string Supplier { get; set; } = "";
            public DateTime? OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string? Status { get; set; }
            public int? WarehouseId { get; set; }
            public string WarehouseName { get; set; } = "";
            public List<PurchaseProductRow> Items { get; set; } = new();

            public int? supplierId => SupplierId;
            public string supplier => Supplier;
            public DateTime? orderDate => OrderDate;
            public decimal totalAmount => TotalAmount;
            public string? status => Status;
            public int? warehouseId => WarehouseId;
            public string warehouseName => WarehouseName;
            public string? poNumber => PoNumber;
            public List<PurchaseProductRow> items => Items;
        }

        private sealed class FastMovingRow
        {
            public int? ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public decimal UnitsSold { get; set; }
            public decimal SalesValue { get; set; }
        }

        private sealed class StockRow
        {
            public int StockId { get; set; }
            public int ProductId { get; set; }
            public string Product { get; set; } = "";
            public string SKU { get; set; } = "";
            public int? CategoryId { get; set; }
            public string Category { get; set; } = "";
            public int WarehouseId { get; set; }
            public string Warehouse { get; set; } = "";
            public decimal Quantity { get; set; }
            public decimal ReservedQuantity { get; set; }
            public decimal AvailableQuantity { get; set; }
            public decimal Price { get; set; }
            public decimal CostPrice { get; set; }
            public string? Status { get; set; }
            public int ReorderLevel { get; set; }

            public int stockId => StockId;
            public int productId => ProductId;
            public string product => Product;
            public string sku => SKU;
            public int? categoryId => CategoryId;
            public string category => Category;
            public int warehouseId => WarehouseId;
            public string warehouse => Warehouse;
            public decimal quantity => Quantity;
            public decimal reservedQuantity => ReservedQuantity;
            public decimal availableQuantity => AvailableQuantity;
            public decimal price => Price;
            public decimal costPrice => CostPrice;
            public string? status => Status;
            public int reorderLevel => ReorderLevel;
        }
    }
}
