using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/bin-stock")]
    public class BinStockController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BinStockController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.BinStocks.ToList());
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var binStock = _context.BinStocks.Find(id);

            if (binStock == null)
                return NotFound();

            return Ok(binStock);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(BinStockDto dto)
        {
            var validationError = ValidateBinStock(dto, null);
            if (validationError != null)
                return validationError;

            var existingBinStock = _context.BinStocks.FirstOrDefault(item =>
                item.ProductId == dto.ProductId &&
                item.VariantId == dto.VariantId &&
                item.WarehouseId == dto.WarehouseId &&
                item.BinId == dto.BinId);

            if (existingBinStock != null)
                return Conflict("Bin stock already exists for this product and bin. Use putaway or update the existing record.");

            var binStock = new BinStock
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                WarehouseId = dto.WarehouseId,
                BinId = dto.BinId,
                Quantity = dto.Quantity
            };

            _context.BinStocks.Add(binStock);
            _context.SaveChanges();

            return Ok(binStock);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, BinStockDto dto)
        {
            var validationError = ValidateBinStock(dto, id);
            if (validationError != null)
                return validationError;

            var binStock = _context.BinStocks.Find(id);

            if (binStock == null)
                return NotFound();

            binStock.ProductId = dto.ProductId;
            binStock.VariantId = dto.VariantId;
            binStock.WarehouseId = dto.WarehouseId;
            binStock.BinId = dto.BinId;
            binStock.Quantity = dto.Quantity;

            _context.SaveChanges();

            return Ok(binStock);
        }

        private IActionResult? ValidateBinStock(BinStockDto dto, int? existingBinStockId)
        {
            if (dto.ProductId <= 0 || dto.WarehouseId <= 0 || dto.BinId <= 0)
                return BadRequest("Product, warehouse, and bin are required.");

            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var product = _context.Products.Find(dto.ProductId);
            if (product == null)
                return BadRequest("Invalid ProductId");

            var warehouse = _context.Warehouses.Find(dto.WarehouseId);
            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            var bin = _context.Bins.Find(dto.BinId);
            if (bin == null)
                return BadRequest("Invalid BinId");

            if (bin.WarehouseId != dto.WarehouseId)
                return BadRequest("Bin does not belong to the selected warehouse.");

            var stock = _context.Stocks.FirstOrDefault(item =>
                item.ProductId == dto.ProductId &&
                item.WarehouseId == dto.WarehouseId &&
                item.VariantId == dto.VariantId);

            if (stock == null)
                return BadRequest("No warehouse stock exists for this product.");

            var allocatedWarehouseQuantity = _context.BinStocks
                .Where(item =>
                    item.ProductId == dto.ProductId &&
                    item.WarehouseId == dto.WarehouseId &&
                    item.VariantId == dto.VariantId &&
                    (!existingBinStockId.HasValue || item.BinStockId != existingBinStockId.Value))
                .Sum(item => (decimal?)item.Quantity) ?? 0;

            if (allocatedWarehouseQuantity + dto.Quantity > stock.Quantity)
                return BadRequest($"Quantity exceeds warehouse stock. Available unallocated stock: {stock.Quantity - allocatedWarehouseQuantity}.");

            var allocatedBinQuantity = _context.BinStocks
                .Where(item =>
                    item.BinId == dto.BinId &&
                    (!existingBinStockId.HasValue || item.BinStockId != existingBinStockId.Value))
                .Sum(item => (decimal?)item.Quantity) ?? 0;

            if (bin.Capacity.HasValue && allocatedBinQuantity + dto.Quantity > bin.Capacity.Value)
                return BadRequest($"Quantity exceeds bin capacity. Available bin capacity: {bin.Capacity.Value - allocatedBinQuantity}.");

            return null;
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var binStock = _context.BinStocks.Find(id);

            if (binStock == null)
                return NotFound();

            _context.BinStocks.Remove(binStock);
            _context.SaveChanges();

            return Ok("Bin stock deleted successfully");
        }
    }
}
