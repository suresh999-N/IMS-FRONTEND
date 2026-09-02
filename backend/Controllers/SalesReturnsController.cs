using IMSBackend.Data;
using IMSBackend.DTOs.SalesReturns;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesReturnsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesReturnsController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GET ALL SALES RETURNS (WITH FILTERS & PAGINATION)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetSalesReturns(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] int? customerId,
            [FromQuery] int? invoiceId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = _context.SalesReturns
                .AsNoTracking()
                .Where(r => !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchPattern = $"%{search.Trim()}%";
                query = query.Where(r =>
                    (r.ReturnNumber != null && EF.Functions.Like(r.ReturnNumber, searchPattern)) ||
                    (r.Invoice != null && r.Invoice.InvoiceNumber != null && EF.Functions.Like(r.Invoice.InvoiceNumber, searchPattern)) ||
                    (r.Customer != null && r.Customer.Name != null && EF.Functions.Like(r.Customer.Name, searchPattern)) ||
                    (r.Reason != null && EF.Functions.Like(r.Reason, searchPattern)));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "all" && status != "All")
            {
                query = query.Where(r => r.Status.ToLower() == status.Trim().ToLower());
            }

            if (customerId.HasValue && customerId > 0)
            {
                query = query.Where(r => r.CustomerId == customerId.Value);
            }

            if (invoiceId.HasValue && invoiceId > 0)
            {
                query = query.Where(r => r.InvoiceId == invoiceId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.ReturnDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.ReturnDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Include(r => r.Invoice)
                .Include(r => r.Customer)
                .Include(r => r.Warehouse)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Product)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Variant)
                .OrderByDescending(r => r.SalesReturnId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => MapToDto(r))
                .ToListAsync(cancellationToken);

            return Ok(new
            {
                data = items,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        // ==========================================
        // 2. GET SALES RETURN BY ID
        // ==========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesReturnById(int id, CancellationToken cancellationToken = default)
        {
            var salesReturn = await _context.SalesReturns
                .AsNoTracking()
                .Include(r => r.Invoice)
                .Include(r => r.Customer)
                .Include(r => r.Warehouse)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Product)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Variant)
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            return Ok(MapToDto(salesReturn));
        }

        // ==========================================
        // 3. GET RETURNABLE INVOICES
        // ==========================================
        [HttpGet("returnable-invoices")]
        public async Task<IActionResult> GetReturnableInvoices(CancellationToken cancellationToken = default)
        {
            var invoices = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                .Where(i => !i.IsCancelled && i.Status != "Cancelled")
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync(cancellationToken);

            var activeReturnItems = await (
                from r in _context.SalesReturns.AsNoTracking()
                from ri in r.Items
                where !r.IsDeleted && r.Status != "Rejected" && r.Status != "rejected"
                select new { r.InvoiceId, ri.ProductId, ri.VariantId, ri.ReturnQuantity }
            ).ToListAsync(cancellationToken);

            var result = new List<object>();

            foreach (var inv in invoices)
            {
                if (inv.InvoiceItems == null || !inv.InvoiceItems.Any()) continue;

                decimal totalReturnableQty = 0m;

                foreach (var item in inv.InvoiceItems)
                {
                    var prevReturned = activeReturnItems
                        .Where(ri => ri.InvoiceId == inv.InvoiceId &&
                                     ri.ProductId == (item.ProductId ?? 0) &&
                                     (ri.VariantId == item.VariantId || (ri.VariantId == null && item.VariantId == null)))
                        .Sum(ri => ri.ReturnQuantity);

                    var returnable = Math.Max(0m, item.Quantity - prevReturned);
                    totalReturnableQty += returnable;
                }

                if (totalReturnableQty > 0)
                {
                    result.Add(new
                    {
                        inv.InvoiceId,
                        Id = inv.InvoiceId,
                        inv.InvoiceNumber,
                        inv.InvoiceDate,
                        inv.CustomerId,
                        CustomerName = inv.Customer?.Name ?? "Walk-in Customer",
                        inv.TotalAmount,
                        TotalReturnableQuantity = totalReturnableQty
                    });
                }
            }

            return Ok(result);
        }

        // ==========================================
        // 4. GET INVOICE DETAILS & RETURNABLE QUANTITIES
        // ==========================================
        [HttpGet("invoice-details/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceReturnableDetails(int invoiceId, CancellationToken cancellationToken = default)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems!)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && !i.IsCancelled, cancellationToken);

            if (invoice == null)
            {
                return NotFound(new { success = false, message = "Invoice not found or cancelled." });
            }

            // Calculate previously returned quantities for items of this invoice from non-rejected returns
            var activeReturnItems = await (
                from ri in _context.SalesReturnItems.AsNoTracking()
                join r in _context.SalesReturns.AsNoTracking() on ri.SalesReturnId equals r.SalesReturnId
                where r.InvoiceId == invoiceId && !r.IsDeleted && r.Status != "Rejected" && r.Status != "rejected"
                select ri
            ).ToListAsync(cancellationToken);

            var itemsDto = new List<InvoiceReturnableItemDto>();

            if (invoice.InvoiceItems != null)
            {
                foreach (var item in invoice.InvoiceItems)
                {
                    var previouslyReturned = activeReturnItems
                        .Where(ri => ri.ProductId == item.ProductId && ri.VariantId == item.VariantId)
                        .Sum(ri => ri.ReturnQuantity);

                    var returnableQty = Math.Max(0m, item.Quantity - previouslyReturned);

                    itemsDto.Add(new InvoiceReturnableItemDto
                    {
                        ProductId = item.ProductId ?? 0,
                        ProductName = item.Product?.Name ?? "Unknown Product",
                        ProductSKU = item.Product?.SKU ?? string.Empty,
                        VariantId = item.VariantId,
                        VariantName = null,
                        SoldQuantity = item.Quantity,
                        PreviouslyReturnedQuantity = previouslyReturned,
                        ReturnableQuantity = returnableQty,
                        Price = item.Price,
                        TaxPercent = item.TaxPercent,
                        DiscountPercent = 0m
                    });
                }
            }

            var result = new InvoiceReturnableDetailsDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerId = invoice.CustomerId ?? 0,
                CustomerName = invoice.Customer?.Name ?? "N/A",
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                BalanceAmount = invoice.BalanceAmount,
                Status = invoice.Status,
                Items = itemsDto
            };

            return Ok(result);
        }

        // ==========================================
        // 4. CREATE SALES RETURN (DRAFT OR SUBMITTED)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CreateSalesReturn([FromBody] CreateSalesReturnDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { success = false, message = "Invalid data. Please provide return items." });
            }

            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceId == dto.InvoiceId && !i.IsCancelled, cancellationToken);

            if (invoice == null)
            {
                return BadRequest(new { success = false, message = "Invoice not found or is cancelled." });
            }

            if (!invoice.CustomerId.HasValue)
            {
                return BadRequest(new { success = false, message = "Selected invoice does not have a customer assigned." });
            }

            // Fetch existing active return items for validation
            var existingReturnItems = await (
                from ri in _context.SalesReturnItems.AsNoTracking()
                join r in _context.SalesReturns.AsNoTracking() on ri.SalesReturnId equals r.SalesReturnId
                where r.InvoiceId == dto.InvoiceId && !r.IsDeleted && r.Status != "Rejected" && r.Status != "rejected"
                select ri
            ).ToListAsync(cancellationToken);

            var itemsToSave = new List<SalesReturnItem>();
            decimal subtotal = 0m;
            decimal totalTax = 0m;
            decimal totalDiscount = 0m;

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.ReturnQuantity <= 0) continue;

                var invItem = invoice.InvoiceItems?.FirstOrDefault(x =>
                    x.ProductId == itemDto.ProductId &&
                    (x.VariantId == itemDto.VariantId || (x.VariantId == null && (itemDto.VariantId == null || itemDto.VariantId == 0))));

                if (invItem == null)
                {
                    return BadRequest(new { success = false, message = $"Product ID {itemDto.ProductId} was not found on Invoice {invoice.InvoiceNumber}." });
                }

                var previouslyReturned = existingReturnItems
                    .Where(x => x.ProductId == itemDto.ProductId && (x.VariantId == itemDto.VariantId || (x.VariantId == null && (itemDto.VariantId == null || itemDto.VariantId == 0))))
                    .Sum(x => x.ReturnQuantity);

                var maxReturnable = invItem.Quantity - previouslyReturned;

                if (itemDto.ReturnQuantity > maxReturnable)
                {
                    return BadRequest(new { success = false, message = $"Cannot return {itemDto.ReturnQuantity} units for Product ID {itemDto.ProductId}. Maximum returnable quantity is {maxReturnable}." });
                }

                var lineSubtotal = itemDto.ReturnQuantity * itemDto.Price;
                var lineDiscount = lineSubtotal * (itemDto.Discount / 100m);
                var taxableAmount = lineSubtotal - lineDiscount;
                var lineTax = taxableAmount * (itemDto.Tax / 100m);
                var lineTotal = taxableAmount + lineTax;

                subtotal += lineSubtotal;
                totalDiscount += lineDiscount;
                totalTax += lineTax;

                itemsToSave.Add(new SalesReturnItem
                {
                    ProductId = itemDto.ProductId,
                    VariantId = (itemDto.VariantId.HasValue && itemDto.VariantId.Value > 0) ? itemDto.VariantId : null,
                    InvoicedQuantity = invItem.Quantity,
                    ReturnQuantity = itemDto.ReturnQuantity,
                    Price = itemDto.Price,
                    Tax = itemDto.Tax,
                    TaxAmount = lineTax,
                    Discount = itemDto.Discount,
                    Total = lineTotal
                });
            }

            if (!itemsToSave.Any())
            {
                return BadRequest(new { success = false, message = "At least one item must have a return quantity greater than 0." });
            }

            var grandTotal = subtotal - totalDiscount + totalTax;
            var now = DateTime.UtcNow;

            var salesReturn = new SalesReturn
            {
                InvoiceId = dto.InvoiceId,
                CustomerId = invoice.CustomerId.Value,
                WarehouseId = dto.WarehouseId,
                ReturnDate = dto.ReturnDate,

                // Temporary value because return_number is NOT NULL
                // and the database ID is not available until after the first save.
                ReturnNumber = $"TEMP-{Guid.NewGuid():N}",

                TotalAmount = subtotal,
                TaxAmount = totalTax,
                DiscountAmount = totalDiscount,
                GrandTotal = grandTotal,
                RefundAmount = grandTotal,
                Status = dto.SubmitForApproval ? "Pending Approval" : "Draft",
                Reason = dto.Reason,
                Notes = dto.Notes,
                CreatedAt = now,
                UpdatedAt = now,
                Items = itemsToSave
            };

            _context.SalesReturns.Add(salesReturn);

            await _context.SaveChangesAsync();

            // Now SalesReturnId is generated by MySQL.
            salesReturn.ReturnNumber = $"SRET-{salesReturn.SalesReturnId:D6}";

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSalesReturnById), new { id = salesReturn.SalesReturnId }, MapToDto(salesReturn));
        }

        // ==========================================
        // 5. UPDATE SALES RETURN (DRAFT ONLY)
        // ==========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesReturn(int id, [FromBody] UpdateSalesReturnDto dto, CancellationToken cancellationToken = default)
        {
            var salesReturn = await _context.SalesReturns
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Draft" && salesReturn.Status != "draft")
            {
                return BadRequest(new { success = false, message = $"Only returns in 'Draft' status can be edited. Current status is '{salesReturn.Status}'." });
            }

            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceId == salesReturn.InvoiceId, cancellationToken);

            if (invoice == null)
            {
                return BadRequest(new { success = false, message = "Associated invoice not found." });
            }

            // Other returns on same invoice
            var existingReturnItems = await (
                from ri in _context.SalesReturnItems.AsNoTracking()
                join r in _context.SalesReturns.AsNoTracking() on ri.SalesReturnId equals r.SalesReturnId
                where r.InvoiceId == salesReturn.InvoiceId && r.SalesReturnId != id && !r.IsDeleted && r.Status != "Rejected" && r.Status != "rejected"
                select ri
            ).ToListAsync(cancellationToken);

            _context.SalesReturnItems.RemoveRange(salesReturn.Items);

            decimal subtotal = 0m;
            decimal totalTax = 0m;
            decimal totalDiscount = 0m;
            var newItems = new List<SalesReturnItem>();

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.ReturnQuantity <= 0) continue;

                var invItem = invoice.InvoiceItems?.FirstOrDefault(x =>
                    x.ProductId == itemDto.ProductId &&
                    (x.VariantId == itemDto.VariantId || (x.VariantId == null && (itemDto.VariantId == null || itemDto.VariantId == 0))));

                if (invItem == null)
                {
                    return BadRequest(new { success = false, message = $"Product ID {itemDto.ProductId} was not found on Invoice {invoice.InvoiceNumber}." });
                }

                var previouslyReturned = existingReturnItems
                    .Where(x => x.ProductId == itemDto.ProductId && (x.VariantId == itemDto.VariantId || (x.VariantId == null && (itemDto.VariantId == null || itemDto.VariantId == 0))))
                    .Sum(x => x.ReturnQuantity);

                var maxReturnable = invItem.Quantity - previouslyReturned;

                if (itemDto.ReturnQuantity > maxReturnable)
                {
                    return BadRequest(new { success = false, message = $"Cannot return {itemDto.ReturnQuantity} units for Product ID {itemDto.ProductId}. Maximum returnable quantity is {maxReturnable}." });
                }

                var lineSubtotal = itemDto.ReturnQuantity * itemDto.Price;
                var lineDiscount = lineSubtotal * (itemDto.Discount / 100m);
                var taxableAmount = lineSubtotal - lineDiscount;
                var lineTax = taxableAmount * (itemDto.Tax / 100m);
                var lineTotal = taxableAmount + lineTax;

                subtotal += lineSubtotal;
                totalDiscount += lineDiscount;
                totalTax += lineTax;

                newItems.Add(new SalesReturnItem
                {
                    SalesReturnId = salesReturn.SalesReturnId,
                    ProductId = itemDto.ProductId,
                    VariantId = (itemDto.VariantId.HasValue && itemDto.VariantId.Value > 0) ? itemDto.VariantId : null,
                    InvoicedQuantity = invItem.Quantity,
                    ReturnQuantity = itemDto.ReturnQuantity,
                    Price = itemDto.Price,
                    Tax = itemDto.Tax,
                    TaxAmount = lineTax,
                    Discount = itemDto.Discount,
                    Total = lineTotal
                });
            }

            if (!newItems.Any())
            {
                return BadRequest(new { success = false, message = "At least one item must have a return quantity greater than 0." });
            }

            var grandTotal = subtotal - totalDiscount + totalTax;

            salesReturn.WarehouseId = dto.WarehouseId;
            salesReturn.ReturnDate = dto.ReturnDate ?? salesReturn.ReturnDate;
            salesReturn.TotalAmount = subtotal;
            salesReturn.TaxAmount = totalTax;
            salesReturn.DiscountAmount = totalDiscount;
            salesReturn.GrandTotal = grandTotal;
            salesReturn.RefundAmount = grandTotal;
            salesReturn.Reason = dto.Reason;
            salesReturn.Notes = dto.Notes;
            salesReturn.UpdatedAt = DateTime.UtcNow;
            salesReturn.Items = newItems;

            if (dto.SubmitForApproval)
            {
                salesReturn.Status = "Pending Approval";
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(MapToDto(salesReturn));
        }

        // ==========================================
        // 6. DELETE SALES RETURN (DRAFT ONLY)
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesReturn(int id, CancellationToken cancellationToken = default)
        {
            var salesReturn = await _context.SalesReturns
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Draft" && salesReturn.Status != "draft")
            {
                return BadRequest(new { success = false, message = $"Only returns in 'Draft' status can be deleted. Current status is '{salesReturn.Status}'." });
            }

            salesReturn.IsDeleted = true;
            salesReturn.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "Sales return deleted successfully." });
        }

        // ==========================================
        // 7. SUBMIT FOR APPROVAL (Draft -> Pending Approval)
        // ==========================================
        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitForApproval(int id, CancellationToken cancellationToken = default)
        {
            var salesReturn = await _context.SalesReturns
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Draft" && salesReturn.Status != "draft")
            {
                return BadRequest(new { success = false, message = $"Only 'Draft' returns can be submitted for approval. Current status is '{salesReturn.Status}'." });
            }

            salesReturn.Status = "Pending Approval";
            salesReturn.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "Sales return submitted for approval successfully.", status = salesReturn.Status });
        }

        // ==========================================
        // 8. APPROVE SALES RETURN (Pending Approval -> Approved)
        //    Increases Stock & Creates StockMovement ('return_in')
        // ==========================================
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveSalesReturn(int id, [FromBody] SalesReturnStatusUpdateDto? dto, CancellationToken cancellationToken = default)
        {
            var salesReturn = await _context.SalesReturns
                .Include(r => r.Items)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Pending Approval" && salesReturn.Status != "pending approval")
            {
                return BadRequest(new { success = false, message = $"Only returns in 'Pending Approval' status can be approved. Current status is '{salesReturn.Status}'." });
            }

            var now = DateTime.UtcNow;
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email) ?? "Admin";

            salesReturn.Status = "Approved";
            salesReturn.ApprovedBy = userName;
            salesReturn.ApprovedAt = now;
            salesReturn.UpdatedAt = now;

            // 1. Update Inventory & Create Stock Movement 'return_in'
            var warehouseId = salesReturn.WarehouseId ?? 1;

            foreach (var item in salesReturn.Items)
            {
                // Add Stock Movement
                _context.StockMovements.Add(new StockMovement
                {
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    WarehouseId = warehouseId,
                    MovementType = "return_in",
                    Quantity = item.ReturnQuantity,
                    ReferenceId = salesReturn.SalesReturnId,
                    ReferenceType = "sales_return",
                    Notes = $"Stock restored from approved sales return #{salesReturn.ReturnNumber}",
                    CreatedAt = now
                });

                // Update Stock table
                var stock = await _context.Stocks.FirstOrDefaultAsync(s =>
                    s.ProductId == item.ProductId &&
                    s.VariantId == item.VariantId &&
                    s.WarehouseId == warehouseId, cancellationToken);

                if (stock == null)
                {
                    _context.Stocks.Add(new Stock
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        WarehouseId = warehouseId,
                        Quantity = item.ReturnQuantity,
                        ReservedQuantity = 0m
                    });
                }
                else
                {
                    stock.Quantity += item.ReturnQuantity;
                }
            }

            // 2. Update Invoice Return Status
            if (salesReturn.Invoice != null)
            {
                // Check total returned amount across all approved/completed returns for this invoice
                var totalApprovedReturnAmount = await _context.SalesReturns
    .Where(r =>
        r.InvoiceId == salesReturn.InvoiceId &&
        r.SalesReturnId != salesReturn.SalesReturnId &&
        !r.IsDeleted &&
        (r.Status == "Approved" ||
         r.Status == "Refund Processed" ||
         r.Status == "Completed"))
    .SumAsync(r => r.GrandTotal, cancellationToken)
    + salesReturn.GrandTotal;

                if (totalApprovedReturnAmount >= salesReturn.Invoice.TotalAmount)
                {
                    salesReturn.Invoice.Status = "Fully Returned";
                }
                else
                {
                    salesReturn.Invoice.Status = "Partially Returned";
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "Sales return approved successfully. Stock updated and return_in movements created.", status = salesReturn.Status });
        }

        // ==========================================
        // 9. REJECT SALES RETURN (Pending Approval -> Rejected)
        // ==========================================
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectSalesReturn(int id, [FromBody] SalesReturnStatusUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Reason))
            {
                return BadRequest(new { success = false, message = "A rejection reason is required." });
            }

            var salesReturn = await _context.SalesReturns
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Pending Approval" && salesReturn.Status != "pending approval")
            {
                return BadRequest(new { success = false, message = $"Only returns in 'Pending Approval' status can be rejected. Current status is '{salesReturn.Status}'." });
            }

            var now = DateTime.UtcNow;
            salesReturn.Status = "Rejected";
            salesReturn.RejectionReason = dto.Reason.Trim();
            salesReturn.UpdatedAt = now;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "Sales return rejected successfully.", status = salesReturn.Status });
        }

        // ==========================================
        // 10. PROCESS REFUND (Approved -> Refund Processed)
        // ==========================================
        [HttpPost("{id}/process-refund")]
        public async Task<IActionResult> ProcessRefund(int id, [FromBody] ProcessRefundDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefundMethod))
            {
                return BadRequest(new { success = false, message = "Refund method is required." });
            }

            var salesReturn = await _context.SalesReturns
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Approved" && salesReturn.Status != "approved")
            {
                return BadRequest(new { success = false, message = $"Only approved returns can have refunds processed. Current status is '{salesReturn.Status}'." });
            }

            var now = DateTime.UtcNow;
            salesReturn.Status = "Refund Processed";
            salesReturn.RefundMethod = dto.RefundMethod.Trim();
            salesReturn.RefundReference = dto.RefundReference?.Trim();
            salesReturn.RefundDate = dto.RefundDate ?? now;
            if (dto.Amount.HasValue && dto.Amount.Value > 0)
            {
                salesReturn.RefundAmount = dto.Amount.Value;
            }
            salesReturn.UpdatedAt = now;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "Refund processed successfully.", status = salesReturn.Status });
        }

        // ==========================================
        // 11. COMPLETE WORKFLOW (Refund Processed/Approved -> Completed)
        // ==========================================
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteSalesReturn(int id, CancellationToken cancellationToken = default)
        {
            var salesReturn = await _context.SalesReturns
                .FirstOrDefaultAsync(r => r.SalesReturnId == id && !r.IsDeleted, cancellationToken);

            if (salesReturn == null)
            {
                return NotFound(new { success = false, message = "Sales return record not found." });
            }

            if (salesReturn.Status != "Refund Processed" && salesReturn.Status != "Approved")
            {
                return BadRequest(new { success = false, message = $"Only 'Approved' or 'Refund Processed' returns can be completed. Current status is '{salesReturn.Status}'." });
            }

            salesReturn.Status = "Completed";
            salesReturn.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true, message = "Sales return workflow completed and locked.", status = salesReturn.Status });
        }

        // Helper Map Method
        private static SalesReturnDto MapToDto(SalesReturn r)
        {
            return new SalesReturnDto
            {
                SalesReturnId = r.SalesReturnId,
                ReturnNumber = r.ReturnNumber ?? $"SRET-{r.SalesReturnId:D6}",
                InvoiceId = r.InvoiceId,
                InvoiceNumber = r.Invoice?.InvoiceNumber,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer?.Name,
                WarehouseId = r.WarehouseId,
                WarehouseName = r.Warehouse?.Name,
                ReturnDate = r.ReturnDate,
                TotalAmount = r.TotalAmount,
                TaxAmount = r.TaxAmount,
                DiscountAmount = r.DiscountAmount,
                GrandTotal = r.GrandTotal,
                RefundAmount = r.RefundAmount,
                Status = r.Status,
                Reason = r.Reason,
                RejectionReason = r.RejectionReason,
                ApprovedBy = r.ApprovedBy,
                ApprovedAt = r.ApprovedAt,
                RefundMethod = r.RefundMethod,
                RefundReference = r.RefundReference,
                RefundDate = r.RefundDate,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt ?? r.CreatedAt,
                Items = r.Items?.Select(i => new SalesReturnItemDto
                {
                    Id = i.Id,
                    SalesReturnId = i.SalesReturnId,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    ProductSKU = i.Product?.SKU,
                    VariantId = i.VariantId,
                    VariantName = i.Variant?.SKU,
                    InvoicedQuantity = i.InvoicedQuantity,
                    ReturnQuantity = i.ReturnQuantity,
                    Price = i.Price,
                    Tax = i.Tax,
                    TaxAmount = i.TaxAmount,
                    Discount = i.Discount,
                    Total = i.Total
                }).ToList() ?? new List<SalesReturnItemDto>()
            };
        }
    }
}
