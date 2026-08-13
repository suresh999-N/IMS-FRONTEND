using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-movements")]
    public class StockMovementController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StockMovementController> _logger;

        public StockMovementController(
            AppDbContext context,
            ILogger<StockMovementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll()
        {
            var movements = (
                from movement in _context.StockMovements.AsNoTracking()
                join product in _context.Products.AsNoTracking()
                    on movement.ProductId equals product.ProductId into productRows
                from product in productRows.DefaultIfEmpty()
                join warehouse in _context.Warehouses.AsNoTracking()
                    on movement.WarehouseId equals warehouse.WarehouseId into warehouseRows
                from warehouse in warehouseRows.DefaultIfEmpty()
                join variant in _context.ProductVariants.AsNoTracking()
                    on movement.VariantId equals variant.VariantId into variantRows
                from variant in variantRows.DefaultIfEmpty()
                select new
                {
                    movement.MovementId,
                    movement.ProductId,
                    movement.VariantId,
                    movement.WarehouseId,
                    ProductName = product == null || product.Name == "" ? "Unknown Product" : product.Name,
                    product_name = product == null || product.Name == "" ? "Unknown Product" : product.Name,
                    WarehouseName = warehouse == null || warehouse.Name == "" ? "Unknown Warehouse" : warehouse.Name,
                    warehouse_name = warehouse == null || warehouse.Name == "" ? "Unknown Warehouse" : warehouse.Name,
                    VariantName = variant == null || variant.VariantName == "" ? "Standard" : variant.VariantName,
                    variant_name = variant == null || variant.VariantName == "" ? "Standard" : variant.VariantName,
                    movement.MovementType,
                    MovementTypeLabel = movement.MovementType == "OPENING" ? "Opening Stock" : movement.MovementType,
                    movement.Quantity,
                    movement.ReferenceId,
                    movement.ReferenceType,
                    ReferenceDisplay = movement.ReferenceType == null || movement.ReferenceType == ""
                        ? "Not set"
                        : movement.ReferenceType,
                    movement.Notes,
                    movement.CreatedAt
                }).OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.MovementId).ToList();

            return Ok(movements);
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var movement = _context.StockMovements.Find(id);

            if (movement == null)
                return NotFound();

            return Ok(movement);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(StockMovementDto dto, CancellationToken cancellationToken)
        {
            var movementType = NormalizeMovementType(dto.MovementType);
            if (movementType == null)
            {
                return BadRequest("Movement type must be one of: IN, OUT, SALE, PURCHASE, ADJUSTMENT, TRANSFER, OPENING.");
            }

            // Check Product
            if (dto.Quantity == 0)
                return BadRequest("Movement quantity cannot be zero.");

            var product = await _context.Products.FirstOrDefaultAsync(item =>
                item.ProductId == dto.ProductId &&
                !item.IsDeleted,
                cancellationToken);

            if (product == null)
                return BadRequest("Invalid ProductId");

            // Check Warehouse
            var warehouse = await _context.Warehouses.FindAsync(new object[] { dto.WarehouseId }, cancellationToken);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            if (dto.ReferenceId.HasValue && !string.IsNullOrWhiteSpace(dto.ReferenceType))
            {
                var duplicateMovement = await _context.StockMovements
                    .AsNoTracking()
                    .AnyAsync(item =>
                        item.ReferenceId == dto.ReferenceId &&
                        item.ReferenceType == dto.ReferenceType &&
                        item.MovementType == movementType &&
                        item.ProductId == dto.ProductId &&
                        item.VariantId == dto.VariantId &&
                        item.WarehouseId == dto.WarehouseId,
                        cancellationToken);

                if (duplicateMovement)
                    return Conflict("Duplicate stock movement for the same reference is not allowed.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var now = DateTime.UtcNow;
                var stock = await _context.Stocks.FirstOrDefaultAsync(item =>
                    item.ProductId == dto.ProductId &&
                    item.VariantId == dto.VariantId &&
                    item.WarehouseId == dto.WarehouseId,
                    cancellationToken);
                var openingQty = stock?.Quantity ?? 0;
                var changeQty = ResolveStockChange(movementType, dto.Quantity);
                var closingQty = openingQty + changeQty;

                if (closingQty < 0)
                    return BadRequest("Stock movement would create negative stock.");

                if (stock == null)
                {
                    stock = new Stock
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = dto.WarehouseId,
                        Quantity = closingQty,
                        ReservedQuantity = 0
                    };
                    _context.Stocks.Add(stock);
                }
                else
                {
                    stock.Quantity = closingQty;
                }

                var movement = new StockMovement
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.WarehouseId,
                    MovementType = movementType,
                    Quantity = dto.Quantity,
                    ReferenceId = dto.ReferenceId,
                    ReferenceType = dto.ReferenceType,
                    Notes = dto.Notes,
                    CreatedAt = now
                };

                _context.StockMovements.Add(movement);
                await _context.SaveChangesAsync(cancellationToken);

                _context.StockLedgers.Add(new StockLedger
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.WarehouseId,
                    OpeningQty = openingQty,
                    ChangeQty = changeQty,
                    ClosingQty = closingQty,
                    TransactionType = movementType,
                    TransactionId = movement.MovementId,
                    CreatedAt = now
                });

                product.UpdatedAt = now;
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(movement);
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Stock movement create failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    GetDbUpdateUserMessage(exception));
            }
        }
        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, StockMovementDto dto)
        {
            var movementType = NormalizeMovementType(dto.MovementType);
            if (movementType == null)
            {
                return BadRequest("Movement type must be one of: IN, OUT, SALE, PURCHASE, ADJUSTMENT, TRANSFER, OPENING.");
            }

            var movement = _context.StockMovements.Find(id);

            if (movement == null)
                return NotFound("Stock movement not found");

            // Check Product
            var product = _context.Products.FirstOrDefault(item =>
                item.ProductId == dto.ProductId &&
                !item.IsDeleted);

            if (product == null)
                return BadRequest("Invalid ProductId");

            // Check Warehouse
            var warehouse = _context.Warehouses.Find(dto.WarehouseId);

            if (warehouse == null)
                return BadRequest("Invalid WarehouseId");

            // Update values
            movement.ProductId = dto.ProductId;
            movement.VariantId = dto.VariantId;
            movement.WarehouseId = dto.WarehouseId;
            movement.MovementType = movementType;
            movement.Quantity = dto.Quantity;
            movement.ReferenceId = dto.ReferenceId;
            movement.ReferenceType = dto.ReferenceType;
            movement.Notes = dto.Notes;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Stock movement update failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    GetDbUpdateUserMessage(exception));
            }

            return Ok(movement);
        }

        private static string? NormalizeMovementType(string? movementType)
        {
            var normalized = movementType?.Trim().ToUpperInvariant();

            return normalized switch
            {
                "IN" or "STOCK_IN" or "RETURN_IN" => "IN",
                "OUT" or "STOCK_OUT" or "RETURN_OUT" => "OUT",
                "SALE" => "SALE",
                "PURCHASE" => "PURCHASE",
                "ADJUSTMENT" => "ADJUSTMENT",
                "TRANSFER" => "TRANSFER",
                "OPENING" or "OPENING_STOCK" => "OPENING",
                _ => null
            };
        }

        private static bool IsInboundMovement(string movementType)
        {
            return movementType is "IN" or "PURCHASE" or "OPENING";
        }

        private static bool IsOutboundMovement(string movementType)
        {
            return movementType is "OUT" or "SALE";
        }

        private static decimal ResolveStockChange(string movementType, decimal quantity)
        {
            if (IsInboundMovement(movementType))
            {
                return Math.Abs(quantity);
            }

            if (IsOutboundMovement(movementType))
            {
                return -Math.Abs(quantity);
            }

            return quantity;
        }

        private static string GetDbUpdateUserMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);

            if (detail.Contains("movement_type", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("truncated", StringComparison.OrdinalIgnoreCase))
            {
                return $"Invalid stock movement type for the database column. {detail}";
            }

            return detail;
        }

        private static string GetInnermostMessage(Exception exception)
        {
            var current = exception;

            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return string.IsNullOrWhiteSpace(current.Message)
                ? exception.Message
                : current.Message;
        }

        private void LogDbUpdateException(DbUpdateException exception, string message)
        {
            _logger.LogError(
                exception,
                "{Message} InnerException: {InnerException}",
                message,
                exception.InnerException?.ToString() ?? "No inner exception");
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
            => await Reverse(id, dto, cancellationToken);

        [HttpPost("{id}/reverse")]
        public async Task<IActionResult> Reverse(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var movement = await _context.StockMovements
                .FirstOrDefaultAsync(item => item.MovementId == id, cancellationToken);

            if (movement == null)
                return NotFound();

            var alreadyReversed = await _context.StockMovements
                .AsNoTracking()
                .AnyAsync(item =>
                    item.ReferenceId == movement.MovementId &&
                    item.ReferenceType == "stock_movement_reversal",
                    cancellationToken);

            if (alreadyReversed)
                return BadRequest("Stock movement is already reversed.");

            var movementType = NormalizeMovementType(movement.MovementType);
            if (movementType == null)
                return BadRequest("Cannot reverse a stock movement with an invalid movement type.");

            var stock = await _context.Stocks.FirstOrDefaultAsync(item =>
                item.ProductId == movement.ProductId &&
                item.VariantId == movement.VariantId &&
                item.WarehouseId == movement.WarehouseId,
                cancellationToken);
            var openingQty = stock?.Quantity ?? 0;
            var reversalQty = -ResolveStockChange(movementType, movement.Quantity);
            var closingQty = openingQty + reversalQty;

            if (closingQty < 0)
                return BadRequest("Stock movement cannot be reversed because it would create negative stock.");

            if (stock == null)
            {
                stock = new Stock
                {
                    ProductId = movement.ProductId,
                    VariantId = movement.VariantId,
                    WarehouseId = movement.WarehouseId,
                    Quantity = closingQty,
                    ReservedQuantity = 0
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                stock.Quantity = closingQty;
            }

            var now = DateTime.UtcNow;
            _context.StockMovements.Add(new StockMovement
            {
                ProductId = movement.ProductId,
                VariantId = movement.VariantId,
                WarehouseId = movement.WarehouseId,
                MovementType = movement.MovementType,
                Quantity = -movement.Quantity,
                ReferenceId = movement.MovementId,
                ReferenceType = "stock_movement_reversal",
                Notes = $"Reversal for stock movement {movement.MovementId}",
                CreatedAt = now
            });

            _context.StockLedgers.Add(new StockLedger
            {
                ProductId = movement.ProductId,
                VariantId = movement.VariantId,
                WarehouseId = movement.WarehouseId,
                OpeningQty = openingQty,
                ChangeQty = reversalQty,
                ClosingQty = closingQty,
                TransactionType = "STOCK_MOVEMENT_REVERSAL",
                TransactionId = movement.MovementId,
                CreatedAt = now
            });

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Ok("Stock movement reversed successfully");
        }
    }
}
