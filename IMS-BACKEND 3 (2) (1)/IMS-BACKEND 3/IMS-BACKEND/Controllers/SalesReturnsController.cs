using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs.SalesReturns;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesReturnsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<SalesReturnsController> _logger;

        public SalesReturnsController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<SalesReturnsController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // =========================================================
        // 1. GET CUSTOMERS DROPDOWN
        // =========================================================
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersDropdown()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SalesReturnCustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.Name
                })
                .ToListAsync();

            return Ok(ApiResponse<List<SalesReturnCustomerDto>>.Ok(customers, "Customers retrieved successfully."));
        }

        // =========================================================
        // 2. GET INVOICES FOR CUSTOMER
        // =========================================================
        [HttpGet("customers/{customerId}/invoices")]
        public async Task<IActionResult> GetInvoicesForCustomer(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "A valid Customer ID is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var customerExists = await _context.Customers
                .AsNoTracking()
                .AnyAsync(c => c.CustomerId == customerId);

            if (!customerExists)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Customer record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var invoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.CustomerId == customerId && !i.IsCancelled)
                .OrderByDescending(i => i.InvoiceId)
                .Select(i => new SalesReturnInvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNumber = i.InvoiceNumber ?? $"INV-{i.InvoiceId:D6}",
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount
                })
                .ToListAsync();

            return Ok(ApiResponse<List<SalesReturnInvoiceDto>>.Ok(invoices, "Invoices retrieved successfully."));
        }

        // =========================================================
        // 3. GET INVOICE ITEMS FOR RETURN
        // =========================================================
        [HttpGet("invoices/{invoiceId}/items")]
        public async Task<IActionResult> GetInvoiceItemsForReturn(int invoiceId)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && !i.IsCancelled);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Invoice record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var invoiceItems = await (
                from item in _context.InvoiceItems.AsNoTracking()
                join p in _context.Products.AsNoTracking() on item.ProductId equals p.ProductId into products
                from p in products.DefaultIfEmpty()
                join v in _context.ProductVariants.AsNoTracking() on item.VariantId equals v.VariantId into variants
                from v in variants.DefaultIfEmpty()
                where item.InvoiceId == invoiceId && item.ProductId.HasValue
                select new
                {
                    InvoiceItemId = item.Id,
                    ProductId = item.ProductId!.Value,
                    ProductName = p != null ? p.Name : null,
                    VariantId = item.VariantId,
                    VariantName = v != null ? v.VariantName : null,
                    InvoicedQuantity = item.Quantity,
                    Price = item.Price
                }
            ).ToListAsync();

            // Calculate previous returns for this invoice to determine remaining returnable quantity
            var previousReturns = await (
                from ret in _context.SalesReturns.AsNoTracking()
                join retItem in _context.SalesReturnItems.AsNoTracking() on ret.SalesReturnId equals retItem.SalesReturnId
                where ret.InvoiceId == invoiceId
                select new
                {
                    retItem.ProductId,
                    retItem.VariantId,
                    retItem.ReturnQuantity
                }
            ).ToListAsync();

            var result = new List<SalesReturnInvoiceItemDto>();

            foreach (var item in invoiceItems)
            {
                var prevReturned = previousReturns
                    .Where(r => r.ProductId == item.ProductId && r.VariantId == item.VariantId)
                    .Sum(r => r.ReturnQuantity);

                var remaining = Math.Max(0m, item.InvoicedQuantity - prevReturned);

                result.Add(new SalesReturnInvoiceItemDto
                {
                    InvoiceItemId = item.InvoiceItemId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    VariantId = item.VariantId,
                    VariantName = item.VariantName,
                    InvoicedQuantity = item.InvoicedQuantity,
                    Price = item.Price,
                    PreviousReturnedQuantity = prevReturned,
                    RemainingReturnableQuantity = remaining
                });
            }

            return Ok(ApiResponse<List<SalesReturnInvoiceItemDto>>.Ok(result, "Invoice items retrieved successfully."));
        }

        // =========================================================
        // 4. CREATE SALES RETURN
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateSalesReturn([FromBody] CreateSalesReturnDto dto)
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

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

            if (customer == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Selected customer was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == dto.InvoiceId && !i.IsCancelled);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Selected invoice was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (invoice.CustomerId != dto.CustomerId)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected invoice does not belong to the selected customer.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Fetch invoice items for validation & authoritative price
            var invoiceItems = await _context.InvoiceItems
                .AsNoTracking()
                .Where(ii => ii.InvoiceId == dto.InvoiceId && ii.ProductId.HasValue)
                .ToListAsync();

            // Fetch previous returns to calculate remaining returnable quantity per item
            var previousReturnItems = await (
                from ret in _context.SalesReturns.AsNoTracking()
                join retItem in _context.SalesReturnItems.AsNoTracking() on ret.SalesReturnId equals retItem.SalesReturnId
                where ret.InvoiceId == dto.InvoiceId
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

                var salesReturn = new SalesReturn
                {
                    ReturnNumber = returnNumber,
                    CustomerId = dto.CustomerId,
                    InvoiceId = dto.InvoiceId,
                    ReturnDate = dto.ReturnDate == default ? now.Date : dto.ReturnDate,
                    Reason = dto.Reason.Trim(),
                    Status = "Draft",
                    CreatedAt = now
                };

                _context.SalesReturns.Add(salesReturn);
                await _context.SaveChangesAsync();

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

                    var matchingInvoiceItem = invoiceItems.FirstOrDefault(ii =>
                        ii.ProductId == itemDto.ProductId &&
                        ii.VariantId == itemDto.VariantId);

                    if (matchingInvoiceItem == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Product ID {itemDto.ProductId} (Variant ID: {itemDto.VariantId?.ToString() ?? "N/A"}) is not present in the selected invoice.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var invoicedQty = matchingInvoiceItem.Quantity;
                    var price = matchingInvoiceItem.Price;

                    var prevReturnedQty = previousReturnItems
                        .Where(r => r.ProductId == itemDto.ProductId && r.VariantId == itemDto.VariantId)
                        .Sum(r => r.ReturnQuantity);

                    var remainingReturnableQty = Math.Max(0m, invoicedQty - prevReturnedQty);

                    if (itemDto.ReturnQuantity > remainingReturnableQty)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Return quantity ({itemDto.ReturnQuantity}) for Product ID {itemDto.ProductId} exceeds remaining returnable quantity ({remainingReturnableQty}).",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var lineTotal = itemDto.ReturnQuantity * price;
                    totalReturnAmount += lineTotal;

                    var returnItem = new SalesReturnItem
                    {
                        SalesReturnId = salesReturn.SalesReturnId,
                        ProductId = itemDto.ProductId,
                        VariantId = itemDto.VariantId,
                        InvoicedQuantity = invoicedQty,
                        ReturnQuantity = itemDto.ReturnQuantity,
                        Price = price,
                        Total = lineTotal,
                        CreatedAt = now
                    };

                    _context.SalesReturnItems.Add(returnItem);
                }

                salesReturn.TotalReturnAmount = totalReturnAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Log audit trail
                await _auditLogService.LogAsync(
                    "Create",
                    "Sales Return",
                    salesReturn.SalesReturnId,
                    $"Sales Return {returnNumber} created for Customer {customer.Name}",
                    "sales_returns");

                var responseDto = await BuildSalesReturnDetailsDtoAsync(salesReturn.SalesReturnId);

                return Ok(ApiResponse<SalesReturnDetailsDto>.Ok(
                    responseDto,
                    "Sales return created successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create sales return for Invoice {InvoiceId}", dto.InvoiceId);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An error occurred while processing the sales return.",
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        // =========================================================
        // 5. GET SALES RETURNS LIST
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetSalesReturns([FromQuery] string? search, [FromQuery] int? customerId)
        {
            var query = _context.SalesReturns
                .AsNoTracking()
                .AsQueryable();

            if (customerId.HasValue && customerId.Value > 0)
            {
                query = query.Where(r => r.CustomerId == customerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(r =>
                    r.ReturnNumber.ToLower().Contains(term) ||
                    r.Reason.ToLower().Contains(term) ||
                    (r.Customer != null && r.Customer.Name != null && r.Customer.Name.ToLower().Contains(term)) ||
                    (r.Invoice != null && r.Invoice.InvoiceNumber != null && r.Invoice.InvoiceNumber.ToLower().Contains(term)));
            }

            var list = await (
                from r in query
                join c in _context.Customers.AsNoTracking() on r.CustomerId equals c.CustomerId into customers
                from c in customers.DefaultIfEmpty()
                join inv in _context.Invoices.AsNoTracking() on r.InvoiceId equals inv.InvoiceId into invoices
                from inv in invoices.DefaultIfEmpty()
                orderby r.SalesReturnId descending
                select new SalesReturnListDto
                {
                    SalesReturnId = r.SalesReturnId,
                    ReturnNumber = r.ReturnNumber,
                    InvoiceId = r.InvoiceId,
                    InvoiceNumber = inv != null ? (inv.InvoiceNumber ?? $"INV-{inv.InvoiceId:D6}") : null,
                    CustomerId = r.CustomerId,
                    CustomerName = c != null ? c.Name : null,
                    ReturnDate = r.ReturnDate,
                    TotalReturnAmount = r.TotalReturnAmount,
                    Reason = r.Reason,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                }
            ).ToListAsync();

            return Ok(ApiResponse<List<SalesReturnListDto>>.Ok(list, "Sales returns retrieved successfully."));
        }

        // =========================================================
        // 6. GET SALES RETURN BY ID
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesReturnById(int id)
        {
            var responseDto = await BuildSalesReturnDetailsDtoAsync(id);

            if (responseDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Sales return record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<SalesReturnDetailsDto>.Ok(responseDto, "Sales return retrieved successfully."));
        }

        // =========================================================
        // 7. UPDATE SALES RETURN
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesReturn(int id, [FromBody] UpdateSalesReturnDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid input data.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var salesReturn = await _context.SalesReturns
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.SalesReturnId == id);

            if (salesReturn == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Sales return record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (string.Equals(salesReturn.Status, "Completed", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(salesReturn.Status, "Finalized", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Completed or finalized sales returns cannot be edited.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

            if (customer == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Selected customer was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == dto.InvoiceId && !i.IsCancelled);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Selected invoice was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (invoice.CustomerId != dto.CustomerId)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected invoice does not belong to the selected customer.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var invoiceItems = await _context.InvoiceItems
                .AsNoTracking()
                .Where(ii => ii.InvoiceId == dto.InvoiceId && ii.ProductId.HasValue)
                .ToListAsync();

            var otherReturnItems = await (
                from ret in _context.SalesReturns.AsNoTracking()
                join retItem in _context.SalesReturnItems.AsNoTracking() on ret.SalesReturnId equals retItem.SalesReturnId
                where ret.InvoiceId == dto.InvoiceId && ret.SalesReturnId != id
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

                salesReturn.CustomerId = dto.CustomerId;
                salesReturn.InvoiceId = dto.InvoiceId;
                salesReturn.ReturnDate = dto.ReturnDate == default ? salesReturn.ReturnDate : dto.ReturnDate;
                salesReturn.Reason = dto.Reason.Trim();
                salesReturn.UpdatedAt = now;

                _context.SalesReturnItems.RemoveRange(salesReturn.Items);

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

                    var matchingInvoiceItem = invoiceItems.FirstOrDefault(ii =>
                        ii.ProductId == itemDto.ProductId &&
                        ii.VariantId == itemDto.VariantId);

                    if (matchingInvoiceItem == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Product ID {itemDto.ProductId} (Variant ID: {itemDto.VariantId?.ToString() ?? "N/A"}) is not present in the selected invoice.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var invoicedQty = matchingInvoiceItem.Quantity;
                    var price = matchingInvoiceItem.Price;

                    var prevReturnedQty = otherReturnItems
                        .Where(r => r.ProductId == itemDto.ProductId && r.VariantId == itemDto.VariantId)
                        .Sum(r => r.ReturnQuantity);

                    var remainingReturnableQty = Math.Max(0m, invoicedQty - prevReturnedQty);

                    if (itemDto.ReturnQuantity > remainingReturnableQty)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Return quantity ({itemDto.ReturnQuantity}) for Product ID {itemDto.ProductId} exceeds remaining returnable quantity ({remainingReturnableQty}).",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    var lineTotal = itemDto.ReturnQuantity * price;
                    totalReturnAmount += lineTotal;

                    _context.SalesReturnItems.Add(new SalesReturnItem
                    {
                        SalesReturnId = salesReturn.SalesReturnId,
                        ProductId = itemDto.ProductId,
                        VariantId = itemDto.VariantId,
                        InvoicedQuantity = invoicedQty,
                        ReturnQuantity = itemDto.ReturnQuantity,
                        Price = price,
                        Total = lineTotal,
                        CreatedAt = now
                    });
                }

                salesReturn.TotalReturnAmount = totalReturnAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _auditLogService.LogAsync(
                    "Update",
                    "Sales Return",
                    salesReturn.SalesReturnId,
                    $"Sales Return {salesReturn.ReturnNumber} updated",
                    "sales_returns");

                var responseDto = await BuildSalesReturnDetailsDtoAsync(salesReturn.SalesReturnId);

                return Ok(ApiResponse<SalesReturnDetailsDto>.Ok(
                    responseDto,
                    "Sales return updated successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to update sales return ID {SalesReturnId}", id);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An error occurred while updating the sales return.",
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        // =========================================================
        // 8. DELETE SALES RETURN
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesReturn(int id)
        {
            var salesReturn = await _context.SalesReturns
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.SalesReturnId == id);

            if (salesReturn == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Sales return record was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (string.Equals(salesReturn.Status, "Finalized", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Finalized sales returns cannot be deleted.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.SalesReturnItems.RemoveRange(salesReturn.Items);
                _context.SalesReturns.Remove(salesReturn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _auditLogService.LogAsync(
                    "Delete",
                    "Sales Return",
                    id,
                    $"Sales Return {salesReturn.ReturnNumber} deleted",
                    "sales_returns");

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Sales return deleted successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to delete sales return ID {SalesReturnId}", id);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An error occurred while deleting the sales return.",
                    traceId: HttpContext.TraceIdentifier));
            }
        }

        // =========================================================
        // PRIVATE HELPER METHODS
        // =========================================================

        private async Task<string> GenerateUniqueReturnNumberAsync()
        {
            var maxId = await _context.SalesReturns.MaxAsync(r => (int?)r.SalesReturnId) ?? 0;
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var candidate = $"SRR-{(maxId + 1 + attempt):D6}";

                var exists = await _context.SalesReturns
                    .AnyAsync(r => r.ReturnNumber == candidate);

                if (!exists)
                {
                    return candidate;
                }
            }

            return $"SRR-{DateTime.UtcNow.Ticks.ToString()[^6..]}";
        }

        private async Task<SalesReturnDetailsDto?> BuildSalesReturnDetailsDtoAsync(int salesReturnId)
        {
            var header = await (
                from r in _context.SalesReturns.AsNoTracking()
                join c in _context.Customers.AsNoTracking() on r.CustomerId equals c.CustomerId into customers
                from c in customers.DefaultIfEmpty()
                join inv in _context.Invoices.AsNoTracking() on r.InvoiceId equals inv.InvoiceId into invoices
                from inv in invoices.DefaultIfEmpty()
                where r.SalesReturnId == salesReturnId
                select new
                {
                    r.SalesReturnId,
                    r.ReturnNumber,
                    r.CustomerId,
                    CustomerName = c != null ? c.Name : null,
                    r.InvoiceId,
                    InvoiceNumber = inv != null ? (inv.InvoiceNumber ?? $"INV-{inv.InvoiceId:D6}") : null,
                    r.ReturnDate,
                    r.Reason,
                    r.TotalReturnAmount,
                    r.Status
                }
            ).FirstOrDefaultAsync();

            if (header == null)
            {
                return null;
            }

            var items = await (
                from item in _context.SalesReturnItems.AsNoTracking()
                join p in _context.Products.AsNoTracking() on item.ProductId equals p.ProductId into products
                from p in products.DefaultIfEmpty()
                join v in _context.ProductVariants.AsNoTracking() on item.VariantId equals v.VariantId into variants
                from v in variants.DefaultIfEmpty()
                where item.SalesReturnId == salesReturnId
                select new SalesReturnItemDetailsDto
                {
                    SalesReturnItemId = item.SalesReturnItemId,
                    ProductId = item.ProductId,
                    ProductName = p != null ? p.Name : null,
                    VariantId = item.VariantId,
                    VariantName = v != null ? v.VariantName : null,
                    InvoicedQuantity = item.InvoicedQuantity,
                    ReturnQuantity = item.ReturnQuantity,
                    Price = item.Price,
                    Total = item.Total
                }
            ).ToListAsync();

            return new SalesReturnDetailsDto
            {
                SalesReturnId = header.SalesReturnId,
                ReturnNumber = header.ReturnNumber,
                CustomerId = header.CustomerId,
                CustomerName = header.CustomerName,
                InvoiceId = header.InvoiceId,
                InvoiceNumber = header.InvoiceNumber,
                ReturnDate = header.ReturnDate,
                Reason = header.Reason,
                TotalReturnAmount = header.TotalReturnAmount,
                Status = header.Status,
                Items = items
            };
        }
    }
}
