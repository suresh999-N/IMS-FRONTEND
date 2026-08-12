using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<PurchaseOrdersController> _logger;

        public PurchaseOrdersController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<PurchaseOrdersController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrders()
        {
            var data = await (
                from purchaseOrder in _context.PurchaseOrders.AsNoTracking()
                join supplier in _context.Suppliers.AsNoTracking()
                    on purchaseOrder.SupplierId equals supplier.SupplierId into suppliers
                from supplier in suppliers.DefaultIfEmpty()
                join purchaseItem in _context.PurchaseOrderItems.AsNoTracking()
                    on purchaseOrder.PoId equals purchaseItem.PoId into purchaseItems
                from purchaseItem in purchaseItems.DefaultIfEmpty()
                join product in _context.Products.AsNoTracking()
                    on purchaseItem.ProductId equals product.ProductId into products
                from product in products.DefaultIfEmpty()
                where !purchaseOrder.IsCancelled
                select new
                {
                    Id = purchaseOrder.PoId,
                    purchaseOrder.PoId,
                    purchaseOrder.PoNumber,
                    purchaseOrder.SupplierId,
                    Supplier = supplier != null ? supplier.Name : null,
                    SupplierName = supplier != null ? supplier.Name : null,
                    ProductId = purchaseItem != null ? purchaseItem.ProductId : null,
                    ProductName = product != null ? product.Name : null,
                    ProductSku = product != null ? product.SKU : null,
                    VariantId = purchaseItem != null ? purchaseItem.VariantId : null,
                    Quantity = purchaseItem != null ? purchaseItem.Quantity : null,
                    Price = purchaseItem != null ? purchaseItem.Price : null,
                    purchaseOrder.OrderDate,
                    purchaseOrder.ExpectedDate,
                    purchaseOrder.Status,
                    ReceivingStatus = purchaseOrder.ReceivingStatus ??
                        (purchaseOrder.Status == "received"
                            ? "received"
                            : purchaseOrder.Status == "partially_received"
                                ? "partial"
                                : "pending"),
                    purchaseOrder.TotalAmount,
                    purchaseOrder.Notes
                    ,
                    SourceIndentId = _context.PurchaseIndents
                        .AsNoTracking()
                        .Where(indent =>
                            !indent.IsDeleted &&
                            indent.IndentNumber != null &&
                            purchaseOrder.Notes != null &&
                            purchaseOrder.Notes.Contains(indent.IndentNumber))
                        .Select(indent => (int?)indent.PurchaseIndentId)
                        .FirstOrDefault(),
                    IndentNumber = _context.PurchaseIndents
                        .AsNoTracking()
                        .Where(indent =>
                            !indent.IsDeleted &&
                            indent.IndentNumber != null &&
                            purchaseOrder.Notes != null &&
                            purchaseOrder.Notes.Contains(indent.IndentNumber))
                        .Select(indent => indent.IndentNumber)
                        .FirstOrDefault()
                }
            ).ToListAsync();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder(PurchaseOrderDto dto)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Purchase order payload is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.SupplierId <= 0 || dto.ProductId <= 0 || dto.Quantity <= 0 || dto.Price < 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Supplier, product, quantity greater than zero, and non-negative price are required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var supplierExists = await _context.Suppliers
                .AsNoTracking()
                .AnyAsync(item => item.SupplierId == dto.SupplierId);
            var productExists = await _context.Products
                .AsNoTracking()
                .AnyAsync(item => item.ProductId == dto.ProductId && !item.IsDeleted);

            if (!supplierExists || !productExists)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected supplier or product was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var subtotal = dto.Quantity * dto.Price;

            var discountAmount = subtotal * dto.Discount / 100;

            var taxableAmount = subtotal - discountAmount;

            var taxAmount = taxableAmount * dto.Tax / 100;

            var total = taxableAmount + taxAmount;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var po = new PurchaseOrder
                {
                    SupplierId = dto.SupplierId,
                    PoNumber = "PO-" + DateTime.UtcNow.Ticks,
                    OrderDate = dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate,
                    ExpectedDate = dto.ExpectedDate,
                    Status = "pending",
                    ReceivingStatus = "pending",
                    TotalAmount = total,
                    Notes = dto.Notes?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.PurchaseOrders.Add(po);
                await _context.SaveChangesAsync();

                _context.PurchaseOrderItems.Add(new PurchaseOrderItem
                {
                    PoId = po.PoId,
                    ProductId = dto.ProductId,
                    VariantId = dto.VariantId,
                    Quantity = dto.Quantity,
                    Price = dto.Price,
                    Discount = dto.Discount,
                    Tax = dto.Tax,
                    Total = total
                });

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "CREATE_PURCHASE_ORDER",
                    "Purchases",
                    po.PoId,
                    $"Purchase Order {po.PoNumber} created",
                    "purchase_orders");

                await transaction.CommitAsync();

                return Ok(ApiResponse<object>.Ok(
                    new { po.PoId, po.PoNumber },
                    "Purchase order created successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync();
                LogDbUpdateException(exception, "Purchase order create failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
        }


        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePurchaseOrder(
    int id,
    CancellationToken cancellationToken)
        {
            var po = await _context.PurchaseOrders
                .FirstOrDefaultAsync(
                    item => item.PoId == id && !item.IsCancelled,
                    cancellationToken);

            if (po == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Purchase order was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (po.Status == "approved")
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Purchase order is already approved.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (po.Status == "cancelled")
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Cancelled purchase order cannot be approved.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var itemsExist = await _context.PurchaseOrderItems
                .AnyAsync(
                    item => item.PoId == id,
                    cancellationToken);

            if (!itemsExist)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Purchase order has no items.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                po.Status = "approved";
                po.ReceivingStatus = "pending";

                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "APPROVE_PURCHASE_ORDER",
                    "Purchases",
                    po.PoId,
                    $"Purchase Order {po.PoNumber} approved",
                    "purchase_orders",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(ApiResponse<object>.Ok(
                    new
                    {
                        po.PoId,
                        po.PoNumber,
                        po.Status,
                        po.ReceivingStatus
                    },
                    "Purchase order approved successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(
                    exception,
                    "Purchase order approval failed. PoId={PoId}, TraceId={TraceId}",
                    id,
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
            => await CancelPurchaseOrder(id, dto, cancellationToken);

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelPurchaseOrder(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
        {
            var po = await _context.PurchaseOrders
                .FirstOrDefaultAsync(item => item.PoId == id, cancellationToken);

            if (po == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Purchase order was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (po.IsCancelled)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Purchase order is already cancelled.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var hasReceipts = await _context.GoodsReceipts
                .AsNoTracking()
                .AnyAsync(item => item.PoId == id && !item.IsCancelled, cancellationToken);
            var hasPayments = await _context.SupplierPayments
                .AsNoTracking()
                .AnyAsync(item => item.PoId == id && !item.IsCancelled, cancellationToken);

            if (hasReceipts || hasPayments)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Purchase order has receiving or payment history. Reverse those transactions before cancelling the PO.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var now = DateTime.UtcNow;
                po.Status = "cancelled";
                po.ReceivingStatus = "pending";
                po.IsCancelled = true;
                po.CancelledAt = now;
                po.CancellationReason = dto?.Reason;

                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "DELETE_PURCHASE_ORDER",
                    "Purchases",
                    po.PoId,
                    $"Purchase Order {po.PoNumber} cancelled",
                    "purchase_orders",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(ApiResponse<object>.Ok(
                    new { purchaseOrderId = id },
                    "Purchase order cancelled successfully.",
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Purchase order cancel failed.");

                return Conflict(ApiResponse<object>.Fail(
                    GetDeleteDependencyMessage(exception),
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        private async Task DeleteGoodsReceiptDependencies(GoodsReceipt receipt)
        {
            var items = await _context.GoodsReceiptItems
                .Where(item => item.GrnId == receipt.GrnId)
                .ToListAsync();
            var productIds = items
                .Where(item => item.ProductId.HasValue)
                .Select(item => item.ProductId!.Value)
                .Distinct()
                .ToList();
            var variantIds = items
                .Where(item => item.VariantId.HasValue)
                .Select(item => item.VariantId!.Value)
                .Distinct()
                .ToList();

            var stockMovements = await _context.StockMovements
                .Where(item =>
                    (item.ReferenceId == receipt.GrnId && item.ReferenceType == "goods_receipt") ||
                    productIds.Contains(item.ProductId))
                .ToListAsync();

            var stockLedgers = await _context.StockLedgers
                .Where(item =>
                    (item.TransactionId == receipt.GrnId &&
                        (item.TransactionType == "goods_receipt" ||
                         item.TransactionType == "purchase" ||
                         item.TransactionType == "PURCHASE")) ||
                    productIds.Contains(item.ProductId))
                .ToListAsync();

            var stockAdjustmentItems = await _context.StockAdjustmentItems
                .Where(item => productIds.Contains(item.ProductId) &&
                    (!variantIds.Any() || !item.VariantId.HasValue || variantIds.Contains(item.VariantId.Value)))
                .ToListAsync();

            var stockTransferItems = await _context.StockTransferItems
                .Where(item => productIds.Contains(item.ProductId) &&
                    (!variantIds.Any() || !item.VariantId.HasValue || variantIds.Contains(item.VariantId.Value)))
                .ToListAsync();

            var stockAuditItems = await _context.StockAuditItems
                .Where(item => item.ProductId.HasValue && productIds.Contains(item.ProductId.Value))
                .ToListAsync();

            var auditLogs = await _context.AuditLogs
                .Where(item =>
                    (item.RecordId == receipt.GrnId &&
                        (item.TableName == "goods_receipts" ||
                         item.TableName == "goods_receipt_items" ||
                         item.Module == "GoodsReceipts" ||
                         item.Module == "Receiving")) ||
                    (item.RecordId.HasValue && productIds.Contains(item.RecordId.Value) &&
                        (item.TableName == "products" ||
                         item.Module == "Inventory" ||
                         item.Module == "Stock")))
                .ToListAsync();

            _context.StockAdjustmentItems.RemoveRange(stockAdjustmentItems);
            _context.StockTransferItems.RemoveRange(stockTransferItems);
            _context.StockAuditItems.RemoveRange(stockAuditItems);
            _context.StockMovements.RemoveRange(stockMovements);
            _context.StockLedgers.RemoveRange(stockLedgers);
            _context.AuditLogs.RemoveRange(auditLogs);
            await _context.SaveChangesAsync();

            _context.GoodsReceiptItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                if (!item.ProductId.HasValue)
                {
                    continue;
                }

                var quantity = item.QuantityReceived ?? 0;
                var stockRows = await _context.Stocks
                    .Where(stock =>
                        stock.ProductId == item.ProductId.Value &&
                        stock.VariantId == item.VariantId &&
                        stock.WarehouseId == receipt.WarehouseId)
                    .ToListAsync();

                foreach (var stock in stockRows)
                {
                    stock.Quantity = Math.Max(stock.Quantity - quantity, 0);

                    if (stock.Quantity == 0 && stock.ReservedQuantity == 0)
                    {
                        _context.Stocks.Remove(stock);
                    }
                }

                var product = await _context.Products
                    .FirstOrDefaultAsync(product => product.ProductId == item.ProductId.Value);

                if (product != null)
                {
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();

            _context.GoodsReceipts.Remove(receipt);
            await _context.SaveChangesAsync();
        }

        private static string GetDeleteDependencyMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);

            if (detail.Contains("goods_receipt", StringComparison.OrdinalIgnoreCase))
            {
                return "Goods receipt records exist";
            }

            if (detail.Contains("stock", StringComparison.OrdinalIgnoreCase))
            {
                return "Stock register records exist";
            }

            return "Purchase order records exist";
        }

        private static string GetDbUpdateUserMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);

            if (detail.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("constraint", StringComparison.OrdinalIgnoreCase))
            {
                return "Purchase order could not be saved because a selected supplier or product is invalid.";
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
                "{Message} InnerException: {InnerException}. TraceId: {TraceId}",
                message,
                exception.InnerException?.ToString() ?? "No inner exception",
                HttpContext.TraceIdentifier);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PoId == id);

            if (purchaseOrder == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Purchase order not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var items = await (
                from item in _context.PurchaseOrderItems.AsNoTracking()

                join product in _context.Products.AsNoTracking()
                    on item.ProductId equals product.ProductId into productGroup
                from product in productGroup.DefaultIfEmpty()

                join variant in _context.ProductVariants.AsNoTracking()
                    on item.VariantId equals variant.VariantId into variantGroup
                from variant in variantGroup.DefaultIfEmpty()

                where item.PoId == id

                select new
                {
                    item.Id,
                    item.ProductId,
                    ProductName = product != null ? product.Name : null,

                    item.VariantId,
                    VariantName = variant != null ? variant.VariantName : null,

                    OrderedQty = item.Quantity,
                    ReceivedQuantity = item.ReceivedQuantity ?? 0,
                    item.Price,
                    item.Discount,
                    item.Tax,
                    item.Total
                }).ToListAsync();

            return Ok(ApiResponse<object>.Ok(
                new
                {
                    purchaseOrder.PoId,
                    purchaseOrder.PoNumber,
                    purchaseOrder.SupplierId,
                    purchaseOrder.OrderDate,
                    purchaseOrder.ExpectedDate,
                    purchaseOrder.Status,
                    purchaseOrder.TotalAmount,
                    Items = items
                },
                "Purchase order retrieved successfully.",
                HttpContext.TraceIdentifier));
        }

    }
}
