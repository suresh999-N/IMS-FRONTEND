using IMSBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Services;

public sealed class LowStockProductDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public decimal CurrentStock { get; set; }

    public int ReorderLevel { get; set; }

    public string Status { get; set; } = "Low Stock";
}

public sealed class InventoryHealthStatusDto
{
    public int LowStockCount { get; set; }

    public int OutOfStockCount { get; set; }

    public string Status { get; set; } = "Healthy";

    public string Tone { get; set; } = "success";

    public string Message { get; set; } = "Operations Healthy";
}

public sealed class InventoryHealthService
{
    private readonly AppDbContext _context;

    public InventoryHealthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LowStockProductDto>> GetLowStockProducts(CancellationToken cancellationToken = default)
    {
        var rows = await GetInventoryRows()
            .Where(item => item.CurrentStock <= item.ReorderLevel)
            .OrderBy(item => item.CurrentStock)
            .ThenBy(item => item.Name)
            .ToListAsync(cancellationToken);

        return rows
            .Select(item => new LowStockProductDto
            {
                ProductId = item.ProductId,
                Name = item.Name,
                SKU = item.SKU,
                CurrentStock = item.CurrentStock,
                ReorderLevel = item.ReorderLevel,
                Status = item.CurrentStock <= 0 ? "Critical" : "Low Stock"
            })
            .ToList();
    }

    public async Task<int> GetLowStockCount(CancellationToken cancellationToken = default)
    {
        return await GetInventoryRows()
            .CountAsync(item => item.CurrentStock <= item.ReorderLevel, cancellationToken);
    }

    public async Task<InventoryHealthStatusDto> GetInventoryHealthStatus(CancellationToken cancellationToken = default)
    {
        var lowStockProducts = await GetLowStockProducts(cancellationToken);
        var outOfStockCount = lowStockProducts.Count(item => item.CurrentStock <= 0);

        if (lowStockProducts.Count == 0)
        {
            return new InventoryHealthStatusDto
            {
                LowStockCount = 0,
                OutOfStockCount = 0,
                Status = "Healthy",
                Tone = "success",
                Message = "Operations Healthy"
            };
        }

        return new InventoryHealthStatusDto
        {
            LowStockCount = lowStockProducts.Count,
            OutOfStockCount = outOfStockCount,
            Status = outOfStockCount > 0 ? "Critical" : "Attention",
            Tone = "danger",
            Message = "Inventory Attention Required"
        };
    }

    private IQueryable<InventoryHealthRow> GetInventoryRows()
    {
        return
            from product in _context.Products.AsNoTracking()
            where !product.IsDeleted &&
                  product.Status != null &&
                  product.Status.ToLower() == "active"
            join stock in _context.Stocks.AsNoTracking()
                on product.ProductId equals stock.ProductId into stockGroup
            select new InventoryHealthRow
            {
                ProductId = product.ProductId,
                Name = product.Name,
                SKU = product.SKU,
                CurrentStock = stockGroup.Sum(stock => (decimal?)stock.Quantity) ?? 0,
                ReorderLevel = product.ReorderLevel ?? 0
            };
    }

    private sealed class InventoryHealthRow
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal CurrentStock { get; set; }

        public int ReorderLevel { get; set; }
    }
}
