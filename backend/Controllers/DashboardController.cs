using IMSBackend.Data;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly InventoryHealthService _inventoryHealthService;

        public DashboardController(AppDbContext context, InventoryHealthService inventoryHealthService)
        {
            _context = context;
            _inventoryHealthService = inventoryHealthService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            var totalProducts =
                await _context.Products.AsNoTracking().CountAsync(x => !x.IsDeleted, cancellationToken);

            var totalCustomers =
                await _context.Customers.AsNoTracking().CountAsync(cancellationToken);

            var totalSuppliers =
                await _context.Suppliers.AsNoTracking().CountAsync(cancellationToken);

            var totalSales =
                await _context.Invoices.AsNoTracking()
                    .Where(x => !x.IsCancelled)
                    .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0;

            var totalPurchases =
                await _context.PurchaseOrders.AsNoTracking()
                    .Where(x => !x.IsCancelled)
                    .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0;

            var inventoryHealth =
                await _inventoryHealthService.GetInventoryHealthStatus(cancellationToken);


            return Ok(new
            {
                totalProducts,
                totalCustomers,
                totalSuppliers,
                totalSales,
                totalPurchases,

                lowStockProducts = inventoryHealth.LowStockCount,
                outOfStockProducts = inventoryHealth.OutOfStockCount,
                inventoryHealthStatus = inventoryHealth.Status,
                inventoryHealthTone = inventoryHealth.Tone,
                inventoryHealthMessage = inventoryHealth.Message
            });
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts(CancellationToken cancellationToken)
        {
            var lowStockProducts = await _inventoryHealthService.GetLowStockProducts(cancellationToken);

            return Ok(lowStockProducts.Select(product => new
            {
                StockId = product.ProductId,
                product.ProductId,
                product.Name,
                product.SKU,
                Quantity = product.CurrentStock,
                CurrentStock = product.CurrentStock,
                product.ReorderLevel,
                ReservedQuantity = 0,
                AvailableQuantity = product.CurrentStock,
                product.Status
            }));
        }

        [HttpGet("recent-sales")]
        public async Task<IActionResult> GetRecentSales(CancellationToken cancellationToken)
        {
            var recentSales = await (
                from invoice in _context.Invoices
                join customer in _context.Customers
                    on invoice.CustomerId equals customer.CustomerId
                where !invoice.IsCancelled
                orderby invoice.InvoiceDate descending
                select new
                {
                    invoice.InvoiceId,
                    invoice.InvoiceNumber,
                    invoice.InvoiceDate,
                    CustomerName = customer.Name,
                    invoice.TotalAmount,
                    invoice.PaidAmount,
                    invoice.BalanceAmount,
                    invoice.Status
                })
                .Take(10)
                .ToListAsync(cancellationToken);

            return Ok(recentSales);
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopSellingProducts(CancellationToken cancellationToken)
        {
            var topProducts = await (
                from item in _context.InvoiceItems
                join product in _context.Products
                    on item.ProductId equals product.ProductId
                where !product.IsDeleted
                group item by new
                {
                    product.ProductId,
                    product.Name,
                    product.SKU
                }
                into grouped
                orderby grouped.Sum(x => x.Total) descending
                select new
                {
                    grouped.Key.ProductId,
                    grouped.Key.Name,
                    grouped.Key.SKU,
                    TotalSold = grouped.Sum(x => x.Quantity),
                    Revenue = grouped.Sum(x => x.Total)
                })
                .Take(10)
                .ToListAsync(cancellationToken);

            return Ok(topProducts);
        }

        [HttpGet("monthly-sales")]
        public async Task<IActionResult> GetMonthlySales(CancellationToken cancellationToken)
        {
            var rawSales = await (
                from invoice in _context.Invoices.AsNoTracking()
                where invoice.InvoiceDate != null &&
                      !invoice.IsCancelled
                group invoice by new
                {
                    invoice.InvoiceDate!.Value.Year,
                    invoice.InvoiceDate!.Value.Month
                }
                into grouped
                orderby grouped.Key.Year descending, grouped.Key.Month descending
                select new
                {
                    Year = grouped.Key.Year,
                    Month = grouped.Key.Month,
                    TotalSales = grouped.Sum(x => x.TotalAmount),
                    TotalInvoices = grouped.Count()
                })
                .Take(12)
                .ToListAsync(cancellationToken);

            var monthlySales = rawSales
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new
                {
                    year = x.Year,
                    month = x.Month,
                    monthLabel = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                    totalSales = x.TotalSales,
                    totalInvoices = x.TotalInvoices
                })
                .ToList();

            return Ok(new
            {
                success = true,
                monthlySales,
                data = monthlySales,
                message = (string?)null
            });
        }

        [HttpGet("monthly-purchases")]
        public async Task<IActionResult> GetMonthlyPurchases(CancellationToken cancellationToken)
        {
            var rawPurchases = await (
                from po in _context.PurchaseOrders.AsNoTracking()
                where po.OrderDate != null &&
                      !po.IsCancelled
                group po by new
                {
                    po.OrderDate!.Value.Year,
                    po.OrderDate!.Value.Month
                }
                into grouped
                orderby grouped.Key.Year descending, grouped.Key.Month descending
                select new
                {
                    Year = grouped.Key.Year,
                    Month = grouped.Key.Month,
                    TotalPurchases = grouped.Sum(x => x.TotalAmount),
                    TotalOrders = grouped.Count()
                })
                .Take(12)
                .ToListAsync(cancellationToken);

            var monthlyPurchases = rawPurchases
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new
                {
                    year = x.Year,
                    month = x.Month,
                    monthLabel = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                    totalPurchases = x.TotalPurchases,
                    totalOrders = x.TotalOrders
                })
                .ToList();

            return Ok(monthlyPurchases);
        }

        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetRecentActivities(CancellationToken cancellationToken)
        {
            var auditLogs = await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .ToListAsync(cancellationToken);

            var userIds = auditLogs
                .Where(x => x.UserId.HasValue)
                .Select(x => x.UserId!.Value)
                .Distinct()
                .ToList();

            var userNames = await _context.Users
                .AsNoTracking()
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Name ?? x.Email, cancellationToken);

            var activities = auditLogs.Select(x => new
            {
                id = x.LogId,
                date = DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc),
                userName = x.UserId.HasValue && userNames.TryGetValue(x.UserId.Value, out var userName)
                    ? userName
                    : null,
                type = x.Action ?? x.Module ?? "ACTIVITY",
                module = x.Module,
                description = x.Description ?? x.Action ?? "Activity recorded",
                recordId = x.RecordId,
                tableName = x.TableName
            });

            return Ok(activities);
        }
    }
}
