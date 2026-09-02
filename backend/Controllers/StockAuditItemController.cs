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
        // 🔹 GET PRODUCTS BY AUDIT
        // =========================
        [HttpGet("products")]
        public IActionResult GetProductsByAudit([FromQuery] int auditId)
        {
            var audit = _context.StockAudits.Find(auditId);
            if (audit == null)
                return BadRequest("Invalid AuditId.");

            var warehouseId = audit.WarehouseId;

            var binStockProductIds = _context.BinStocks
                .Where(bs => bs.WarehouseId == warehouseId)
                .Select(bs => bs.ProductId);

            var stockProductIds = _context.Stocks
                .Where(s => s.WarehouseId == warehouseId)
                .Select(s => s.ProductId);

            var warehouseProductIds = _context.Products
                .Where(p => p.WarehouseId == warehouseId && !p.IsDeleted)
                .Select(p => p.ProductId);

            var allProductIds = binStockProductIds
                .Union(stockProductIds)
                .Union(warehouseProductIds)
                .Distinct();

            var products = _context.Products
                .Where(p => allProductIds.Contains(p.ProductId) && !p.IsDeleted)
                .Select(p => new
                {
                    p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU
                })
                .OrderBy(p => p.Name)
                .ToList();

            return Ok(products);
        }

        // =========================
        // 🔹 GET VARIANTS BY AUDIT + PRODUCT
        // =========================
        [HttpGet("variants")]
        public IActionResult GetVariantsByAuditProduct([FromQuery] int auditId, [FromQuery] int productId)
        {
            var audit = _context.StockAudits.Find(auditId);
            if (audit == null)
                return BadRequest("Invalid AuditId.");

            var warehouseId = audit.WarehouseId;

            var binStockVariantIds = _context.BinStocks
                .Where(bs => bs.WarehouseId == warehouseId && bs.ProductId == productId && bs.VariantId.HasValue && bs.VariantId > 0)
                .Select(bs => bs.VariantId!.Value);

            var stockVariantIds = _context.Stocks
                .Where(s => s.WarehouseId == warehouseId && s.ProductId == productId && s.VariantId.HasValue && s.VariantId > 0)
                .Select(s => s.VariantId!.Value);

            var productVariantIds = binStockVariantIds
                .Union(stockVariantIds)
                .ToList();

            IQueryable<ProductVariant> query = _context.ProductVariants.Where(v => v.ProductId == productId);
            if (productVariantIds.Count > 0)
            {
                query = query.Where(v => productVariantIds.Contains(v.VariantId));
            }

            var variants = query
                .Select(v => new
                {
                    v.VariantId,
                    v.ProductId,
                    v.VariantName,
                    v.SKU
                })
                .OrderBy(v => v.VariantName)
                .ToList();

            return Ok(variants);
        }

        // =========================
        // 🔹 GET BINS BY AUDIT + PRODUCT + VARIANT
        // =========================
        [HttpGet("bins")]
        public IActionResult GetBinsByAuditProductVariant([FromQuery] int auditId, [FromQuery] int productId, [FromQuery] int? variantId)
        {
            var audit = _context.StockAudits.Find(auditId);
            if (audit == null)
                return BadRequest("Invalid AuditId.");

            var warehouseId = audit.WarehouseId;

            var query = _context.BinStocks
                .Where(bs => bs.WarehouseId == warehouseId && bs.ProductId == productId);

            if (variantId.HasValue && variantId.Value > 0)
            {
                query = query.Where(bs => bs.VariantId == variantId.Value);
            }

            var matchingBinStocks = query.ToList();
            var binIds = matchingBinStocks.Select(bs => bs.BinId).Distinct().ToList();

            var binsInfo = _context.Bins
                .Where(b => binIds.Contains(b.BinId))
                .ToDictionary(b => b.BinId, b => b.BinCode);

            var result = matchingBinStocks
                .Select(bs => new
                {
                    bs.BinId,
                    BinCode = binsInfo.ContainsKey(bs.BinId) && !string.IsNullOrWhiteSpace(binsInfo[bs.BinId])
                        ? binsInfo[bs.BinId]
                        : $"BIN-{bs.BinId}",
                    bs.Quantity
                })
                .OrderBy(b => b.BinCode)
                .ToList();

            return Ok(result);
        }

        // =========================
        // 🔹 GET SYSTEM QUANTITY BY AUDIT + PRODUCT + VARIANT + BIN
        // =========================
        [HttpGet("system-quantity")]
        public IActionResult GetSystemQuantity([FromQuery] int auditId, [FromQuery] int productId, [FromQuery] int binId, [FromQuery] int? variantId)
        {
            var audit = _context.StockAudits.Find(auditId);
            if (audit == null)
                return BadRequest("Invalid AuditId.");

            var warehouseId = audit.WarehouseId;

            var query = _context.BinStocks
                .Where(bs => bs.WarehouseId == warehouseId && bs.ProductId == productId && bs.BinId == binId);

            if (variantId.HasValue && variantId.Value > 0)
            {
                query = query.Where(bs => bs.VariantId == variantId.Value);
            }

            var binStock = query.FirstOrDefault();
            var bin = _context.Bins.Find(binId);

            return Ok(new
            {
                SystemQuantity = binStock?.Quantity ?? 0,
                BinId = binId,
                BinCode = bin?.BinCode ?? $"BIN-{binId}"
            });
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
            if (!dto.AuditId.HasValue || dto.AuditId.Value <= 0)
                return BadRequest("AuditId is required.");

            if (!dto.ProductId.HasValue || dto.ProductId.Value <= 0)
                return BadRequest("ProductId is required.");

            if (!dto.BinId.HasValue || dto.BinId.Value <= 0)
                return BadRequest("BinId is required.");

            if (!dto.PhysicalQuantity.HasValue)
                return BadRequest("PhysicalQuantity is required.");

            var audit = _context.StockAudits.Find(dto.AuditId.Value);
            if (audit == null)
                return BadRequest("Invalid AuditId.");

            var product = _context.Products.Find(dto.ProductId.Value);
            if (product == null)
                return BadRequest("Invalid ProductId.");

            var bin = _context.Bins.Find(dto.BinId.Value);
            if (bin == null)
                return BadRequest("Invalid BinId.");

            if (bin.WarehouseId != audit.WarehouseId)
                return BadRequest("Selected Bin does not belong to the Audit's warehouse.");

            decimal sysQty = dto.SystemQuantity ?? 0;
            var binStockQuery = _context.BinStocks
                .Where(bs => bs.WarehouseId == audit.WarehouseId && bs.ProductId == dto.ProductId.Value && bs.BinId == dto.BinId.Value);

            if (dto.VariantId.HasValue && dto.VariantId.Value > 0)
            {
                binStockQuery = binStockQuery.Where(bs => bs.VariantId == dto.VariantId.Value);
            }

            var existingBinStock = binStockQuery.FirstOrDefault();
            if (existingBinStock != null)
            {
                sysQty = existingBinStock.Quantity;
            }

            decimal difference = (dto.PhysicalQuantity ?? 0) - sysQty;

            var item = new StockAuditItem
            {
                AuditId = dto.AuditId,
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                BinId = dto.BinId,
                SystemQuantity = sysQty,
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

        // =========================
        // 🔹 GET ALL AUDIT ITEMS DATA BY AUDIT
        // =========================
        [HttpGet("by-audit/{auditId}")]
        public IActionResult GetAuditItemsByAudit(int auditId)
        {
            // 1. Find the selected Stock Audit
            var audit = _context.StockAudits.Find(auditId);

            if (audit == null)
                return BadRequest("Invalid AuditId.");

            // 2. Get the warehouse belonging to this Audit
            var warehouseId = audit.WarehouseId;

            // 3. Get all BinStocks from this Audit's warehouse
            //    Each Product + Variant + Bin combination becomes one row
            var binStocks = _context.BinStocks
                .Where(bs => bs.WarehouseId == warehouseId)
                .ToList();

            // 4. Get required product IDs
            var productIds = binStocks
                .Select(bs => bs.ProductId)
                .Distinct()
                .ToList();

            var products = _context.Products
                .Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .ToDictionary(p => p.ProductId, p => p);

            // 5. Get required variant IDs
            var variantIds = binStocks
                .Where(bs => bs.VariantId.HasValue && bs.VariantId.Value > 0)
                .Select(bs => bs.VariantId!.Value)
                .Distinct()
                .ToList();

            var variants = _context.ProductVariants
                .Where(v => variantIds.Contains(v.VariantId))
                .ToDictionary(v => v.VariantId, v => v);

            // 6. Get required bin IDs
            var binIds = binStocks
                .Select(bs => bs.BinId)
                .Distinct()
                .ToList();

            var bins = _context.Bins
                .Where(b => binIds.Contains(b.BinId))
                .ToDictionary(b => b.BinId, b => b);

            // 7. Build the final result
            var result = binStocks
                .Where(bs => products.ContainsKey(bs.ProductId))
                .Select(bs => new
                {
                    AuditId = audit.AuditId,

                    ProductId = bs.ProductId,
                    ProductName = products[bs.ProductId].Name,

                    VariantId = bs.VariantId,
                    VariantName =
                        bs.VariantId.HasValue &&
                        variants.ContainsKey(bs.VariantId.Value)
                            ? variants[bs.VariantId.Value].VariantName
                            : null,

                    BinId = bs.BinId,
                    BinCode =
                        bins.ContainsKey(bs.BinId) &&
                        !string.IsNullOrWhiteSpace(bins[bs.BinId].BinCode)
                            ? bins[bs.BinId].BinCode
                            : $"BIN-{bs.BinId}",

                    SystemQuantity = bs.Quantity
                })
                .OrderBy(x => x.ProductName)
                .ThenBy(x => x.VariantName)
                .ThenBy(x => x.BinCode)
                .ToList();

            return Ok(result);
        }

    }
}