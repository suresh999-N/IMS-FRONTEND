using System.Security.Claims;
using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/bin-transfers")]
    public class BinTransfersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BinTransfersController> _logger;

        public BinTransfersController(
            AppDbContext context,
            ILogger<BinTransfersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(BinTransferDto dto, CancellationToken cancellationToken)
        {
            if (dto.ProductId <= 0 || dto.FromBinId <= 0 || dto.ToBinId <= 0)
                return ValidationError("Product, source bin, and destination bin are required.");

            if (dto.FromBinId == dto.ToBinId)
                return ValidationError("Source and destination bins must be different.");

            if (dto.Quantity <= 0)
                return ValidationError("Quantity must be greater than zero.");

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && !x.IsDeleted, cancellationToken);

            if (product == null)
                return ValidationError("Invalid product.");

            var sourceBin = await _context.Bins
                .FirstOrDefaultAsync(x => x.BinId == dto.FromBinId, cancellationToken);

            if (sourceBin == null)
                return ValidationError("Source bin does not exist.");

            var destinationBin = await _context.Bins
                .FirstOrDefaultAsync(x => x.BinId == dto.ToBinId, cancellationToken);

            if (destinationBin == null)
                return ValidationError("Destination bin does not exist.");

            if (sourceBin.WarehouseId != destinationBin.WarehouseId)
                return ValidationError("Bin transfers must stay within the same warehouse. Use Warehouse Transfer for cross-warehouse movement.");

            var sourceBinStock = await _context.BinStocks
                .FirstOrDefaultAsync(x =>
                    x.ProductId == dto.ProductId &&
                    x.BinId == dto.FromBinId &&
                    x.VariantId == dto.VariantId,
                    cancellationToken);

            if (sourceBinStock == null)
                return ValidationError("Product does not exist in the selected source bin.");

            if (sourceBinStock.Quantity < dto.Quantity)
                return ValidationError("Insufficient stock in source bin.");

            var destinationBinQuantity = await _context.BinStocks
                .Where(x => x.BinId == dto.ToBinId)
                .SumAsync(x => (decimal?)x.Quantity, cancellationToken) ?? 0;

            if (destinationBin.Capacity.HasValue && destinationBinQuantity + dto.Quantity > destinationBin.Capacity.Value)
                return ValidationError($"Quantity exceeds destination bin capacity. Available bin capacity: {destinationBin.Capacity.Value - destinationBinQuantity}.");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogInformation(
                    "Warehouse bin transfer started. ProductId={ProductId}, VariantId={VariantId}, WarehouseId={WarehouseId}, FromBinId={FromBinId}, ToBinId={ToBinId}, Quantity={Quantity}, UserId={UserId}",
                    dto.ProductId,
                    dto.VariantId,
                    sourceBin.WarehouseId,
                    dto.FromBinId,
                    dto.ToBinId,
                    dto.Quantity,
                    GetCurrentUserId());

                var destinationBinStock = await _context.BinStocks
                    .FirstOrDefaultAsync(x =>
                        x.ProductId == dto.ProductId &&
                        x.BinId == dto.ToBinId &&
                        x.VariantId == dto.VariantId,
                        cancellationToken);

                sourceBinStock.Quantity -= dto.Quantity;

                if (destinationBinStock == null)
                {
                    destinationBinStock = new BinStock
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = destinationBin.WarehouseId,
                        BinId = dto.ToBinId,
                        Quantity = dto.Quantity
                    };

                    _context.BinStocks.Add(destinationBinStock);
                }
                else
                {
                    destinationBinStock.Quantity += dto.Quantity;
                }

                var sourceClosingQty = Math.Max(0, sourceBinStock.Quantity);
                var sourceOpeningQty = sourceClosingQty + dto.Quantity;
                var destinationOpeningQty = destinationBinStock.Quantity - dto.Quantity;

                if (sourceBinStock.Quantity <= 0)
                {
                    _context.BinStocks.Remove(sourceBinStock);
                }

                _context.StockMovements.AddRange(
                    new StockMovement
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = sourceBin.WarehouseId,
                        MovementType = "Bin Transfer Out",
                        Quantity = dto.Quantity,
                        ReferenceType = "bin_transfer",
                        Notes = $"Transferred from Bin {dto.FromBinId} to Bin {dto.ToBinId}",
                        CreatedAt = DateTime.UtcNow
                    },
                    new StockMovement
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = destinationBin.WarehouseId,
                        MovementType = "Bin Transfer In",
                        Quantity = dto.Quantity,
                        ReferenceType = "bin_transfer",
                        Notes = $"Received into Bin {dto.ToBinId} from Bin {dto.FromBinId}",
                        CreatedAt = DateTime.UtcNow
                    });

                _context.StockLedgers.AddRange(
                    new StockLedger
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = sourceBin.WarehouseId,
                        OpeningQty = sourceOpeningQty,
                        ChangeQty = -dto.Quantity,
                        ClosingQty = sourceClosingQty,
                        TransactionType = "Bin Transfer Out",
                        CreatedAt = DateTime.UtcNow
                    },
                    new StockLedger
                    {
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        WarehouseId = destinationBin.WarehouseId,
                        OpeningQty = destinationOpeningQty,
                        ChangeQty = dto.Quantity,
                        ClosingQty = destinationBinStock.Quantity,
                        TransactionType = "Bin Transfer In",
                        CreatedAt = DateTime.UtcNow
                    });

                _context.BinTransferAudits.Add(new BinTransferAudit
                {
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    WarehouseId = sourceBin.WarehouseId,
                    FromBinId = dto.FromBinId,
                    ToBinId = dto.ToBinId,
                    Quantity = dto.Quantity,
                    UserId = GetCurrentUserId(),
                    UserName = GetCurrentUserName(),
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Warehouse bin transfer completed. ProductId={ProductId}, VariantId={VariantId}, WarehouseId={WarehouseId}, FromBinId={FromBinId}, ToBinId={ToBinId}, Quantity={Quantity}, SourceClosingQty={SourceClosingQty}, DestinationClosingQty={DestinationClosingQty}",
                    dto.ProductId,
                    dto.VariantId,
                    sourceBin.WarehouseId,
                    dto.FromBinId,
                    dto.ToBinId,
                    dto.Quantity,
                    sourceClosingQty,
                    destinationBinStock.Quantity);

                return Ok(ApiResponse<object>.Ok(new
                {
                    dto.ProductId,
                    dto.VariantId,
                    WarehouseId = sourceBin.WarehouseId,
                    dto.FromBinId,
                    dto.ToBinId,
                    dto.Quantity,
                    SourceClosingQuantity = sourceClosingQty,
                    DestinationClosingQuantity = destinationBinStock.Quantity
                }, "Bin transfer completed successfully.", HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(
                    exception,
                    "Warehouse bin transfer database failure. ProductId={ProductId}, VariantId={VariantId}, FromBinId={FromBinId}, ToBinId={ToBinId}, Quantity={Quantity}, TraceId={TraceId}",
                    dto.ProductId,
                    dto.VariantId,
                    dto.FromBinId,
                    dto.ToBinId,
                    dto.Quantity,
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        "Bin transfer could not be saved. Verify the warehouse audit migration has been applied and try again.",
                        traceId: HttpContext.TraceIdentifier));
            }
        }

        private BadRequestObjectResult ValidationError(string message)
        {
            return BadRequest(ApiResponse<object>.Fail(message, traceId: HttpContext.TraceIdentifier));
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
    }
}
