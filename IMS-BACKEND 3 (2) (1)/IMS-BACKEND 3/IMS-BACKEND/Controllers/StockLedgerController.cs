using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-ledger")]
    public class StockLedgerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockLedgerController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var ledgers = (
                from ledger in _context.StockLedgers.AsNoTracking()
                join product in _context.Products.AsNoTracking()
                    on ledger.ProductId equals product.ProductId into productRows
                from product in productRows.DefaultIfEmpty()
                join warehouse in _context.Warehouses.AsNoTracking()
                    on ledger.WarehouseId equals warehouse.WarehouseId into warehouseRows
                from warehouse in warehouseRows.DefaultIfEmpty()
                join variant in _context.ProductVariants.AsNoTracking()
                    on ledger.VariantId equals variant.VariantId into variantRows
                from variant in variantRows.DefaultIfEmpty()
                select new
                {
                    ledger.LedgerId,
                    ledger.ProductId,
                    ledger.VariantId,
                    ledger.WarehouseId,
                    ProductName = product == null || product.Name == "" ? "Unknown Product" : product.Name,
                    product_name = product == null || product.Name == "" ? "Unknown Product" : product.Name,
                    WarehouseName = warehouse == null || warehouse.Name == "" ? "Unknown Warehouse" : warehouse.Name,
                    warehouse_name = warehouse == null || warehouse.Name == "" ? "Unknown Warehouse" : warehouse.Name,
                    VariantName = variant == null || variant.VariantName == "" ? "Standard" : variant.VariantName,
                    variant_name = variant == null || variant.VariantName == "" ? "Standard" : variant.VariantName,
                    ledger.OpeningQty,
                    ledger.ChangeQty,
                    ledger.ClosingQty,
                    ledger.TransactionType,
                    ledger.TransactionId,
                    TransactionDisplay = ledger.TransactionType == null || ledger.TransactionType == ""
                        ? "Not set"
                        : ledger.TransactionType,
                    ledger.CreatedAt
                }).OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.LedgerId).ToList();

            return Ok(ledgers);
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ledger = _context.StockLedgers.Find(id);

            if (ledger == null)
                return NotFound();

            return Ok(ledger);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(StockLedgerDto dto)
        {
            var ledger = new StockLedger
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                WarehouseId = dto.WarehouseId,
                OpeningQty = dto.OpeningQty,
                ChangeQty = dto.ChangeQty,
                ClosingQty = dto.ClosingQty,
                TransactionType = dto.TransactionType,
                TransactionId = dto.TransactionId,
                CreatedAt = DateTime.Now
            };

            _context.StockLedgers.Add(ledger);
            _context.SaveChanges();

            return Ok(ledger);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, StockLedgerDto dto)
        {
            var ledger = _context.StockLedgers.Find(id);

            if (ledger == null)
                return NotFound();

            ledger.ProductId = dto.ProductId;
            ledger.VariantId = dto.VariantId;
            ledger.WarehouseId = dto.WarehouseId;
            ledger.OpeningQty = dto.OpeningQty;
            ledger.ChangeQty = dto.ChangeQty;
            ledger.ClosingQty = dto.ClosingQty;
            ledger.TransactionType = dto.TransactionType;
            ledger.TransactionId = dto.TransactionId;

            _context.SaveChanges();

            return Ok(ledger);
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ledger = _context.StockLedgers.Find(id);

            if (ledger == null)
                return NotFound();

            _context.StockLedgers.Remove(ledger);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}
