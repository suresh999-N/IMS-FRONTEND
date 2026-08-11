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
    public class GoodsReceiptsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<GoodsReceiptsController> _logger;

        public GoodsReceiptsController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<GoodsReceiptsController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoodsReceipts()
        {
            var data = await (
                from receipt in _context.GoodsReceipts.AsNoTracking()

                join receiptItem in _context.GoodsReceiptItems.AsNoTracking()
                    on receipt.GrnId equals receiptItem.GrnId into receiptItems
                from receiptItem in receiptItems.DefaultIfEmpty()

                join purchaseOrder in _context.PurchaseOrders.AsNoTracking()
                    on receipt.PoId equals purchaseOrder.PoId into purchaseOrders
                from purchaseOrder in purchaseOrders.DefaultIfEmpty()

                join supplier in _context.Suppliers.AsNoTracking()
                    on receipt.SupplierId equals supplier.SupplierId into suppliers
                from supplier in suppliers.DefaultIfEmpty()

                join product in _context.Products.AsNoTracking()
                    on receiptItem.ProductId equals product.ProductId into products
                from product in products.DefaultIfEmpty()

                join warehouse in _context.Warehouses.AsNoTracking()
                    on receipt.WarehouseId equals warehouse.WarehouseId into warehouses
                from warehouse in warehouses.DefaultIfEmpty()

                where !receipt.IsCancelled

                select new
                {
                    receipt.GrnId,
                    receipt.GrnNumber,

                    receipt.PoId,
                    PoNumber = purchaseOrder != null ? purchaseOrder.PoNumber : null,

                    receipt.SupplierId,
                    SupplierName = supplier != null ? supplier.Name : null,

                    receipt.WarehouseId,
                    WarehouseName = warehouse != null ? warehouse.Name : null,

                    receipt.ReceiptDate,
                    receipt.Status,
                    receipt.Notes,

                    ProductId = receiptItem != null ? receiptItem.ProductId : null,
                    ProductName = product != null ? product.Name : null,
                    ProductSku = product != null ? product.SKU : null,

                    VariantId = receiptItem != null ? receiptItem.VariantId : null,

                    QuantityReceived = receiptItem != null ? receiptItem.QuantityReceived : null,

                    Price = receiptItem != null ? receiptItem.Price : null,

                    Discount = receiptItem != null ? receiptItem.Discount : null,

                    Tax = receiptItem != null ? receiptItem.Tax : null,

                    TaxPercentage = receiptItem != null ? receiptItem.TaxPercentage : null,

                    TaxAmount = receiptItem != null ? receiptItem.TaxAmount : null,

                    TaxableAmount = receiptItem != null ? receiptItem.TaxableAmount : null,

                    LineTotal = receiptItem != null ? receiptItem.LineTotal : null
                }
            ).ToListAsync();

            var result = data
                .GroupBy(x => x.GrnId)
                .Select(grn => new GoodsReceiptResponseDto
                {
                    GrnId = grn.Key,
                    GrnNumber = grn.First().GrnNumber,

                    PoId = grn.First().PoId,
                    PoNumber = grn.First().PoNumber,

                    SupplierId = grn.First().SupplierId,
                    SupplierName = grn.First().SupplierName,

                    WarehouseId = grn.First().WarehouseId,
                    WarehouseName = grn.First().WarehouseName,

                    ReceiptDate = grn.First().ReceiptDate,
                    Status = grn.First().Status,
                    Notes = grn.First().Notes,

                    Items = grn
                        .Where(x => x.ProductId != null)
                        .Select(x => new GoodsReceiptItemResponseDto
                        {
                            ProductId = x.ProductId,
                            ProductName = x.ProductName,
                            ProductSku = x.ProductSku,
                            VariantId = x.VariantId,
                            QuantityReceived = x.QuantityReceived,
                            Price = x.Price,
                            Discount = x.Discount,
                            Tax = x.Tax,
                            TaxPercentage = x.TaxPercentage,
                            TaxAmount = x.TaxAmount,
                            TaxableAmount = x.TaxableAmount,
                            LineTotal = x.LineTotal
                        })
                        .ToList()
                })
                .ToList();

            return Ok(result);
        }






        [HttpPost]
        public async Task<IActionResult> CreateGoodsReceipt(GoodsReceiptDto dto)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Goods receipt payload is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.PoId <= 0 ||
    dto.SupplierId <= 0 ||
    dto.WarehouseId <= 0 ||
    dto.Items == null ||
    !dto.Items.Any())
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Purchase order, supplier, warehouse and at least one item are required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.Items.Any(i => i.QuantityReceived <= 0 || i.Price < 0))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Received quantity must be greater than zero and price cannot be negative.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(item => item.PoId == dto.PoId);

            if (purchaseOrder == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected purchase order was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (purchaseOrder.IsCancelled)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Cannot create goods receipt for a cancelled purchase order.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (string.Equals(purchaseOrder.Status, "Received", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(purchaseOrder.ReceivingStatus, "Received", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(purchaseOrder.ReceivingStatus, "received", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Purchase order is fully received and cannot accept further goods receipts.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (purchaseOrder.SupplierId != dto.SupplierId)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected supplier does not match the purchase order.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var poItemsForValidation = await _context.PurchaseOrderItems
                .AsNoTracking()
                .Where(x => x.PoId == dto.PoId)
                .ToListAsync();

            foreach (var item in dto.Items)
            {
                var matchingPoItem = poItemsForValidation.FirstOrDefault(x =>
                    x.ProductId == item.ProductId &&
                    x.VariantId == item.VariantId);

                if (matchingPoItem == null)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        $"Product ID {item.ProductId} (Variant ID: {item.VariantId?.ToString() ?? "N/A"}) is not present in the selected Purchase Order.",
                        traceId: HttpContext.TraceIdentifier));
                }

                var orderedQty = matchingPoItem.Quantity ?? 0m;
                var alreadyReceivedQty = matchingPoItem.ReceivedQuantity ?? 0m;
                var remainingQty = Math.Max(0m, orderedQty - alreadyReceivedQty);

                if (item.QuantityReceived > remainingQty)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        $"Received quantity ({item.QuantityReceived}) for Product ID {item.ProductId} exceeds remaining PO quantity ({remainingQty}).",
                        traceId: HttpContext.TraceIdentifier));
                }
            }

            var warehouseExists = await _context.Warehouses
                .AnyAsync(item => item.WarehouseId == dto.WarehouseId);

            if (!warehouseExists)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected warehouse was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;
                var receipt = new GoodsReceipt
                {
                    PoId = dto.PoId,
                    SupplierId = dto.SupplierId,
                    WarehouseId = dto.WarehouseId,
                    ReceiptDate = dto.ReceiptDate == default ? now : dto.ReceiptDate,
                    SupplierInvoice = dto.SupplierInvoice,
                    SupplierInvoiceDate = dto.SupplierInvoiceDate,
                    Status = "completed",
                    Notes = dto.Notes?.Trim()
                };

                _context.GoodsReceipts.Add(receipt);
                await _context.SaveChangesAsync();

                receipt.GrnNumber = $"GRN-{receipt.GrnId:D6}";
                await _context.SaveChangesAsync();

                foreach (var item in dto.Items)
                {
                    var subtotal = item.QuantityReceived * item.Price;

                    var discountAmount = subtotal * item.Discount / 100;

                    var taxableAmount = subtotal - discountAmount;

                    var taxAmount = taxableAmount * item.Tax / 100;

                    var lineTotal = taxableAmount + taxAmount;

                    _context.GoodsReceiptItems.Add(new GoodsReceiptItem
                    {
                        GrnId = receipt.GrnId,
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        QuantityReceived = item.QuantityReceived,
                        Price = item.Price,
                        Discount = item.Discount,
                        Tax = item.Tax,
                        TaxPercentage = item.Tax,
                        TaxAmount = taxAmount,
                        TaxableAmount = taxableAmount,
                        LineTotal = lineTotal
                    });



                    var poItem = await _context.PurchaseOrderItems
    .FirstOrDefaultAsync(x =>
        x.PoId == dto.PoId &&
        x.ProductId == item.ProductId &&
        x.VariantId == item.VariantId);

                    if (poItem != null)
                    {
                        poItem.ReceivedQuantity =
                            (poItem.ReceivedQuantity ?? 0) + item.QuantityReceived;
                    }




                    var product = await _context.Products
                        .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && !x.IsDeleted);

                    if (product != null)
                    {
                        product.UpdatedAt = now;
                    }

                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        WarehouseId = dto.WarehouseId,
                        MovementType = "PURCHASE",
                        Quantity = item.QuantityReceived,
                        ReferenceId = receipt.GrnId,
                        ReferenceType = "goods_receipt",
                        Notes = "Stock added from goods receipt",
                        CreatedAt = now
                    });

                    var stock = await _context.Stocks.FirstOrDefaultAsync(s =>
    s.ProductId == item.ProductId &&
    s.VariantId == item.VariantId &&
    s.WarehouseId == dto.WarehouseId);

                    if (stock == null)
                    {
                        _context.Stocks.Add(new Stock
                        {
                            ProductId = item.ProductId,
                            VariantId = item.VariantId,
                            WarehouseId = dto.WarehouseId,
                            Quantity = item.QuantityReceived,
                            ReservedQuantity = 0
                        });
                    }
                    else
                    {
                        stock.Quantity += item.QuantityReceived;
                    }


                }










                // Save GRN + stock changes first
                await _context.SaveChangesAsync();

                var poItems = await _context.PurchaseOrderItems
    .Where(x => x.PoId == dto.PoId)
    .ToListAsync();

                var totalOrderedQty = poItems.Sum(x => x.Quantity ?? 0);

                var totalReceivedQty = poItems.Sum(x => x.ReceivedQuantity ?? 0);

                UpdatePurchaseOrderReceivingStatus(
                    purchaseOrder,
                    totalOrderedQty,
                    totalReceivedQty);

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "CREATE_GOODS_RECEIPT",
                    "Inventory",
                    receipt.GrnId,
                    $"Goods receipt GRN-{receipt.GrnId} created",
                    "goods_receipts");

                await transaction.CommitAsync();

                return Ok(ApiResponse<object>.Ok(
    new
    {
        receipt.GrnId,
        receipt.GrnNumber,
        receipt.PoId,
        receipt.ReceiptDate
    },
    "Goods received successfully.",
    HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync();
                LogDbUpdateException(exception, "Goods receipt create failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                _logger.LogError(
                    exception,
                    "Goods receipt create failed. InnerException: {InnerException}. TraceId: {TraceId}",
                    exception.InnerException?.ToString() ?? "No inner exception",
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoodsReceipt(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
            => await ReverseGoodsReceipt(id, dto, cancellationToken);

        [HttpPost("{id}/reverse")]
        public async Task<IActionResult> ReverseGoodsReceipt(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
        {
            var receipt = await _context.GoodsReceipts
                .FirstOrDefaultAsync(item => item.GrnId == id, cancellationToken);

            if (receipt == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Goods receipt was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (receipt.IsCancelled)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Goods receipt is already reversed.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTime.UtcNow;
                var items = await _context.GoodsReceiptItems
                    .Where(item => item.GrnId == id)
                    .ToListAsync(cancellationToken);

                foreach (var item in items)
                {
                    if (!item.ProductId.HasValue)
                    {
                        continue;
                    }

                    var receivedQuantity = item.QuantityReceived ?? 0;
                    var stock = await _context.Stocks
                        .FirstOrDefaultAsync(stock =>
                            stock.ProductId == item.ProductId.Value &&
                            stock.VariantId == item.VariantId &&
                            stock.WarehouseId == receipt.WarehouseId,
                            cancellationToken);

                    if (stock == null || stock.Quantity < receivedQuantity)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Conflict(ApiResponse<object>.Fail(
                            "Goods receipt cannot be fully reversed because some received stock has already been consumed. Use a partial reversal workflow for the remaining quantity.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var openingQty = stock.Quantity;
                    stock.Quantity -= receivedQuantity;


                    var poItem = await _context.PurchaseOrderItems
    .FirstOrDefaultAsync(x =>
        x.PoId == receipt.PoId &&
        x.ProductId == item.ProductId &&
        x.VariantId == item.VariantId,
        cancellationToken);

                    if (poItem != null)
                    {
                        poItem.ReceivedQuantity =
                            Math.Max(
                                0,
                                (poItem.ReceivedQuantity ?? 0)
                                - receivedQuantity);
                    }

                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = item.ProductId.Value,
                        VariantId = item.VariantId,
                        WarehouseId = receipt.WarehouseId ?? stock.WarehouseId,
                        MovementType = "PURCHASE",
                        Quantity = -receivedQuantity,
                        ReferenceId = receipt.GrnId,
                        ReferenceType = "goods_receipt_reversal",
                        Notes = $"Reversal for goods receipt {receipt.GrnId}",
                        CreatedAt = now
                    });






                    _context.StockLedgers.Add(new StockLedger
                    {
                        ProductId = item.ProductId.Value,
                        VariantId = item.VariantId,
                        WarehouseId = receipt.WarehouseId ?? stock.WarehouseId,
                        OpeningQty = openingQty,
                        ChangeQty = -receivedQuantity,
                        ClosingQty = stock.Quantity,
                        TransactionType = "PURCHASE_REVERSAL",
                        TransactionId = receipt.GrnId,
                        CreatedAt = now
                    });
                }

                receipt.Status = "reversed";
                receipt.IsCancelled = true;
                receipt.CancelledAt = now;
                receipt.CancellationReason = dto?.Reason;

                if (receipt.PoId.HasValue)
                {
                    var po = await _context.PurchaseOrders
                        .FirstOrDefaultAsync(item => item.PoId == receipt.PoId.Value, cancellationToken);

                    if (po != null)
                    {
                        await UpdatePurchaseOrderReceivingStatus(po, cancellationToken);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "DELETE_GOODS_RECEIPT",
                    "Inventory",
                    receipt.GrnId,
                    $"Goods receipt GRN-{receipt.GrnId} reversed",
                    "goods_receipts",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(ApiResponse<object>.Ok(
                    new
                    {
                        goodsReceiptId = id
                    },
                    "Goods receipt reversed successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Goods receipt reversal failed.");

                var blockers = await GetGoodsReceiptDeleteBlockers(id);
                _logger.LogWarning(
                    exception,
                    "Goods receipt reversal blockers remain. GrnId: {GrnId}. Blockers: {Blockers}. InnerException: {InnerException}. TraceId: {TraceId}",
                    id,
                    string.Join("; ", blockers),
                    exception.InnerException?.ToString() ?? "No inner exception",
                    HttpContext.TraceIdentifier);

                return Conflict(ApiResponse<object>.Fail(
                    blockers.Count > 0
                        ? string.Join("; ", blockers)
                        : GetDeleteDependencyMessage(exception, id),
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        private async Task UpdatePurchaseOrderReceivingStatus(PurchaseOrder purchaseOrder, CancellationToken cancellationToken)
        {
            var poItems = await _context.PurchaseOrderItems
                .Where(x => x.PoId == purchaseOrder.PoId)
                .ToListAsync(cancellationToken);

            var totalOrderedQty = poItems.Sum(x => x.Quantity ?? 0);
            var totalReceivedQty = poItems.Sum(x => x.ReceivedQuantity ?? 0);

            UpdatePurchaseOrderReceivingStatus(
                purchaseOrder,
                totalOrderedQty,
                totalReceivedQty);
        }

        private static void UpdatePurchaseOrderReceivingStatus(
            PurchaseOrder purchaseOrder,
            decimal totalOrderedQty,
            decimal totalReceivedQty)
        {
            if (totalReceivedQty <= 0)
            {
                purchaseOrder.Status = "Ordered";
                purchaseOrder.ReceivingStatus = "Ordered";
            }
            else if (totalReceivedQty < totalOrderedQty)
            {
                purchaseOrder.Status = "Partially Received";
                purchaseOrder.ReceivingStatus = "Partially Received";
            }
            else
            {
                purchaseOrder.Status = "Received";
                purchaseOrder.ReceivingStatus = "Received";
            }
        }

        private async Task<ReceiptCleanupSummary> CleanupGoodsReceiptDependencies(GoodsReceipt receipt)
        {
            var receiptId = receipt.GrnId;
            var items = await _context.GoodsReceiptItems
                .Where(item => item.GrnId == receiptId)
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
            var warehouseId = receipt.WarehouseId;
            var summary = new ReceiptCleanupSummary();

            var stockMovements = await _context.StockMovements
    .Where(item =>
        item.ReferenceId == receiptId &&
        item.ReferenceType == "goods_receipt")
    .ToListAsync();

            var stockLedgers = await _context.StockLedgers
    .Where(item =>
        item.TransactionId == receiptId &&
        item.TransactionType == "PURCHASE")
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
                    (item.RecordId == receiptId &&
                        (item.TableName == "goods_receipts" ||
                         item.TableName == "goods_receipt_items" ||
                         item.Module == "GoodsReceipts" ||
                         item.Module == "Receiving")) ||
                    (item.RecordId.HasValue && productIds.Contains(item.RecordId.Value) &&
                        (item.TableName == "products" ||
                         item.Module == "Inventory" ||
                         item.Module == "Stock")))
                .ToListAsync();

            summary.StockMovementsDeleted = stockMovements.Count;
            summary.StockLedgersDeleted = stockLedgers.Count;
            summary.StockAdjustmentItemsDeleted = stockAdjustmentItems.Count;
            summary.StockTransferItemsDeleted = stockTransferItems.Count;
            summary.StockAuditItemsDeleted = stockAuditItems.Count;
            summary.AuditLogsDeleted = auditLogs.Count;

            await LogGoodsReceiptCleanupCounts(
                receipt,
                items.Count,
                productIds,
                stockMovements.Count,
                stockLedgers.Count,
                stockAdjustmentItems.Count,
                stockTransferItems.Count,
                stockAuditItems.Count,
                auditLogs.Count);

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
                        (!warehouseId.HasValue || stock.WarehouseId == warehouseId.Value))
                    .ToListAsync();

                foreach (var stock in stockRows)
                {
                    stock.Quantity = Math.Max(stock.Quantity - quantity, 0);

                    if (stock.Quantity == 0 && stock.ReservedQuantity == 0)
                    {
                        _context.Stocks.Remove(stock);
                        summary.StockRowsDeleted++;
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




            // Recalculate stock ledger balances
            foreach (var productId in productIds)
            {
                var ledgers = await _context.StockLedgers
                    .Where(item =>
                        item.ProductId == productId &&
                        (!warehouseId.HasValue || item.WarehouseId == warehouseId))
                    .OrderBy(item => item.CreatedAt)
                    .ThenBy(item => item.LedgerId)
                    .ToListAsync();

                decimal runningQty = 0;

                foreach (var ledger in ledgers)
                {
                    ledger.OpeningQty = runningQty;

                    runningQty += ledger.ChangeQty;

                    ledger.ClosingQty = runningQty;
                }
            }

            await _context.SaveChangesAsync();


            // Recalculate PO status after GRN delete
            if (receipt.PoId.HasValue)
            {
                var purchaseOrder = await _context.PurchaseOrders
                    .FirstOrDefaultAsync(po => po.PoId == receipt.PoId);

                if (purchaseOrder != null)
                {
                    // Total ordered quantity


                    var poItems = await _context.PurchaseOrderItems
    .Where(x => x.PoId == receipt.PoId!.Value)
    .ToListAsync();

                    var totalOrderedQty = poItems.Sum(x => x.Quantity ?? 0);

                    var totalReceivedQty = poItems.Sum(x => x.ReceivedQuantity ?? 0);

                    // Update PO status

                    purchaseOrder.Status = "ordered";

                    if (totalReceivedQty <= 0)
                    {
                        purchaseOrder.ReceivingStatus = "pending";
                    }
                    else if (totalReceivedQty < totalOrderedQty)
                    {
                        purchaseOrder.ReceivingStatus = "partial";
                    }
                    else
                    {
                        purchaseOrder.ReceivingStatus = "received";
                    }

                    await _context.SaveChangesAsync();
                }
            }


            return summary;
        }

        private async Task LogGoodsReceiptCleanupCounts(
            GoodsReceipt receipt,
            int goodsReceiptItemsCount,
            IReadOnlyCollection<int> productIds,
            int stockMovementsCount,
            int stockLedgersCount,
            int stockAdjustmentItemsCount,
            int stockTransferItemsCount,
            int stockAuditItemsCount,
            int auditLogsCount)
        {
            var stockRegisterCount = productIds.Count == 0
                ? 0
                : await _context.Stocks
                    .AsNoTracking()
                    .CountAsync(item => productIds.Contains(item.ProductId));

            _logger.LogInformation(
                "GoodsReceipt delete cleanup counts. GrnId: {GrnId}, PoId: {PoId}, ProductIds: {ProductIds}, GoodsReceiptItems: {GoodsReceiptItems}, StockRegister: {StockRegister}, StockMovements: {StockMovements}, StockLedger: {StockLedger}, InventoryTransactions: {InventoryTransactions}, StockAdjustmentItems: {StockAdjustmentItems}, StockTransferItems: {StockTransferItems}, StockAuditItems: {StockAuditItems}, AuditLogs: {AuditLogs}, TraceId: {TraceId}",
                receipt.GrnId,
                receipt.PoId,
                string.Join(",", productIds),
                goodsReceiptItemsCount,
                stockRegisterCount,
                stockMovementsCount,
                stockLedgersCount,
                0,
                stockAdjustmentItemsCount,
                stockTransferItemsCount,
                stockAuditItemsCount,
                auditLogsCount,
                HttpContext.TraceIdentifier);
        }

        private async Task<List<string>> GetGoodsReceiptDeleteBlockers(int receiptId)
        {
            var productIds = await _context.GoodsReceiptItems
                .AsNoTracking()
                .Where(item => item.GrnId == receiptId && item.ProductId.HasValue)
                .Select(item => item.ProductId!.Value)
                .Distinct()
                .ToListAsync();
            var blockers = new List<string>();

            if (await _context.GoodsReceiptItems.AsNoTracking().AnyAsync(item => item.GrnId == receiptId))
            {
                blockers.Add("GoodsReceiptItems records exist");
            }

            if (productIds.Count > 0 &&
                await _context.Stocks.AsNoTracking().AnyAsync(item => productIds.Contains(item.ProductId)))
            {
                blockers.Add("StockRegister records exist");
            }

            if (await _context.StockMovements.AsNoTracking().AnyAsync(item =>
                    item.ReferenceId == receiptId ||
                    productIds.Contains(item.ProductId)))
            {
                blockers.Add("StockMovements records exist");
            }

            if (await _context.StockLedgers.AsNoTracking().AnyAsync(item =>
                    item.TransactionId == receiptId ||
                    productIds.Contains(item.ProductId)))
            {
                blockers.Add("StockLedger records exist");
            }

            if (productIds.Count > 0 &&
                await _context.StockAdjustmentItems.AsNoTracking().AnyAsync(item => productIds.Contains(item.ProductId)))
            {
                blockers.Add("StockAdjustmentItems records exist");
            }

            if (productIds.Count > 0 &&
                await _context.StockTransferItems.AsNoTracking().AnyAsync(item => productIds.Contains(item.ProductId)))
            {
                blockers.Add("StockTransferItems records exist");
            }

            if (productIds.Count > 0 &&
                await _context.StockAuditItems.AsNoTracking().AnyAsync(item => item.ProductId.HasValue && productIds.Contains(item.ProductId.Value)))
            {
                blockers.Add("StockAuditItems records exist");
            }

            return blockers.Distinct().ToList();
        }

        private static string GetDeleteDependencyMessage(DbUpdateException exception, int receiptId)
        {
            var detail = GetInnermostMessage(exception);
            var normalizedDetail = detail.ToLowerInvariant();
            var foreignKeySource = TryReadForeignKeySource(detail);

            if (!string.IsNullOrWhiteSpace(foreignKeySource))
            {
                return $"Delete blocked by {foreignKeySource} rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("goods_receipt_items"))
            {
                return $"Delete blocked by GoodsReceiptItems rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("stock_adjustment_items"))
            {
                return $"Delete blocked by StockAdjustmentItems rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("stock_transfer_items"))
            {
                return $"Delete blocked by StockTransferItems rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("stock_audit_items"))
            {
                return $"Delete blocked by StockAuditItems rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("stock_movements"))
            {
                return $"Delete blocked by StockMovements rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("stock_ledger"))
            {
                return $"Delete blocked by StockLedger rows for GoodsReceiptId={receiptId}";
            }

            if (normalizedDetail.Contains("stock"))
            {
                return $"Delete blocked by StockRegister rows for GoodsReceiptId={receiptId}";
            }

            return detail;
        }

        private static string? TryReadForeignKeySource(string detail)
        {
            var tableMatch = System.Text.RegularExpressions.Regex.Match(
                detail,
                @"table\s+`[^`]+`\.`(?<table>[^`]+)`",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (tableMatch.Success)
            {
                return tableMatch.Groups["table"].Value;
            }

            var constraintMatch = System.Text.RegularExpressions.Regex.Match(
                detail,
                @"CONSTRAINT\s+`(?<constraint>[^`]+)`",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return constraintMatch.Success ? constraintMatch.Groups["constraint"].Value : null;
        }

        private static string GetDbUpdateUserMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);

            if (detail.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("constraint", StringComparison.OrdinalIgnoreCase))
            {
                return "Goods receipt could not be saved because a selected purchase order, supplier, product, or warehouse is invalid.";
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

        private sealed class ReceiptCleanupSummary
        {
            public int StockRowsDeleted { get; set; }
            public int StockMovementsDeleted { get; set; }
            public int StockLedgersDeleted { get; set; }
            public int StockAdjustmentItemsDeleted { get; set; }
            public int StockTransferItemsDeleted { get; set; }
            public int StockAuditItemsDeleted { get; set; }
            public int AuditLogsDeleted { get; set; }
        }

        // GET: api/GoodsReceipts/by-po/5
        [HttpGet("by-po/{poId:int}")]
        public async Task<IActionResult> GetGrnsByPurchaseOrder(int poId)
        {
            var grns = await _context.GoodsReceipts
                .AsNoTracking()
                .Where(g =>
                    g.PoId == poId &&
                    !g.IsCancelled &&
                    g.Status == "completed")
                .Select(g => new
                {
                    g.GrnId,
                    g.GrnNumber,
                    g.ReceiptDate
                })
                .OrderByDescending(g => g.ReceiptDate)
                .ToListAsync();

            return Ok(grns);
        }

        // GET: api/GoodsReceipts/1/return-items
        [HttpGet("{grnId:int}/return-items")]
        public async Task<IActionResult> GetReturnItems(int grnId)
        {
            var grn = await _context.GoodsReceipts
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.GrnId == grnId &&
                    !g.IsCancelled &&
                    g.Status == "completed");

            if (grn == null)
            {
                return NotFound("Approved GRN not found.");
            }

            var items = await (
                from gri in _context.GoodsReceiptItems

                join p in _context.Products
                    on gri.ProductId equals p.ProductId

                join u in _context.Units
                    on p.UnitId equals u.UnitId into units
                from u in units.DefaultIfEmpty()

                join poi in _context.PurchaseOrderItems
                    on new { PoId = grn.PoId, ProductId = gri.ProductId } equals new { PoId = (int?)poi.PoId, ProductId = poi.ProductId } into poItems
                from poi in poItems.DefaultIfEmpty()

                where gri.GrnId == grnId

                select new
                {
                    GoodsReceiptItemId = gri.Id,
                    gri.ProductId,
                    ProductName = p.Name,

                    gri.VariantId,

                    UnitId = p.UnitId,
                    UnitName = u != null ? u.Name : "",

                    ReceivedQty = gri.QuantityReceived,

                    ReturnedQty = 0,

                    AvailableQty = gri.QuantityReceived ?? 0,
                    UnitCost = (gri.Price != null && gri.Price > 0)
                        ? (gri.Price ?? 0)
                        : (poi != null && poi.Price != null && poi.Price > 0)
                            ? (poi.Price ?? 0)
                            : (p.CostPrice != null && p.CostPrice > 0)
                                ? (p.CostPrice ?? 0)
                                : (p.Price ?? 0),

                    TaxPercent = gri.Tax ?? (poi != null ? poi.Tax ?? 0 : 0)
                }).ToListAsync();

            return Ok(new
            {
                grn.GrnId,
                grn.GrnNumber,
                Items = items
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGoodsReceipt(int id)
        {
            var grn = await _context.GoodsReceipts
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GrnId == id && !g.IsCancelled);

            if (grn == null)
                return NotFound();

            var purchaseOrder = await _context.PurchaseOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(po => po.PoId == grn.PoId);

            var supplier = await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SupplierId == grn.SupplierId);

            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WarehouseId == grn.WarehouseId);

            var items = await (
                from gri in _context.GoodsReceiptItems.AsNoTracking()
                join p in _context.Products.AsNoTracking()
                    on gri.ProductId equals p.ProductId into products
                from p in products.DefaultIfEmpty()
                join poi in _context.PurchaseOrderItems.AsNoTracking()
                    on new { PoId = grn.PoId, ProductId = gri.ProductId, VariantId = gri.VariantId }
                    equals new { PoId = (int?)poi.PoId, ProductId = (int?)poi.ProductId, VariantId = poi.VariantId } into poItems
                from poi in poItems.DefaultIfEmpty()
                where gri.GrnId == id
                select new GoodsReceiptItemResponseDto
                {
                    ProductId = gri.ProductId,
                    ProductName = p != null ? p.Name : null,
                    ProductSku = p != null ? p.SKU : null,
                    VariantId = gri.VariantId,
                    OrderedQuantity = poi != null ? poi.Quantity : null,
                    QuantityReceived = gri.QuantityReceived,
                    Price = gri.Price ?? (poi != null ? poi.Price : (p != null ? p.CostPrice ?? p.Price : null)),
                    Discount = gri.Discount,
                    Tax = gri.Tax,
                    TaxPercentage = gri.TaxPercentage,
                    TaxAmount = gri.TaxAmount,
                    TaxableAmount = gri.TaxableAmount,
                    LineTotal = gri.LineTotal
                }
            ).ToListAsync();

            return Ok(new GoodsReceiptResponseDto
            {
                GrnId = grn.GrnId,
                GrnNumber = grn.GrnNumber,
                PoId = grn.PoId,
                PoNumber = purchaseOrder?.PoNumber,
                SupplierId = grn.SupplierId,
                SupplierName = supplier?.Name,
                WarehouseId = grn.WarehouseId,
                WarehouseName = warehouse?.Name,
                ReceiptDate = grn.ReceiptDate,
                Status = grn.Status,
                Notes = grn.Notes,
                Items = items
            });
        }
    }
}
