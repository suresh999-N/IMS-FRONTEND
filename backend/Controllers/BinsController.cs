using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BinsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BinsController> _logger;

        public BinsController(AppDbContext context, ILogger<BinsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Bins.ToList());
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var bin = _context.Bins.Find(id);

            if (bin == null)
                return NotFound();

            return Ok(bin);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(BinDto dto)
        {
            var binCode = dto.BinCode?.Trim();

            if (dto.WarehouseId <= 0 || dto.RackId <= 0 || string.IsNullOrWhiteSpace(binCode))
                return BadRequest("Warehouse, rack, and bin code are required.");

            if (dto.Capacity.HasValue && dto.Capacity.Value <= 0)
                return BadRequest("Bin capacity must be greater than zero.");

            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var rack = _context.Racks.Find(dto.RackId);

            if (rack == null)
                return BadRequest("Invalid RackId");

            if (rack.WarehouseId != dto.WarehouseId)
                return BadRequest("Rack does not belong to the selected warehouse.");

            var duplicateBin = _context.Bins.Any(bin =>
                bin.WarehouseId == dto.WarehouseId &&
                bin.BinCode != null &&
                bin.BinCode.ToLower() == binCode.ToLower());

            if (duplicateBin)
                return Conflict(new
                {
                    message = "Bin code already exists in this warehouse."
                });

            var bin = new Bin
            {
                WarehouseId = dto.WarehouseId,
                RackId = dto.RackId,
                BinCode = binCode,
                Capacity = dto.Capacity,
                Status = dto.Status ?? "active"
            };

            _context.Bins.Add(bin);
            _context.SaveChanges();

            _logger.LogInformation(
                "Warehouse bin created. BinId={BinId}, RackId={RackId}, WarehouseId={WarehouseId}, BinCode={BinCode}, Capacity={Capacity}",
                bin.BinId,
                bin.RackId,
                bin.WarehouseId,
                bin.BinCode,
                bin.Capacity);

            return Ok(bin);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, BinDto dto)
        {
            var binCode = dto.BinCode?.Trim();

            if (dto.WarehouseId <= 0 || dto.RackId <= 0 || string.IsNullOrWhiteSpace(binCode))
                return BadRequest("Warehouse, rack, and bin code are required.");

            if (dto.Capacity.HasValue && dto.Capacity.Value <= 0)
                return BadRequest("Bin capacity must be greater than zero.");

            var bin = _context.Bins.Find(id);

            if (bin == null)
                return NotFound();

            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var rack = _context.Racks.Find(dto.RackId);

            if (rack == null)
                return BadRequest("Invalid RackId");

            if (rack.WarehouseId != dto.WarehouseId)
                return BadRequest("Rack does not belong to the selected warehouse.");

            var currentBinQuantity = _context.BinStocks
                .Where(stock => stock.BinId == id)
                .Sum(stock => (decimal?)stock.Quantity) ?? 0;

            if (dto.Capacity.HasValue && currentBinQuantity > dto.Capacity.Value)
                return BadRequest($"Bin capacity cannot be lower than current bin quantity ({currentBinQuantity}).");

            var duplicateBin = _context.Bins.Any(item =>
                item.BinId != id &&
                item.WarehouseId == dto.WarehouseId &&
                item.BinCode != null &&
                item.BinCode.ToLower() == binCode.ToLower());

            if (duplicateBin)
                return Conflict(new
                {
                    message = "Bin code already exists in this warehouse."
                });

            bin.WarehouseId = dto.WarehouseId;
            bin.RackId = dto.RackId;
            bin.BinCode = binCode;
            bin.Capacity = dto.Capacity;
            bin.Status = dto.Status;

            _context.SaveChanges();

            _logger.LogInformation(
                "Warehouse bin updated. BinId={BinId}, RackId={RackId}, WarehouseId={WarehouseId}, BinCode={BinCode}, Capacity={Capacity}, Status={Status}",
                bin.BinId,
                bin.RackId,
                bin.WarehouseId,
                bin.BinCode,
                bin.Capacity,
                bin.Status);

            return Ok(bin);
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var bin = _context.Bins.Find(id);

            if (bin == null)
                return NotFound();

            var hasStock = _context.BinStocks.Any(stock => stock.BinId == id && stock.Quantity > 0);

            if (hasStock)
                return Conflict("Bin cannot be deleted because stock exists in it.");

            _context.Bins.Remove(bin);
            _context.SaveChanges();

            _logger.LogInformation(
                "Warehouse bin deleted. BinId={BinId}, RackId={RackId}, WarehouseId={WarehouseId}, BinCode={BinCode}",
                bin.BinId,
                bin.RackId,
                bin.WarehouseId,
                bin.BinCode);

            return Ok("Bin deleted successfully");
        }
    }
}
