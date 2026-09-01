using System.Security.Claims;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/putaway-stock")]
    public class PutawayStockController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PutawayStockController> _logger;

        public PutawayStockController(
            AppDbContext context,
            ILogger<PutawayStockController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PutawayStockDto dto,
            CancellationToken cancellationToken)
        {
            // ---------------------------------------------------------
            // 1. Validate input
            // ---------------------------------------------------------

            if (dto.ProductId <= 0)
                return BadRequest("ProductId is required.");

           

            if (dto.WarehouseId <= 0)
                return BadRequest("WarehouseId is required.");

            if (dto.RackId <= 0)
                return BadRequest("RackId is required.");

            if (dto.BinId <= 0)
                return BadRequest("BinId is required.");

            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");


            // ---------------------------------------------------------
            // 2. Get product
            // ---------------------------------------------------------

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.ProductId == dto.ProductId && !x.IsDeleted,
                    cancellationToken);

            if (product == null)
                return BadRequest("Invalid ProductId.");


            // ---------------------------------------------------------
            // 3. Get variant
            // ---------------------------------------------------------

            ProductVariant? variant = null;

            if (dto.VariantId.HasValue)
            {
                variant = await _context.ProductVariants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.VariantId == dto.VariantId.Value &&
                            x.ProductId == dto.ProductId,
                        cancellationToken);

                if (variant == null)
                {
                    return BadRequest(
                        $"VariantId {dto.VariantId} does not belong to ProductId {dto.ProductId}.");
                }
            }


            // ---------------------------------------------------------
            // 4. Warehouse validation
            // ---------------------------------------------------------

            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.WarehouseId == dto.WarehouseId,
                    cancellationToken);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId.");


            // ---------------------------------------------------------
            // 5. Rack validation
            // ---------------------------------------------------------

            var rack = await _context.Racks
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.RackId == dto.RackId &&
                        x.WarehouseId == dto.WarehouseId,
                    cancellationToken);

            if (rack == null)
            {
                return BadRequest(
                    "Rack does not belong to the selected warehouse.");
            }


            // ---------------------------------------------------------
            // 6. Bin validation
            // ---------------------------------------------------------

            var bin = await _context.Bins
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.BinId == dto.BinId &&
                        x.RackId == dto.RackId &&
                        x.WarehouseId == dto.WarehouseId,
                    cancellationToken);

            if (bin == null)
            {
                return BadRequest(
                    "Bin does not belong to the selected rack and warehouse.");
            }


            // ---------------------------------------------------------
            // 7. IMPORTANT:
            // Find stock using ProductId + VariantId + WarehouseId
            //
            // This prevents Product 1 Variant 1 from accidentally
            // using Product 1 Variant NULL.
            // ---------------------------------------------------------

            var stock = await _context.Stocks
    .FirstOrDefaultAsync(
        x =>
            x.ProductId == dto.ProductId &&
            x.WarehouseId == dto.WarehouseId &&
            (!dto.VariantId.HasValue || x.VariantId == dto.VariantId.Value),
        cancellationToken);

            if (stock == null)
            {
                return BadRequest(
                    $"No warehouse stock exists for ProductId {dto.ProductId}, " +
                    $"VariantId {dto.VariantId}, WarehouseId {dto.WarehouseId}.");
            }

            if (stock.Quantity <= 0)
            {
                return BadRequest(
                    $"No available warehouse stock for ProductId {dto.ProductId}, " +
                    $"VariantId {dto.VariantId}.");
            }


            // ---------------------------------------------------------
            // 8. Calculate already allocated bin stock
            //
            // IMPORTANT:
            // VariantId is included in the filter.
            // ---------------------------------------------------------

            var allocatedQuantity = await _context.BinStocks
    .Where(x =>
        x.ProductId == dto.ProductId &&
        x.WarehouseId == dto.WarehouseId &&
        (!dto.VariantId.HasValue || x.VariantId == dto.VariantId.Value))
    .SumAsync(
        x => (decimal?)x.Quantity,
        cancellationToken) ?? 0;


            var availableWarehouseQuantity =
                stock.Quantity - allocatedQuantity;


            // ---------------------------------------------------------
            // 9. Check available warehouse stock
            // ---------------------------------------------------------

            if (dto.Quantity > availableWarehouseQuantity)
            {
                return BadRequest(
                    $"Quantity exceeds available unallocated warehouse stock. " +
                    $"Available: {availableWarehouseQuantity}.");
            }


            // ---------------------------------------------------------
            // 10. Check current bin capacity
            // ---------------------------------------------------------

            var currentBinQuantity = await _context.BinStocks
                .Where(x =>
                    x.BinId == dto.BinId &&
                    x.WarehouseId == dto.WarehouseId)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken) ?? 0;


            if (bin.Capacity.HasValue &&
                currentBinQuantity + dto.Quantity > bin.Capacity.Value)
            {
                var remainingCapacity =
                    bin.Capacity.Value - currentBinQuantity;

                return BadRequest(
                    $"Quantity exceeds bin capacity. " +
                    $"Available bin capacity: {remainingCapacity}.");
            }


            // ---------------------------------------------------------
            // 11. Begin transaction
            // ---------------------------------------------------------

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                // -----------------------------------------------------
                // 12. Find existing BinStock
                //
                // IMPORTANT:
                // ProductId + VariantId + WarehouseId + BinId
                // -----------------------------------------------------

                var binStock = await _context.BinStocks
     .FirstOrDefaultAsync(
         x =>
             x.ProductId == dto.ProductId &&
             x.WarehouseId == dto.WarehouseId &&
             x.BinId == dto.BinId &&
             (!dto.VariantId.HasValue || x.VariantId == dto.VariantId.Value),
         cancellationToken);


                // -----------------------------------------------------
                // 13. Create or update BinStock
                // -----------------------------------------------------

                if (binStock == null)
                {
                    binStock = new BinStock
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = dto.WarehouseId,
                        BinId = dto.BinId,
                        Quantity = dto.Quantity
                    };

                    _context.BinStocks.Add(binStock);
                }
                else
                {
                    binStock.Quantity += dto.Quantity;
                }


                // -----------------------------------------------------
                // 14. Putaway audit
                // -----------------------------------------------------

                var audit = new PutawayAudit
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.WarehouseId,
                    RackId = dto.RackId,
                    BinId = dto.BinId,
                    Quantity = dto.Quantity,
                    UserId = GetCurrentUserId(),
                    UserName = GetCurrentUserName(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.PutawayAudits.Add(audit);


                // -----------------------------------------------------
                // 15. Stock movement
                // -----------------------------------------------------

                var stockMovement = new StockMovement
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.WarehouseId,
                    MovementType = "Putaway",
                    Quantity = dto.Quantity,
                    ReferenceType = "putaway",
                    Notes =
                        $"Putaway Product {dto.ProductId}, " +
                        $"Variant {dto.VariantId} to " +
                        $"Rack {rack.RackCode} / Bin {bin.BinCode}",
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockMovements.Add(stockMovement);


                // -----------------------------------------------------
                // 16. Save
                // -----------------------------------------------------

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);


                // -----------------------------------------------------
                // 17. Log
                // -----------------------------------------------------

                _logger.LogInformation(
                    "Warehouse putaway completed. " +
                    "ProductId={ProductId}, " +
                    "VariantId={VariantId}, " +
                    "WarehouseId={WarehouseId}, " +
                    "RackId={RackId}, " +
                    "BinId={BinId}, " +
                    "Quantity={Quantity}, " +
                    "BinStockId={BinStockId}, " +
                    "UserId={UserId}",
                    dto.ProductId,
                    dto.VariantId,
                    dto.WarehouseId,
                    dto.RackId,
                    dto.BinId,
                    dto.Quantity,
                    binStock.BinStockId,
                    audit.UserId);


                // -----------------------------------------------------
                // 18. Response
                // -----------------------------------------------------

                return Ok(new
                {
                    success = true,

                    data = new
                    {
                        binStockId = binStock.BinStockId,

                        productId = dto.ProductId,

                        variantId = dto.VariantId,

                        variantName = variant?.VariantName,

                        warehouseId = dto.WarehouseId,

                        rackId = dto.RackId,

                        binId = dto.BinId,

                        quantity = binStock.Quantity,

                        putawayQuantity = dto.Quantity,

                        availableWarehouseQuantity =
                            availableWarehouseQuantity - dto.Quantity,

                        auditId = audit.PutawayAuditId
                    },

                    message = "Stock putaway completed successfully."
                });
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }


        // -------------------------------------------------------------
        // Get logged-in user ID
        // -------------------------------------------------------------

        private int? GetCurrentUserId()
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("UserId") ??
                User.FindFirstValue("userId") ??
                User.FindFirstValue("sub");

            return int.TryParse(value, out var userId)
                ? userId
                : null;
        }


        // -------------------------------------------------------------
        // Get logged-in username
        // -------------------------------------------------------------

        private string? GetCurrentUserName()
        {
            return
                User.FindFirstValue(ClaimTypes.Name) ??
                User.FindFirstValue("name") ??
                User.FindFirstValue(ClaimTypes.Email);
        }
    }
}