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

            if (string.IsNullOrWhiteSpace(dto.AdjustmentType))
                return BadRequest("Adjustment Type is required.");

            var normType = dto.AdjustmentType.Trim().ToLowerInvariant();
            if (normType != "increase" && normType != "decrease" && normType != "recount")
                return BadRequest($"Invalid Adjustment Type '{dto.AdjustmentType}'. Allowed values are: increase, decrease, recount.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("Reason is required.");

            if (dto.Reason.Trim().Length < 3)
                return BadRequest("Reason must be at least 3 characters.");

            if (!IsValidStockAdjustmentReason(normType, dto.Reason, out string? reasonError))
                return BadRequest(reasonError);

            var adjustment = new StockAdjustment
            {
                WarehouseId = dto.WarehouseId,
                AdjustmentType = normType,
                Reason = dto.Reason.Trim(),
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

            var warehouse = _context.Warehouses.Find(dto.WarehouseId);
            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            if (string.IsNullOrWhiteSpace(dto.AdjustmentType))
                return BadRequest("Adjustment Type is required.");

            var normType = dto.AdjustmentType.Trim().ToLowerInvariant();
            if (normType != "increase" && normType != "decrease" && normType != "recount")
                return BadRequest($"Invalid Adjustment Type '{dto.AdjustmentType}'. Allowed values are: increase, decrease, recount.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("Reason is required.");

            if (dto.Reason.Trim().Length < 3)
                return BadRequest("Reason must be at least 3 characters.");

            if (!IsValidStockAdjustmentReason(normType, dto.Reason, out string? reasonError))
                return BadRequest(reasonError);

            adjustment.WarehouseId = dto.WarehouseId;
            adjustment.AdjustmentType = normType;
            adjustment.Reason = dto.Reason.Trim();

            _context.SaveChanges();

            return Ok(adjustment);
        }

        private static bool IsValidStockAdjustmentReason(string adjustmentType, string reason, out string? errorMessage)
        {
            errorMessage = null;
            var normReason = reason.Trim().ToLowerInvariant();

            if (adjustmentType == "increase")
            {
                if (normReason.Contains("high stock") || normReason.Contains("overstock") || normReason.Contains("excess stock"))
                {
                    errorMessage = "Reason 'high stock' is invalid for Increase adjustment. Increase adjustments are for replenishments or positive corrections.";
                    return false;
                }

                if (normReason.Contains("damage") || normReason.Contains("expiry") || normReason.Contains("expired") ||
                    normReason.Contains("shrinkage") || normReason.Contains("theft") || normReason.Contains("stolen") ||
                    normReason.Contains("loss") || normReason.Contains("lost") || normReason.Contains("broken") ||
                    normReason.Contains("spoilage") || normReason.Contains("spoiled") || normReason.Contains("waste") ||
                    normReason.Contains("scrap") || normReason.Contains("write-off") || normReason.Contains("deficit"))
                {
                    errorMessage = "Damage, expiry, shrinkage, loss, or waste causes inventory reduction and cannot be used for an Increase adjustment.";
                    return false;
                }

                string[] validKeywords = {
                    "low stock", "new purchase", "purchase", "correction", "correct",
                    "found", "surplus", "inward", "opening", "restock", "received extra",
                    "excess found", "replenish", "return", "recount", "unaccounted", "addition", "balance"
                };

                if (!validKeywords.Any(kw => normReason.Contains(kw)))
                {
                    errorMessage = "Invalid reason for Increase adjustment. Valid reasons include: low stock, new purchase, correction, found inventory, or audit surplus.";
                    return false;
                }

                return true;
            }

            if (adjustmentType == "decrease")
            {
                if (normReason.Contains("low stock"))
                {
                    errorMessage = "Reason 'low stock' is invalid for Decrease adjustment. Decrease adjustments are for reductions (e.g., damage, expiry, shrinkage).";
                    return false;
                }

                if (normReason.Contains("high stock") || normReason.Contains("overstock"))
                {
                    errorMessage = "Reason 'high stock' is invalid for Decrease adjustment. Please use a reduction reason like damage, expiry, shrinkage, or correction.";
                    return false;
                }

                if (normReason.Contains("new purchase") || normReason.Contains("purchase order") ||
                    normReason.Contains("found inventory") || normReason.Contains("found stock") ||
                    normReason.Contains("surplus") || normReason.Contains("received extra") ||
                    normReason.Contains("restock") || normReason.Contains("inward discrepancy"))
                {
                    errorMessage = "New purchase, found inventory, or surplus adds inventory and cannot be used for a Decrease adjustment.";
                    return false;
                }

                string[] validKeywords = {
                    "damage", "damaged", "expiry", "expired", "shrinkage", "theft", "stolen",
                    "pilferage", "loss", "lost", "broken", "breakage", "spoilage", "spoiled",
                    "waste", "scrap", "write-off", "correction", "correct", "deficit",
                    "shortage", "outward discrepancy", "sample", "testing", "consumption",
                    "recount", "reduction", "supplier return", "defect"
                };

                if (!validKeywords.Any(kw => normReason.Contains(kw)))
                {
                    errorMessage = "Invalid reason for Decrease adjustment. Valid reasons include: damage, expiry, shrinkage, theft, loss, breakage, or correction.";
                    return false;
                }

                return true;
            }

            if (adjustmentType == "recount")
            {
                if (normReason.Contains("high stock"))
                {
                    errorMessage = "Reason 'high stock' is invalid for Recount adjustment.";
                    return false;
                }

                string[] validKeywords = {
                    "count", "physical count", "cycle count", "audit", "reconciliation",
                    "correction", "correct", "discrepancy", "recount", "verification", "variance"
                };

                if (!validKeywords.Any(kw => normReason.Contains(kw)))
                {
                    errorMessage = "Invalid reason for Recount adjustment. Valid reasons include: physical count reconciliation, cycle count, or audit correction.";
                    return false;
                }

                return true;
            }

            return true;
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

            var associatedItems = _context.StockAdjustmentItems
                .Where(item => item.AdjustmentId == id)
                .ToList();

            if (associatedItems.Any())
            {
                _context.StockAdjustmentItems.RemoveRange(associatedItems);
            }

            _context.StockAdjustments.Remove(adjustment);

            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}