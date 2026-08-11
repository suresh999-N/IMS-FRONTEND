using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RacksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RacksController> _logger;

        public RacksController(AppDbContext context, ILogger<RacksController> logger)
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
            return Ok(_context.Racks.ToList());
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var rack = _context.Racks.Find(id);

            if (rack == null)
                return NotFound();

            return Ok(rack);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(RackDto dto)
        {
            var rackCode = dto.RackCode?.Trim();

            if (dto.WarehouseId <= 0 || string.IsNullOrWhiteSpace(rackCode))
                return BadRequest("Warehouse and rack code are required.");

            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var duplicateRack = _context.Racks.Any(rack =>
                rack.WarehouseId == dto.WarehouseId &&
                rack.RackCode != null &&
                rack.RackCode.ToLower() == rackCode.ToLower());

            if (duplicateRack)
                return Conflict(new
                {
                    message = "Rack code already exists in this warehouse."
                });

            var rack = new Rack
            {
                WarehouseId = dto.WarehouseId,
                ZoneId = dto.ZoneId,
                RackCode = rackCode,
                Description = dto.Description
            };

            _context.Racks.Add(rack);
            _context.SaveChanges();

            _logger.LogInformation(
                "Warehouse rack created. RackId={RackId}, WarehouseId={WarehouseId}, RackCode={RackCode}",
                rack.RackId,
                rack.WarehouseId,
                rack.RackCode);



            return Ok(rack);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, RackDto dto)
        {
            var rackCode = dto.RackCode?.Trim();

            if (dto.WarehouseId <= 0 || string.IsNullOrWhiteSpace(rackCode))
                return BadRequest("Warehouse and rack code are required.");

            var rack = _context.Racks.Find(id);

            if (rack == null)
                return NotFound();

            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var duplicateRack = _context.Racks.Any(item =>
                item.RackId != id &&
                item.WarehouseId == dto.WarehouseId &&
                item.RackCode != null &&
                item.RackCode.ToLower() == rackCode.ToLower());

            if (duplicateRack)
                return Conflict(new
                {
                    message = "Rack code already exists in this warehouse."
                });

            rack.WarehouseId = dto.WarehouseId;
            rack.ZoneId = dto.ZoneId;
            rack.RackCode = rackCode;
            rack.Description = dto.Description;

            _context.SaveChanges();

            _logger.LogInformation(
                "Warehouse rack updated. RackId={RackId}, WarehouseId={WarehouseId}, RackCode={RackCode}",
                rack.RackId,
                rack.WarehouseId,
                rack.RackCode);

            return Ok(rack);
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rack = _context.Racks.Find(id);

            if (rack == null)
                return NotFound();

            var hasBins = _context.Bins.Any(bin => bin.RackId == id);

            if (hasBins)
                return Conflict("Rack cannot be deleted because it contains bins.");

            _context.Racks.Remove(rack);
            _context.SaveChanges();

            _logger.LogInformation(
                "Warehouse rack deleted. RackId={RackId}, WarehouseId={WarehouseId}, RackCode={RackCode}",
                rack.RackId,
                rack.WarehouseId,
                rack.RackCode);

            return Ok("Rack deleted successfully");
        }
    }
}
