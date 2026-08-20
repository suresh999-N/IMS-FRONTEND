using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs.PurchaseReturns;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReturnsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<PurchaseReturnsController> _logger;

        public PurchaseReturnsController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<PurchaseReturnsController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // =========================================================
        // 1. GET SUPPLIERS DROPDOWN
        // =========================================================
        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliersDropdown()
        {
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    supplierCode = s.SupplierCode,
                    name = s.Name
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(suppliers, "Suppliers retrieved successfully."));
        }

        // =========================================================
        // 2. GET GRNs FOR SUPPLIER
        // =========================================================
        [HttpGet("grns")]
        public async Task<IActionResult> GetGrnsForSupplier([FromQuery] int supplierId)
        {
            if (supplierId <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "A valid Supplier ID is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var grns = await (
                from grn in _context.GoodsReceipts.AsNoTracking()
                join s in _context.Suppliers.AsNoTracking() on grn.SupplierId equals s.SupplierId into suppliers
                from s in suppliers.DefaultIfEmpty()
                where grn.SupplierId == supplierId && !grn.IsCancelled
                orderby grn.GrnId descending
                select new GrnForReturnResponseDto
                {
                    GrnId = grn.GrnId,
                    GrnNumber = grn.GrnNumber,
                    SupplierId = grn.SupplierId,
                    SupplierName = s != null ? s.Name : null,
                    ReceiptDate = grn.ReceiptDate,
                    Status = grn.Status
                }
            ).ToListAsync();

            return Ok(ApiResponse<List<GrnForReturnResponseDto>>.Ok(grns, "GRNs retrieved successfully."));
        }

        // =========================================================
        // 3. GET GRN ITEMS FOR RETURN
        // =========================================================
        [HttpGet("grn/{grnId}/items")]
        public async Task<IActionResult> GetGrnItemsForReturn(int grnId)
        {
            var grn = await _context.GoodsReceipts
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GrnId == grnId && !g.IsCancelled);

            if (grn == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Goods receipt record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Fetch GRN items
            var grnItems = await (
                from item in _context.GoodsReceiptItems.AsNoTracking()
                join p in _context.Products.AsNoTracking() on item.ProductId equals p.ProductId into products
                from p in products.DefaultIfEmpty()
                join v in _context.ProductVariants.AsNoTracking() on item.VariantId equals v.VariantId into variants
                from v in variants.DefaultIfEmpty()
                where item.GrnId == grnId && item.ProductId.HasValue
                select new
                {
                    ProductId = item.ProductId!.Value,
                    ProductName = p != null ? p.Name : null,
                    ProductSku = p != null ? p.SKU : null,
                    VariantId = item.VariantId,
                    VariantName = v != null ? v.VariantName : null,
                    ReceivedQuantity = item.QuantityReceived ?? 0m,
                    Price = item.Price ?? 0m
                }
            ).ToListAsync();

            // Fetch previous returns for this GRN to compute remaining returnable quantities
            var previousReturns = await (
                from ret in _context.PurchaseReturns.AsNoTracking()
                join retItem in _context.PurchaseReturnItems.AsNoTracking() on ret.PurchaseReturnId equals retItem.PurchaseReturnId
                where ret.GrnId == grnId
                select new
                {
                    retItem.ProductId,
                    retItem.VariantId,
                    retItem.ReturnQuantity
                }
            ).ToListAsync();

            var result = new List<GrnItemForReturnResponseDto>();

            foreach (var item in grnItems)
            {
                var prevQty = previousReturns
                    .Where(r => r.ProductId == item.ProductId && r.VariantId == item.VariantId)
                    .Sum(r => r.ReturnQuantity);

                var remaining = Math.Max(0m, item.ReceivedQuantity - prevQty);

                result.Add(new GrnItemForReturnResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductSku = item.ProductSku,
                    VariantId = item.VariantId,
                    VariantName = item.VariantName,
                    ReceivedQuantity = item.ReceivedQuantity,
                    PreviousReturnedQuantity = prevQty,
                    RemainingReturnableQuantity = remaining,
                    Price = item.Price
                });
            }

            return Ok(ApiResponse<List<GrnItemForReturnResponseDto>>.Ok(result, "GRN items retrieved successfully."));
        }

        // =========================================================
        // 4. CREATE PURCHASE RETURN
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseReturn([FromBody] CreatePurchaseReturnDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid input data.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.Items == null || dto.Items.Count == 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "At least one return item is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var supplier = await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SupplierId == dto.SupplierId && !s.IsDeleted);

            if (supplier == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected supplier was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var grn = await _context.GoodsReceipts
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GrnId == dto.GrnId && !g.IsCancelled);

            if (grn == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected Goods Receipt (GRN) was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (grn.SupplierId != dto.SupplierId)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected Goods Receipt (GRN) does not belong to the selected supplier.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Fetch GRN items for validation & authoritative price
            var grnItems = await _context.GoodsReceiptItems
                .AsNoTracking()
                .Where(gi => gi.GrnId == dto.GrnId && gi.ProductId.HasValue)
                .ToListAsync();

            // Fetch previous returns to calculate remaining returnable quantity per item
            var previousReturnItems = await (
                from ret in _context.PurchaseReturns.AsNoTracking()
                join retItem in _context.PurchaseReturnItems.AsNoTracking() on ret.PurchaseReturnId equals retItem.PurchaseReturnId
                where ret.GrnId == dto.GrnId
                select new
                {
                    retItem.ProductId,
                    retItem.VariantId,
                    retItem.ReturnQuantity
                }
            ).ToListAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var returnNumber = await GenerateUniqueReturnNumberAsync();
                var now = DateTime.UtcNow;

                var purchaseReturn = new PurchaseReturn
                {
                    ReturnNumber = returnNumber,
                    SupplierId = (int?)dto.SupplierId,
                    GrnId = (int?)dto.GrnId,
                    ReturnDate = dto.ReturnDate == default ? now.Date : dto.ReturnDate,
                    Reason = dto.Reason.Trim(),
                    Status = "Completed",
                    CreatedAt = now
                };

                _context.PurchaseReturns.Add(purchaseReturn);
                await _context.SaveChangesAsync();

                decimal totalReturnAmount = 0m;
                var createdItems = new List<PurchaseReturnItem>();

                foreach (var itemDto in dto.Items)
                {
                    if (itemDto.ReturnQuantity <= 0)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Return quantity for Product ID {itemDto.ProductId} must be greater than zero.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var matchingGrnItem = grnItems.FirstOrDefault(gi =>
                        gi.ProductId == itemDto.ProductId &&
                        gi.VariantId == itemDto.VariantId);

                    if (matchingGrnItem == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Product ID {itemDto.ProductId} (Variant ID: {itemDto.VariantId?.ToString() ?? "N/A"}) is not present in the selected GRN.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var receivedQty = matchingGrnItem.QuantityReceived ?? 0m;
                    var price = matchingGrnItem.Price ?? 0m;

                    var prevReturnedQty = previousReturnItems
                        .Where(r => r.ProductId == itemDto.ProductId && r.VariantId == itemDto.VariantId)
                        .Sum(r => r.ReturnQuantity);

                    var remainingReturnableQty = Math.Max(0m, receivedQty - prevReturnedQty);

                    if (itemDto.ReturnQuantity > remainingReturnableQty)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Return quantity ({itemDto.ReturnQuantity}) for Product ID {itemDto.ProductId} exceeds remaining returnable quantity ({remainingReturnableQty}).",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var lineTotal = itemDto.ReturnQuantity * price;
                    totalReturnAmount += lineTotal;

                    var returnItem = new PurchaseReturnItem
                    {
                        PurchaseReturnId = purchaseReturn.PurchaseReturnId,
                        ProductId = itemDto.ProductId,
                        VariantId = itemDto.VariantId,
                        ReceivedQuantity = receivedQty,
                        ReturnQuantity = itemDto.ReturnQuantity,
                        Price = price,
                        Total = lineTotal,
                        CreatedAt = now
                    };

                    _context.PurchaseReturnItems.Add(returnItem);
                    createdItems.Add(returnItem);

                    // Update stock movements and stock balance if warehouse is present
                    if (grn.WarehouseId.HasValue)
                    {
                        _context.StockMovements.Add(new StockMovement
                        {
                            ProductId = itemDto.ProductId,
                            VariantId = itemDto.VariantId,
                            WarehouseId = grn.WarehouseId.Value,
                            MovementType = "PURCHASE_RETURN",
                            Quantity = itemDto.ReturnQuantity,
                            ReferenceId = purchaseReturn.PurchaseReturnId,
                            ReferenceType = "purchase_return",
                            Notes = $"Stock reduced for purchase return {returnNumber}",
                            CreatedAt = now
                        });

                        var stockRow = await _context.Stocks
                            .FirstOrDefaultAsync(s =>
                                s.ProductId == itemDto.ProductId &&
                                s.VariantId == itemDto.VariantId &&
                                s.WarehouseId == grn.WarehouseId.Value);

                        if (stockRow != null)
                        {
                            stockRow.Quantity = Math.Max(0m, stockRow.Quantity - itemDto.ReturnQuantity);
                        }
                    }
                }

                purchaseReturn.TotalReturnAmount = totalReturnAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Log audit trail
                await _auditLogService.LogAsync(
                    "Create",
                    "Purchase Return",
                    purchaseReturn.PurchaseReturnId,
                    $"Purchase Return {returnNumber} created for Supplier {supplier.Name}",
                    "purchase_returns");

                var responseDto = await BuildPurchaseReturnResponseDtoAsync(purchaseReturn.PurchaseReturnId);

                return Ok(ApiResponse<PurchaseReturnResponseDto>.Ok(
                    responseDto,
                    "Purchase return created successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create purchase return for GRN {GrnId}", dto.GrnId);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An error occurred while processing the purchase return.",
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        // =========================================================
        // 5. GET PURCHASE RETURNS LIST
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetPurchaseReturns([FromQuery] string? search, [FromQuery] int? supplierId)
        {
            var query = _context.PurchaseReturns
                .AsNoTracking()
                .AsQueryable();

            if (supplierId.HasValue && supplierId.Value > 0)
            {
                query = query.Where(r => r.SupplierId == supplierId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(r =>
                    r.ReturnNumber.ToLower().Contains(term) ||
                    r.Reason.ToLower().Contains(term) ||
                    (r.Supplier != null && r.Supplier.Name != null && r.Supplier.Name.ToLower().Contains(term)) ||
                    (r.GoodsReceipt != null && r.GoodsReceipt.GrnNumber.ToLower().Contains(term)));
            }

            var list = await (
                from r in query
                join s in _context.Suppliers.AsNoTracking() on r.SupplierId equals (int?)s.SupplierId into suppliers
                from s in suppliers.DefaultIfEmpty()
                join grn in _context.GoodsReceipts.AsNoTracking() on r.GrnId equals (int?)grn.GrnId into grns
                from grn in grns.DefaultIfEmpty()
                orderby r.PurchaseReturnId descending
                select new PurchaseReturnResponseDto
                {
                    PurchaseReturnId = r.PurchaseReturnId,
                    ReturnNumber = r.ReturnNumber,
                    SupplierId = r.SupplierId,
                    SupplierName = s != null ? s.Name : null,
                    GrnId = r.GrnId,
                    GrnNumber = grn != null ? grn.GrnNumber : null,
                    ReturnDate = r.ReturnDate,
                    Reason = r.Reason,
                    TotalReturnAmount = r.TotalReturnAmount,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                }
            ).ToListAsync();

            return Ok(ApiResponse<List<PurchaseReturnResponseDto>>.Ok(list, "Purchase returns retrieved successfully."));
        }

        // =========================================================
        // 6. GET PURCHASE RETURN BY ID
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseReturnById(int id)
        {
            var responseDto = await BuildPurchaseReturnResponseDtoAsync(id);

            if (responseDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Purchase return record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<PurchaseReturnResponseDto>.Ok(responseDto, "Purchase return retrieved successfully."));
        }

        // =========================================================
        // 7. UPDATE PURCHASE RETURN
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseReturn(int id, [FromBody] UpdatePurchaseReturnDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid input data.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var purchaseReturn = await _context.PurchaseReturns
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.PurchaseReturnId == id);

            if (purchaseReturn == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Purchase return record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (string.Equals(purchaseReturn.Status, "Finalized", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Finalized purchase returns cannot be edited.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var supplier = await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SupplierId == dto.SupplierId && !s.IsDeleted);

            if (supplier == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected supplier was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var grn = await _context.GoodsReceipts
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GrnId == dto.GrnId && !g.IsCancelled);

            if (grn == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected Goods Receipt (GRN) was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (grn.SupplierId != dto.SupplierId)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected Goods Receipt (GRN) does not belong to the selected supplier.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var grnItems = await _context.GoodsReceiptItems
                .AsNoTracking()
                .Where(gi => gi.GrnId == dto.GrnId && gi.ProductId.HasValue)
                .ToListAsync();

            var otherReturnItems = await (
                from ret in _context.PurchaseReturns.AsNoTracking()
                join retItem in _context.PurchaseReturnItems.AsNoTracking() on ret.PurchaseReturnId equals retItem.PurchaseReturnId
                where ret.GrnId == dto.GrnId && ret.PurchaseReturnId != id
                select new
                {
                    retItem.ProductId,
                    retItem.VariantId,
                    retItem.ReturnQuantity
                }
            ).ToListAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                purchaseReturn.SupplierId = (int?)dto.SupplierId;
                purchaseReturn.GrnId = (int?)dto.GrnId;
                purchaseReturn.ReturnDate = dto.ReturnDate == default ? purchaseReturn.ReturnDate : dto.ReturnDate;
                purchaseReturn.Reason = dto.Reason.Trim();
                purchaseReturn.UpdatedAt = now;

                _context.PurchaseReturnItems.RemoveRange(purchaseReturn.Items);

                decimal totalReturnAmount = 0m;

                foreach (var itemDto in dto.Items)
                {
                    if (itemDto.ReturnQuantity <= 0)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Return quantity for Product ID {itemDto.ProductId} must be greater than zero.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var matchingGrnItem = grnItems.FirstOrDefault(gi =>
                        gi.ProductId == itemDto.ProductId &&
                        gi.VariantId == itemDto.VariantId);

                    if (matchingGrnItem == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Product ID {itemDto.ProductId} (Variant ID: {itemDto.VariantId?.ToString() ?? "N/A"}) is not present in the selected GRN.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var receivedQty = matchingGrnItem.QuantityReceived ?? 0m;
                    var price = matchingGrnItem.Price ?? 0m;

                    var prevReturnedQty = otherReturnItems
                        .Where(r => r.ProductId == itemDto.ProductId && r.VariantId == itemDto.VariantId)
                        .Sum(r => r.ReturnQuantity);

                    var remainingReturnableQty = Math.Max(0m, receivedQty - prevReturnedQty);

                    if (itemDto.ReturnQuantity > remainingReturnableQty)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Return quantity ({itemDto.ReturnQuantity}) for Product ID {itemDto.ProductId} exceeds remaining returnable quantity ({remainingReturnableQty}).",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var lineTotal = itemDto.ReturnQuantity * price;
                    totalReturnAmount += lineTotal;

                    _context.PurchaseReturnItems.Add(new PurchaseReturnItem
                    {
                        PurchaseReturnId = purchaseReturn.PurchaseReturnId,
                        ProductId = itemDto.ProductId,
                        VariantId = itemDto.VariantId,
                        ReceivedQuantity = receivedQty,
                        ReturnQuantity = itemDto.ReturnQuantity,
                        Price = price,
                        Total = lineTotal,
                        CreatedAt = now
                    });
                }

                purchaseReturn.TotalReturnAmount = totalReturnAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _auditLogService.LogAsync(
                    "Update",
                    "Purchase Return",
                    purchaseReturn.PurchaseReturnId,
                    $"Purchase Return {purchaseReturn.ReturnNumber} updated",
                    "purchase_returns");

                var responseDto = await BuildPurchaseReturnResponseDtoAsync(purchaseReturn.PurchaseReturnId);

                return Ok(ApiResponse<PurchaseReturnResponseDto>.Ok(
                    responseDto,
                    "Purchase return updated successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to update purchase return ID {PurchaseReturnId}", id);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An error occurred while updating the purchase return.",
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        // =========================================================
        // 8. DELETE PURCHASE RETURN
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseReturn(int id)
        {
            var purchaseReturn = await _context.PurchaseReturns
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.PurchaseReturnId == id);

            if (purchaseReturn == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Purchase return record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (string.Equals(purchaseReturn.Status, "Finalized", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Finalized purchase returns cannot be deleted.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.PurchaseReturnItems.RemoveRange(purchaseReturn.Items);
                _context.PurchaseReturns.Remove(purchaseReturn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _auditLogService.LogAsync(
                    "Delete",
                    "Purchase Return",
                    id,
                    $"Purchase Return {purchaseReturn.ReturnNumber} deleted",
                    "purchase_returns");

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Purchase return deleted successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to delete purchase return ID {PurchaseReturnId}", id);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An error occurred while deleting the purchase return.",
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        // =========================================================
        // PRIVATE HELPER METHODS
        // =========================================================

        private async Task<string> GenerateUniqueReturnNumberAsync()
        {
            var maxId = await _context.PurchaseReturns.MaxAsync(r => (int?)r.PurchaseReturnId) ?? 0;
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var candidate = $"PRR-{(maxId + 1 + attempt):D6}";

                var exists = await _context.PurchaseReturns
                    .AnyAsync(r => r.ReturnNumber == candidate);

                if (!exists)
                {
                    return candidate;
                }
            }

            return $"PRR-{DateTime.UtcNow.Ticks.ToString()[^6..]}";
        }

        private static string GenerateRandomAlphaNumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }

            return new string(result);
        }

        private async Task<PurchaseReturnResponseDto?> BuildPurchaseReturnResponseDtoAsync(int purchaseReturnId)
        {
            var header = await (
                from r in _context.PurchaseReturns.AsNoTracking()
                join s in _context.Suppliers.AsNoTracking() on r.SupplierId equals (int?)s.SupplierId into suppliers
                from s in suppliers.DefaultIfEmpty()
                join grn in _context.GoodsReceipts.AsNoTracking() on r.GrnId equals (int?)grn.GrnId into grns
                from grn in grns.DefaultIfEmpty()
                where r.PurchaseReturnId == purchaseReturnId
                select new
                {
                    r.PurchaseReturnId,
                    r.ReturnNumber,
                    r.SupplierId,
                    SupplierName = s != null ? s.Name : null,
                    r.GrnId,
                    GrnNumber = grn != null ? grn.GrnNumber : null,
                    r.ReturnDate,
                    r.Reason,
                    r.TotalReturnAmount,
                    r.Status,
                    r.CreatedAt
                }
            ).FirstOrDefaultAsync();

            if (header == null)
            {
                return null;
            }

            var items = await (
                from item in _context.PurchaseReturnItems.AsNoTracking()
                join p in _context.Products.AsNoTracking() on item.ProductId equals p.ProductId into products
                from p in products.DefaultIfEmpty()
                join v in _context.ProductVariants.AsNoTracking() on item.VariantId equals v.VariantId into variants
                from v in variants.DefaultIfEmpty()
                where item.PurchaseReturnId == purchaseReturnId
                select new PurchaseReturnItemResponseDto
                {
                    PurchaseReturnItemId = item.PurchaseReturnItemId,
                    ProductId = item.ProductId,
                    ProductName = p != null ? p.Name : null,
                    ProductSku = p != null ? p.SKU : null,
                    VariantId = item.VariantId,
                    VariantName = v != null ? v.VariantName : null,
                    ReceivedQuantity = item.ReceivedQuantity,
                    ReturnQuantity = item.ReturnQuantity,
                    Price = item.Price,
                    Total = item.Total
                }
            ).ToListAsync();

            return new PurchaseReturnResponseDto
            {
                PurchaseReturnId = header.PurchaseReturnId,
                ReturnNumber = header.ReturnNumber,
                SupplierId = header.SupplierId,
                SupplierName = header.SupplierName,
                GrnId = header.GrnId,
                GrnNumber = header.GrnNumber,
                ReturnDate = header.ReturnDate,
                Reason = header.Reason,
                TotalReturnAmount = header.TotalReturnAmount,
                Status = header.Status,
                CreatedAt = header.CreatedAt,
                Items = items
            };
        }
    }
}
