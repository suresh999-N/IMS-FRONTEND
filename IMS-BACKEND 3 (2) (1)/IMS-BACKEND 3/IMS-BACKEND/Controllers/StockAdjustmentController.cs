using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-adjustments")]
    public class StockAdjustmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockAdjustmentController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.StockAdjustments.ToList());
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var adjustment = _context.StockAdjustments.Find(id);

            if (adjustment == null)
                return NotFound();

            return Ok(adjustment);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(StockAdjustmentDto dto)
        {
            // Check Warehouse
            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var adjustment = new StockAdjustment
            {
                WarehouseId = dto.WarehouseId,
                AdjustmentType = dto.AdjustmentType,
                Reason = dto.Reason,
                CreatedAt = DateTime.Now
            };

            _context.StockAdjustments.Add(adjustment);
            _context.SaveChanges();

            return Ok(adjustment);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, StockAdjustmentDto dto)
        {
            var adjustment = _context.StockAdjustments.Find(id);

            if (adjustment == null)
                return NotFound();

            adjustment.WarehouseId = dto.WarehouseId;
            adjustment.AdjustmentType = dto.AdjustmentType;
            adjustment.Reason = dto.Reason;

            _context.SaveChanges();

            return Ok(adjustment);
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var adjustment = _context.StockAdjustments.Find(id);

            if (adjustment == null)
                return NotFound();

            _context.StockAdjustments.Remove(adjustment);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}