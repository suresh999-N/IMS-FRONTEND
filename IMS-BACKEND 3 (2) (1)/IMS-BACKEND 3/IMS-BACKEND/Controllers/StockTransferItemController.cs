using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-transfer-items")]
    public class StockTransferItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockTransferItemController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.StockTransferItems.ToList());
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.StockTransferItems.Find(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(StockTransferItemDto dto)
        {
            // Check Transfer
            var transfer = _context.StockTransfers.Find(dto.TransferId);

            if (transfer == null)
                return BadRequest("Invalid TransferId");

            // Check Product
            var product = _context.Products.FirstOrDefault(item =>
                item.ProductId == dto.ProductId &&
                !item.IsDeleted);

            if (product == null)
                return BadRequest("Invalid ProductId");

            var item = new StockTransferItem
            {
                TransferId = dto.TransferId,
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                Quantity = dto.Quantity
            };

            _context.StockTransferItems.Add(item);

            var fromStock = _context.Stocks.FirstOrDefault(row =>
                row.ProductId == dto.ProductId &&
                row.VariantId == dto.VariantId &&
                row.WarehouseId == transfer.FromWarehouseId);

            if (fromStock == null || fromStock.Quantity < dto.Quantity)
            {
                return BadRequest("Insufficient stock in source warehouse for transfer.");
            }

            var toStock = _context.Stocks.FirstOrDefault(row =>
                row.ProductId == dto.ProductId &&
                row.VariantId == dto.VariantId &&
                row.WarehouseId == transfer.ToWarehouseId);

            if (toStock == null)
            {
                toStock = new Stock
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = transfer.ToWarehouseId,
                    Quantity = 0,
                    ReservedQuantity = 0
                };
                _context.Stocks.Add(toStock);
            }

            var now = DateTime.UtcNow;
            var fromOpeningQty = fromStock.Quantity;
            var toOpeningQty = toStock.Quantity;

            fromStock.Quantity -= dto.Quantity;
            toStock.Quantity += dto.Quantity;
            product.UpdatedAt = now;

            _context.StockMovements.AddRange(
                new StockMovement
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = transfer.FromWarehouseId,
                    MovementType = "TRANSFER",
                    Quantity = dto.Quantity,
                    ReferenceId = transfer.TransferId,
                    ReferenceType = "transfer_out",
                    Notes = "Stock transfer out",
                    CreatedAt = now
                },
                new StockMovement
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = transfer.ToWarehouseId,
                    MovementType = "TRANSFER",
                    Quantity = dto.Quantity,
                    ReferenceId = transfer.TransferId,
                    ReferenceType = "transfer_in",
                    Notes = "Stock transfer in",
                    CreatedAt = now
                });

            _context.StockLedgers.AddRange(
                new StockLedger
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = transfer.FromWarehouseId,
                    OpeningQty = fromOpeningQty,
                    ChangeQty = -dto.Quantity,
                    ClosingQty = fromStock.Quantity,
                    TransactionType = "TRANSFER_OUT",
                    TransactionId = transfer.TransferId,
                    CreatedAt = now
                },
                new StockLedger
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = transfer.ToWarehouseId,
                    OpeningQty = toOpeningQty,
                    ChangeQty = dto.Quantity,
                    ClosingQty = toStock.Quantity,
                    TransactionType = "TRANSFER_IN",
                    TransactionId = transfer.TransferId,
                    CreatedAt = now
                });

            _context.SaveChanges();

            return Ok(item);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, StockTransferItemDto dto)
        {
            var item = _context.StockTransferItems.Find(id);

            if (item == null)
                return NotFound();

            item.TransferId = dto.TransferId;
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
            var item = _context.StockTransferItems.Find(id);

            if (item == null)
                return NotFound();

            _context.StockTransferItems.Remove(item);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}
