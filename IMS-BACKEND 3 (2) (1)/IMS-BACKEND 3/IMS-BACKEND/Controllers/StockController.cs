using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public StockController(AppDbContext context, AuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var stocks = (
                from stock in _context.Stocks.AsNoTracking()
                join product in _context.Products.AsNoTracking()
                    on stock.ProductId equals product.ProductId into productRows
                from product in productRows.DefaultIfEmpty()
                join warehouse in _context.Warehouses.AsNoTracking()
                    on stock.WarehouseId equals warehouse.WarehouseId into warehouseRows
                from warehouse in warehouseRows.DefaultIfEmpty()
                join variant in _context.ProductVariants.AsNoTracking()
                    on stock.VariantId equals variant.VariantId into variantRows
                from variant in variantRows.DefaultIfEmpty()
                select new
                {
                    stock.StockId,
                    stock.ProductId,
                    stock.VariantId,
                    stock.WarehouseId,
                    ProductName = product == null || product.Name == "" ? "Unknown Product" : product.Name,
                    product_name = product == null || product.Name == "" ? "Unknown Product" : product.Name,
                    WarehouseName = warehouse == null || warehouse.Name == "" ? "Unknown Warehouse" : warehouse.Name,
                    warehouse_name = warehouse == null || warehouse.Name == "" ? "Unknown Warehouse" : warehouse.Name,
                    VariantName = variant == null || variant.VariantName == "" ? "Standard" : variant.VariantName,
                    variant_name = variant == null || variant.VariantName == "" ? "Standard" : variant.VariantName,
                    stock.Quantity,
                    stock.ReservedQuantity,
                    stock.AvailableQuantity
                }).OrderByDescending(x => x.StockId).ToList();

            return Ok(stocks);
        }
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var stock = _context.Stocks.Find(id);

            if (stock == null)
                return NotFound();

            return Ok(stock);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(StockDto dto)
        {
            // Check Product
            var product = await _context.Products.FirstOrDefaultAsync(item =>
                item.ProductId == dto.ProductId &&
                !item.IsDeleted);

            if (product == null)
                return BadRequest("Invalid ProductId");

            // Check Warehouse
            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var stock = new Stock
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                WarehouseId = dto.WarehouseId,
                Quantity = dto.Quantity,
                ReservedQuantity = dto.ReservedQuantity
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                dto.Quantity >= 0 ? "STOCK_ADDED" : "STOCK_REMOVED",
                "Stock",
                stock.StockId,
                dto.Quantity >= 0
                    ? $"Stock added for {product.Name}"
                    : $"Stock removed for {product.Name}",
                "stocks");

            return Ok(stock);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StockDto dto)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
                return NotFound();

            var previousQuantity = stock.Quantity;

            // Check Product
            var product = await _context.Products.FirstOrDefaultAsync(item =>
                item.ProductId == dto.ProductId &&
                !item.IsDeleted);

            if (product == null)
                return BadRequest("Invalid ProductId");

            // Check Warehouse
            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            stock.ProductId = dto.ProductId;
            stock.VariantId = dto.VariantId;
            stock.WarehouseId = dto.WarehouseId;
            stock.Quantity = dto.Quantity;
            stock.ReservedQuantity = dto.ReservedQuantity;

            await _context.SaveChangesAsync();

            var change = dto.Quantity - previousQuantity;
            await _auditLogService.LogAsync(
                change >= 0 ? "STOCK_ADDED" : "STOCK_REMOVED",
                "Stock",
                stock.StockId,
                change >= 0
                    ? $"Stock added for {product.Name}"
                    : $"Stock removed for {product.Name}",
                "stocks");

            return Ok(stock);
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var stock = _context.Stocks.Find(id);

            if (stock == null)
                return NotFound();

            _context.Stocks.Remove(stock);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}
