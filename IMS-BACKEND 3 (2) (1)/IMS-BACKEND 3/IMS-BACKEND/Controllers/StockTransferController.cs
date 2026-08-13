using System.Security.Claims;
using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/stock-transfers")]
    public class StockTransferController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StockTransferController> _logger;

        public StockTransferController(
            AppDbContext context,
            ILogger<StockTransferController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.StockTransfers.AsNoTracking().OrderByDescending(x => x.TransferId).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var transfer = _context.StockTransfers.AsNoTracking().FirstOrDefault(item => item.TransferId == id);

            if (transfer == null)
                return NotFound();

            return Ok(transfer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StockTransferDto dto, CancellationToken cancellationToken)
        {
            if (dto.ProductId <= 0 || dto.FromWarehouseId <= 0 || dto.ToWarehouseId <= 0)
                return ValidationError("Product, source warehouse, and destination warehouse are required.");

            if (dto.FromWarehouseId == dto.ToWarehouseId)
                return ValidationError("Source and destination warehouses must be different.");

            if (dto.Quantity <= 0)
                return ValidationError("Quantity must be greater than zero.");

            var productExists = await _context.Products
                .AsNoTracking()
                .AnyAsync(item => item.ProductId == dto.ProductId && !item.IsDeleted, cancellationToken);

            if (!productExists)
                return ValidationError("Invalid product.");

            var fromWarehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.WarehouseId == dto.FromWarehouseId, cancellationToken);

            if (fromWarehouse == null)
                return ValidationError("Source warehouse does not exist.");

            var toWarehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.WarehouseId == dto.ToWarehouseId, cancellationToken);

            if (toWarehouse == null)
                return ValidationError("Destination warehouse does not exist.");

            var sourceStock = await _context.Stocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId == dto.ProductId &&
                    s.WarehouseId == dto.FromWarehouseId &&
                    s.VariantId == dto.VariantId,
                    cancellationToken);

            if (sourceStock == null)
                return ValidationError("Source warehouse stock not found.");

            if (sourceStock.Quantity < dto.Quantity)
                return ValidationError($"Insufficient stock in source warehouse. Available: {sourceStock.Quantity}.");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var transfer = new StockTransfer
            {
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                TransferDate = dto.TransferDate == default ? DateTime.UtcNow : dto.TransferDate,
                Status = dto.Status ?? "completed"
            };

            _context.StockTransfers.Add(transfer);
            await _context.SaveChangesAsync(cancellationToken);

            var transferItem = new StockTransferItem
            {
                TransferId = transfer.TransferId,
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                Quantity = dto.Quantity
            };

            _context.StockTransferItems.Add(transferItem);

            var sourceOpeningQty = sourceStock.Quantity;
            sourceStock.Quantity -= dto.Quantity;

            if (sourceStock.Quantity < 0)
                return ValidationError("Warehouse transfer would create negative source stock.");

            var sourceBinStocks = await _context.BinStocks
                .Where(item =>
                    item.ProductId == dto.ProductId &&
                    item.WarehouseId == dto.FromWarehouseId &&
                    item.VariantId == dto.VariantId &&
                    item.Quantity > 0)
                .OrderBy(item => item.BinId)
                .ToListAsync(cancellationToken);

            var allocatedSourceQuantity = sourceBinStocks.Sum(item => item.Quantity);
            var unallocatedSourceQuantity = Math.Max(0, sourceOpeningQty - allocatedSourceQuantity);
            var remainingTransferQuantity = dto.Quantity;
            var removedFromUnallocatedStock = Math.Min(unallocatedSourceQuantity, remainingTransferQuantity);
            remainingTransferQuantity -= removedFromUnallocatedStock;
            var binPickNotes = new List<string>();

            // Root cause fix: warehouse transfer previously checked only unallocated stock,
            // so fully put-away stock could never move. Source bins are now depleted when
            // needed, keeping the warehouse-level Stock Register and physical Bin Stock aligned.
            foreach (var binStock in sourceBinStocks)
            {
                if (remainingTransferQuantity <= 0)
                {
                    break;
                }

                var pickedQuantity = Math.Min(binStock.Quantity, remainingTransferQuantity);
                binStock.Quantity -= pickedQuantity;
                remainingTransferQuantity -= pickedQuantity;
                binPickNotes.Add($"Bin {binStock.BinId}: {pickedQuantity}");

                if (binStock.Quantity <= 0)
                {
                    _context.BinStocks.Remove(binStock);
                }
            }

            if (remainingTransferQuantity > 0)
                return ValidationError("Warehouse transfer could not allocate enough source stock from the warehouse and bins.");

            var destinationStock = await _context.Stocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId == dto.ProductId &&
                    s.WarehouseId == dto.ToWarehouseId &&
                    s.VariantId == dto.VariantId,
                    cancellationToken);

            var destinationOpeningQty = destinationStock?.Quantity ?? 0;

            if (destinationStock == null)
            {
                destinationStock = new Stock
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.ToWarehouseId,
                    Quantity = dto.Quantity,
                    ReservedQuantity = 0
                };

                _context.Stocks.Add(destinationStock);
            }
            else
            {
                destinationStock.Quantity += dto.Quantity;
            }

            _context.StockMovements.AddRange(
                new StockMovement
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.FromWarehouseId,
                    MovementType = "Transfer Out",
                    Quantity = dto.Quantity,
                    ReferenceId = transfer.TransferId,
                    ReferenceType = "Warehouse Transfer",
                    Notes = binPickNotes.Count == 0
                        ? $"Transferred unallocated stock to Warehouse {dto.ToWarehouseId}"
                        : $"Transferred to Warehouse {dto.ToWarehouseId}. Picked from source bins: {string.Join(", ", binPickNotes)}",
                    CreatedAt = DateTime.UtcNow
                },
                new StockMovement
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.ToWarehouseId,
                    MovementType = "Transfer In",
                    Quantity = dto.Quantity,
                    ReferenceId = transfer.TransferId,
                    ReferenceType = "Warehouse Transfer",
                    Notes = $"Received from Warehouse {dto.FromWarehouseId} as unallocated stock pending putaway",
                    CreatedAt = DateTime.UtcNow
                });

            _context.StockLedgers.AddRange(
                new StockLedger
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.FromWarehouseId,
                    OpeningQty = sourceOpeningQty,
                    ChangeQty = -dto.Quantity,
                    ClosingQty = sourceStock.Quantity,
                    TransactionType = "Warehouse Transfer Out",
                    TransactionId = transfer.TransferId,
                    CreatedAt = DateTime.UtcNow
                },
                new StockLedger
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = dto.ToWarehouseId,
                    OpeningQty = destinationOpeningQty,
                    ChangeQty = dto.Quantity,
                    ClosingQty = destinationStock.Quantity,
                    TransactionType = "Warehouse Transfer In",
                    TransactionId = transfer.TransferId,
                    CreatedAt = DateTime.UtcNow
                });

            _context.WarehouseTransferAudits.Add(new WarehouseTransferAudit
            {
                TransferId = transfer.TransferId,
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                Quantity = dto.Quantity,
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Warehouse transfer completed. TransferId={TransferId}, ProductId={ProductId}, VariantId={VariantId}, FromWarehouseId={FromWarehouseId}, ToWarehouseId={ToWarehouseId}, Quantity={Quantity}, SourceOpeningQty={SourceOpeningQty}, SourceClosingQty={SourceClosingQty}, DestinationOpeningQty={DestinationOpeningQty}, DestinationClosingQty={DestinationClosingQty}, RemovedFromUnallocatedStock={RemovedFromUnallocatedStock}, RemovedFromBins={RemovedFromBins}",
                transfer.TransferId,
                dto.ProductId,
                dto.VariantId,
                dto.FromWarehouseId,
                dto.ToWarehouseId,
                dto.Quantity,
                sourceOpeningQty,
                sourceStock.Quantity,
                destinationOpeningQty,
                destinationStock.Quantity,
                removedFromUnallocatedStock,
                dto.Quantity - removedFromUnallocatedStock);

            return Ok(ApiResponse<object>.Ok(new
            {
                Transfer = transfer,
                Item = transferItem,
                SourceOpeningQuantity = sourceOpeningQty,
                SourceClosingQuantity = sourceStock.Quantity,
                DestinationOpeningQuantity = destinationOpeningQty,
                DestinationClosingQuantity = destinationStock.Quantity,
                RemovedFromUnallocatedStock = removedFromUnallocatedStock,
                RemovedFromBins = dto.Quantity - removedFromUnallocatedStock
            }, "Warehouse transfer completed successfully.", HttpContext.TraceIdentifier));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, StockTransferDto dto)
        {
            var transfer = _context.StockTransfers.Find(id);

            if (transfer == null)
                return NotFound();

            transfer.FromWarehouseId = dto.FromWarehouseId;
            transfer.ToWarehouseId = dto.ToWarehouseId;
            transfer.TransferDate = dto.TransferDate;
            transfer.Status = dto.Status;

            _context.SaveChanges();

            return Ok(transfer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var transfer = _context.StockTransfers.Find(id);

            if (transfer == null)
                return NotFound();

            _context.StockTransfers.Remove(transfer);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }

        private int? GetCurrentUserId()
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("UserId") ??
                User.FindFirstValue("userId") ??
                User.FindFirstValue("sub");

            return int.TryParse(value, out var userId) ? userId : null;
        }

        private string? GetCurrentUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ??
                User.FindFirstValue("name") ??
                User.FindFirstValue(ClaimTypes.Email);
        }

        private BadRequestObjectResult ValidationError(string message)
        {
            return BadRequest(ApiResponse<object>.Fail(message, traceId: HttpContext.TraceIdentifier));
        }
    }
}
