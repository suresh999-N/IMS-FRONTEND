using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-audit-items")]
    public class StockAuditItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockAuditItemController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.StockAuditItems.ToList());
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.StockAuditItems.Find(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(StockAuditItemDto dto)
        {
            // Calculate difference
            decimal difference =
                (dto.PhysicalQuantity ?? 0) -
                (dto.SystemQuantity ?? 0);

            var item = new StockAuditItem
            {
                AuditId = dto.AuditId,
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                BinId = dto.BinId,
                SystemQuantity = dto.SystemQuantity,
                PhysicalQuantity = dto.PhysicalQuantity,
                Difference = difference
            };

            _context.StockAuditItems.Add(item);
            _context.SaveChanges();

            return Ok(item);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, StockAuditItemDto dto)
        {
            var item = _context.StockAuditItems.Find(id);

            if (item == null)
                return NotFound();

            decimal difference =
                (dto.PhysicalQuantity ?? 0) -
                (dto.SystemQuantity ?? 0);

            item.AuditId = dto.AuditId;
            item.ProductId = dto.ProductId;
            item.VariantId = dto.VariantId;
            item.BinId = dto.BinId;
            item.SystemQuantity = dto.SystemQuantity;
            item.PhysicalQuantity = dto.PhysicalQuantity;
            item.Difference = difference;

            _context.SaveChanges();

            return Ok(item);
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.StockAuditItems.Find(id);

            if (item == null)
                return NotFound();

            _context.StockAuditItems.Remove(item);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}