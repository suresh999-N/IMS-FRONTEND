using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-adjustment-items")]
    public class StockAdjustmentItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockAdjustmentItemController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.StockAdjustmentItems.ToList());
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.StockAdjustmentItems.Find(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpGet("adjustment/{adjustmentId}")]
        public IActionResult GetByAdjustmentId(int adjustmentId)
        {
            var adjustmentExists = _context.StockAdjustments
                .Any(a => a.AdjustmentId == adjustmentId);

            if (!adjustmentExists)
                return NotFound("Stock adjustment not found.");

            var items = _context.StockAdjustmentItems
                .Where(i => i.AdjustmentId == adjustmentId)
                .ToList();

            return Ok(items);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(StockAdjustmentItemDto dto)
        {
            // Check Adjustment
            var adjustment = _context.StockAdjustments.Find(dto.AdjustmentId);

            if (adjustment == null)
                return BadRequest("Invalid AdjustmentId");

            // Check Product
            var product = _context.Products.FirstOrDefault(item =>
                item.ProductId == dto.ProductId &&
                !item.IsDeleted);

            if (product == null)
                return BadRequest("Invalid ProductId");

            var item = new StockAdjustmentItem
            {
                AdjustmentId = dto.AdjustmentId,
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                Quantity = dto.Quantity
            };

            _context.StockAdjustmentItems.Add(item);

            var adjustmentType = adjustment.AdjustmentType?.Trim().ToLowerInvariant();
            var isIncrease = adjustmentType == "increase" || adjustmentType == "adjustment_in";
            var isDecrease = adjustmentType == "decrease" || adjustmentType == "adjustment_out";

            if (!isIncrease && !isDecrease)
            {
                return BadRequest("Adjustment type must be increase or decrease.");
            }

            var stock = _context.Stocks.FirstOrDefault(row =>
                row.ProductId == dto.ProductId &&
                row.VariantId == dto.VariantId &&
                row.WarehouseId == adjustment.WarehouseId);

            if (stock == null)
            {
                if (isDecrease)
                {
                    return BadRequest("Insufficient stock for adjustment decrease.");
                }

                stock = new Stock
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = adjustment.WarehouseId,
                    Quantity = 0,
                    ReservedQuantity = 0
                };
                _context.Stocks.Add(stock);
            }

            var openingQty = stock.Quantity;
            var changeQty = isIncrease ? dto.Quantity : -dto.Quantity;
            var closingQty = openingQty + changeQty;

            if (isDecrease && dto.Quantity > stock.Quantity)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Insufficient stock. Available stock is {stock.Quantity}, but requested quantity is {dto.Quantity}."
                });
            }

            stock.Quantity = closingQty;
            product.UpdatedAt = DateTime.UtcNow;

            _context.StockMovements.Add(new StockMovement
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                WarehouseId = adjustment.WarehouseId,
                MovementType = "ADJUSTMENT",
                Quantity = dto.Quantity,
                ReferenceId = adjustment.AdjustmentId,
                ReferenceType = isIncrease ? "adjustment_in" : "adjustment_out",
                Notes = adjustment.Reason,
                CreatedAt = DateTime.UtcNow
            });

            _context.StockLedgers.Add(new StockLedger
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                WarehouseId = adjustment.WarehouseId,
                OpeningQty = openingQty,
                ChangeQty = changeQty,
                ClosingQty = closingQty,
                TransactionType = isIncrease ? "ADJUSTMENT_IN" : "ADJUSTMENT_OUT",
                TransactionId = adjustment.AdjustmentId,
                CreatedAt = DateTime.UtcNow
            });

            _context.SaveChanges();

            return Ok(item);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, StockAdjustmentItemDto dto)
        {
            var item = _context.StockAdjustmentItems.Find(id);

            if (item == null)
                return NotFound();

            item.AdjustmentId = dto.AdjustmentId;
            item.ProductId = dto.ProductId;
            item.VariantId = dto.VariantId;
            item.Quantity = dto.Quantity;

            _context.SaveChanges();

            return Ok(item);
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.StockAdjustmentItems.Find(id);

            if (item == null)
                return NotFound();

            _context.StockAdjustmentItems.Remove(item);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}
