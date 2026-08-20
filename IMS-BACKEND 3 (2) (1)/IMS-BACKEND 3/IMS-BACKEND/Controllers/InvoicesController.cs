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
    public class InvoicesController : ControllerBase
    {
        private const string InvoiceSaleMovementType = "sale";

        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly PdfService _pdfService;
        private readonly EmailService _emailService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(
            AppDbContext context,
            AuditLogService auditLogService,
            PdfService pdfService,
            EmailService emailService,
            ILogger<InvoicesController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _pdfService = pdfService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices(
            int page = 1,
            int pageSize = 500,
            string? search = null,
            string sortBy = "invoiceId",
            string sortOrder = "desc",
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 500);

            var query =
                from invoice in _context.Invoices.AsNoTracking()
                join customer in _context.Customers.AsNoTracking()
                    on invoice.CustomerId equals customer.CustomerId into customerGroup
                from customer in customerGroup.DefaultIfEmpty()
                where !invoice.IsCancelled
                select new
                {
                    Id = invoice.InvoiceId,
                    invoice.InvoiceId,
                    invoice.SoId,
                    invoice.CustomerId,
                    CustomerName = customer != null ? customer.Name : "No customer",
                    CustomerEmail = customer != null ? customer.Email : null,
                    invoice.InvoiceNumber,
                    invoice.InvoiceDate,
                    invoice.DueDate,
                    invoice.Status,
                    invoice.TotalAmount,
                    invoice.PaidAmount,
                    invoice.BalanceAmount,
                    PaymentMethod = _context.CustomerPayments
                        .Where(p => p.InvoiceId == invoice.InvoiceId && !p.IsCancelled)
                        .OrderBy(p => p.PaymentId)
                        .Select(p => p.PaymentMethod)
                        .FirstOrDefault() ?? "N/A",
                    ReturnedAmount = _context.SalesReturns
                        .Where(returnItem =>
                            returnItem.InvoiceId == invoice.InvoiceId &&
                            (returnItem.Status == "Processed" || returnItem.Status == "Refunded"))
                        .Sum(returnItem => (decimal?)returnItem.TotalReturnAmount) ?? 0,
                    ReturnStatus = (_context.SalesReturns
                        .Where(returnItem =>
                            returnItem.InvoiceId == invoice.InvoiceId &&
                            (returnItem.Status == "Processed" || returnItem.Status == "Refunded"))
                        .Sum(returnItem => (decimal?)returnItem.TotalReturnAmount) ?? 0) <= 0
                        ? null
                        : ((_context.SalesReturns
                            .Where(returnItem =>
                                returnItem.InvoiceId == invoice.InvoiceId &&
                                (returnItem.Status == "Processed" || returnItem.Status == "Refunded"))
                            .Sum(returnItem => (decimal?)returnItem.TotalReturnAmount) ?? 0) >= invoice.TotalAmount
                            ? "Fully Returned"
                            : "Partially Returned"),
                    ItemCount = _context.InvoiceItems.Count(item => item.InvoiceId == invoice.InvoiceId)
                };

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLowerInvariant();

                query = query.Where(invoice =>
                    (invoice.InvoiceNumber ?? string.Empty).ToLower().Contains(normalizedSearch) ||
                    invoice.CustomerName.ToLower().Contains(normalizedSearch) ||
                    (invoice.Status ?? string.Empty).ToLower().Contains(normalizedSearch));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            query = (sortBy.Trim().ToLowerInvariant(), sortOrder.Trim().ToLowerInvariant()) switch
            {
                ("invoice", "asc") => query.OrderBy(invoice => invoice.InvoiceNumber),
                ("invoice", "desc") => query.OrderByDescending(invoice => invoice.InvoiceNumber),
                ("customer", "asc") => query.OrderBy(invoice => invoice.CustomerName),
                ("customer", "desc") => query.OrderByDescending(invoice => invoice.CustomerName),
                ("amount", "asc") => query.OrderBy(invoice => invoice.TotalAmount),
                ("amount", "desc") => query.OrderByDescending(invoice => invoice.TotalAmount),
                ("date", "asc") => query.OrderBy(invoice => invoice.InvoiceDate),
                ("date", "desc") => query.OrderByDescending(invoice => invoice.InvoiceDate),
                _ => query.OrderByDescending(invoice => invoice.InvoiceId)
            };

            var invoices = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                new
                {
                    page,
                    pageSize,
                    totalRecords,
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    data = invoices
                },
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(
            [FromBody] InvoiceDto dto,
            CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invoice payload is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.CustomerId.GetValueOrDefault() <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Customer is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var customerId = dto.CustomerId.GetValueOrDefault();

            if (dto.Items == null || dto.Items.Count == 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "At least one invoice item is required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var lines = dto.Items
    .Select(item => new InvoiceLineDraft(
        item.ProductId,
        item.VariantId,
        item.Quantity,
        item.Price,
        item.TaxPercent,
        item.TaxAmount))
    .ToList();

            var invalidLine = lines.FirstOrDefault(item =>
                item.ProductId <= 0 ||
                item.Quantity <= 0 ||
                item.Price < 0);

            if (invalidLine != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Each invoice item must include a valid product, quantity greater than zero, and non-negative unit price.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var total = lines.Sum(item =>
    (item.Quantity * item.Price) + item.TaxAmount);

            if (total <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invoice total must be greater than zero.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.PaidAmount < 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Paid amount cannot be negative.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (dto.PaidAmount > total)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Paid amount cannot exceed invoice total.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var openingPaymentReference = dto.ReferenceNumber?.Trim();
            if (dto.PaidAmount > 0 && !string.IsNullOrWhiteSpace(openingPaymentReference))
            {
                var normalizedReference = openingPaymentReference.ToLowerInvariant();
                var referenceExists = await _context.CustomerPayments
                    .AsNoTracking()
                    .AnyAsync(payment =>
                        !payment.IsCancelled &&
                        payment.ReferenceNumber != null &&
                        payment.ReferenceNumber.ToLower() == normalizedReference,
                        cancellationToken);

                if (referenceExists)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Payment reference number already exists.",
                        traceId: HttpContext.TraceIdentifier));
                }
            }

            var invoiceDate = dto.InvoiceDate ?? DateTime.UtcNow;

            if (dto.DueDate.HasValue && dto.DueDate.Value.Date < invoiceDate.Date)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Due date cannot be before invoice date.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(item => item.CustomerId == customerId, cancellationToken);

            if (customer == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Selected customer was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var productIds = lines
                .Select(item => item.ProductId)
                .Distinct()
                .ToList();

            var productsById = await _context.Products
                .Where(product => productIds.Contains(product.ProductId) && !product.IsDeleted)
                .ToDictionaryAsync(product => product.ProductId, cancellationToken);

            var missingProductId = productIds.FirstOrDefault(productId => !productsById.ContainsKey(productId));
            if (missingProductId > 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    $"Selected product {missingProductId} was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var variantIds = lines
                .Where(item => item.VariantId.HasValue)
                .Select(item => item.VariantId!.Value)
                .Distinct()
                .ToList();

            if (variantIds.Count > 0)
            {
                var variants = await _context.ProductVariants
                    .Where(variant => variantIds.Contains(variant.VariantId))
                    .ToListAsync(cancellationToken);

                foreach (var line in lines.Where(item => item.VariantId.HasValue))
                {
                    var variant = variants.FirstOrDefault(item => item.VariantId == line.VariantId!.Value);

                    if (variant == null || variant.ProductId != line.ProductId)
                    {
                        return BadRequest(ApiResponse<object>.Fail(
                            $"Selected variant {line.VariantId} is not valid for product {line.ProductId}.",
                            traceId: HttpContext.TraceIdentifier));
                    }
                }
            }

            foreach (var demand in lines
                .GroupBy(item => new { item.ProductId, item.VariantId })
                .Select(group => new StockDemand(
                    group.Key.ProductId,
                    group.Key.VariantId,
                    group.Sum(item => item.Quantity))))
            {
                var stockQuery = _context.Stocks
                    .Where(stock => stock.ProductId == demand.ProductId);

                if (demand.VariantId.HasValue)
                {
                    stockQuery = stockQuery.Where(stock => stock.VariantId == demand.VariantId);
                }

                var availableQuantity = await stockQuery
                    .SumAsync(stock => (decimal?)stock.Quantity, cancellationToken) ?? 0;

                if (availableQuantity < demand.Quantity)
                {
                    var productName = productsById[demand.ProductId].Name;

                    return BadRequest(ApiResponse<object>.Fail(
                        $"Insufficient stock for {productName}. Available: {availableQuantity}, requested: {demand.Quantity}.",
                        traceId: HttpContext.TraceIdentifier));
                }
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTime.UtcNow;
                var balance = total - dto.PaidAmount;
                var status = ResolveInvoiceStatus(dto.PaidAmount, balance, dto.DueDate);
                var invoiceNumber = await GenerateInvoiceNumber(cancellationToken);
                var outstandingBeforeInvoice = customer.OutstandingBalance;
                var outstandingAfterInvoiceDebit = outstandingBeforeInvoice + total;
                var outstandingAfterOpeningPayment = outstandingBeforeInvoice + balance;

                var invoice = new Invoice
                {
                    SoId = dto.SoId,
                    CustomerId = customer.CustomerId,
                    InvoiceNumber = invoiceNumber,
                    InvoiceDate = invoiceDate,
                    DueDate = dto.DueDate,
                    Status = status,
                    TotalAmount = total,
                    PaidAmount = dto.PaidAmount,
                    BalanceAmount = balance
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var line in lines)
                {
                    var itemTotal = line.Quantity * line.Price;

                    _context.InvoiceItems.Add(new InvoiceItem
                    {
                        InvoiceId = invoice.InvoiceId,
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        Quantity = line.Quantity,
                        Price = line.Price,
                        Total = itemTotal,
                        TaxPercent = line.TaxPercent,
                        TaxAmount = line.TaxAmount
                    });

                    await DeductStockForInvoiceLine(
                        invoice,
                        line,
                        productsById[line.ProductId],
                        now,
                        cancellationToken);
                }

                foreach (var productId in productIds)
                {
                    productsById[productId].UpdatedAt = now;
                }

                customer.OutstandingBalance = outstandingAfterOpeningPayment;
                customer.UpdatedAt = now;

                _context.CustomerLedgers.Add(new CustomerLedger
                {
                    CustomerId = customer.CustomerId,
                    TransactionType = "invoice",
                    TransactionId = invoice.InvoiceId,
                    Debit = total,
                    Credit = 0,
                    Balance = outstandingAfterInvoiceDebit,
                    CreatedAt = now
                });

                CustomerPayment? openingPayment = null;
                if (dto.PaidAmount > 0)
                {
                    // PaidAmount entered during invoice creation is a real receipt.
                    // Persist it as a customer payment so Payments, invoice status,
                    // ledger, and customer balance all use the same transaction record.
                    openingPayment = new CustomerPayment
                    {
                        CustomerId = customer.CustomerId,
                        InvoiceId = invoice.InvoiceId,
                        Amount = dto.PaidAmount,
                        PaymentDate = invoiceDate,
                        PaymentMethod = string.IsNullOrWhiteSpace(dto.PaymentMethod)
                            ? "Bank Transfer"
                            : dto.PaymentMethod.Trim(),
                        ReferenceNumber = openingPaymentReference,
                        Notes = $"Opening payment recorded during invoice {invoice.InvoiceNumber} creation."
                    };

                    _context.CustomerPayments.Add(openingPayment);

                    _context.CustomerActivities.Add(new CustomerActivity
                    {
                        CustomerId = customer.CustomerId,
                        ActivityType = "PAYMENT",
                        Description = $"Opening payment of {dto.PaidAmount:0.00} recorded for invoice {invoice.InvoiceNumber}",
                        CreatedAt = now
                    });
                }

                _context.CustomerActivities.Add(new CustomerActivity
                {
                    CustomerId = customer.CustomerId,
                    ActivityType = "INVOICE",
                    Description = $"Invoice {invoice.InvoiceNumber} created",
                    CreatedAt = now
                });

                _context.Notifications.Add(new Notification
                {
                    Title = "Invoice Created",
                    Message = $"Invoice {invoice.InvoiceNumber} created for {customer.Name}.",
                    Type = balance > 0 ? "action" : "info",
                    IsRead = false,
                    CreatedAt = now
                });

                await _auditLogService.LogAsync(
                    "CREATE_INVOICE",
                    "Sales",
                    invoice.InvoiceId,
                    $"Invoice {invoice.InvoiceNumber} created",
                    "invoices",
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                if (openingPayment != null)
                {
                    _context.CustomerLedgers.Add(new CustomerLedger
                    {
                        CustomerId = customer.CustomerId,
                        TransactionType = "payment",
                        TransactionId = openingPayment.PaymentId,
                        Debit = 0,
                        Credit = dto.PaidAmount,
                        Balance = outstandingAfterOpeningPayment,
                        CreatedAt = now
                    });

                    await _auditLogService.LogAsync(
                        "CUSTOMER_PAYMENT_CREATED_FROM_INVOICE",
                        "Payments",
                        openingPayment.PaymentId,
                        $"Opening payment recorded for invoice {invoice.InvoiceNumber}",
                        "customer_payments",
                        cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);

                var createdInvoice = await GetInvoiceResponseAsync(invoice.InvoiceId, cancellationToken);

                return CreatedAtAction(
                    nameof(GetInvoice),
                    new { id = invoice.InvoiceId },
                    ApiResponse<object>.Ok(
                        createdInvoice,
                        "Invoice created successfully.",
                        HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Invoice create failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(
                    exception,
                    "Invoice create failed. InnerException: {InnerException}. TraceId: {TraceId}",
                    exception.InnerException?.ToString() ?? "No inner exception",
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoice(int id, CancellationToken cancellationToken)
        {
            var invoice = await GetInvoiceResponseAsync(id, cancellationToken);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Invoice not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(
                invoice,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInvoice(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
            => await CancelInvoice(id, dto, cancellationToken);

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelInvoice(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(item => item.InvoiceItems!)
                .FirstOrDefaultAsync(item => item.InvoiceId == id, cancellationToken);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Invoice not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (invoice.IsCancelled)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invoice is already cancelled.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var hasPayments = await _context.CustomerPayments
                .AsNoTracking()
                .AnyAsync(item =>
                    item.InvoiceId == id &&
                    !item.IsCancelled,
                    cancellationToken);

            if (hasPayments || invoice.PaidAmount > 0)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Invoice has payment history. Void related payments before cancelling the invoice.",
                    traceId: HttpContext.TraceIdentifier));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTime.UtcNow;
                var movements = await _context.StockMovements
                    .Where(item =>
                        item.ReferenceId == id &&
                        item.ReferenceType == "invoice")
                    .ToListAsync(cancellationToken);

                foreach (var movement in movements)
                {
                    var stock = await _context.Stocks
                        .FirstOrDefaultAsync(item =>
                            item.ProductId == movement.ProductId &&
                            item.VariantId == movement.VariantId &&
                            item.WarehouseId == movement.WarehouseId,
                            cancellationToken);

                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            ProductId = movement.ProductId,
                            VariantId = movement.VariantId,
                            WarehouseId = movement.WarehouseId,
                            Quantity = 0,
                            ReservedQuantity = 0
                        };

                        _context.Stocks.Add(stock);
                    }

                    var openingQty = stock.Quantity;
                    stock.Quantity += movement.Quantity;
                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = movement.ProductId,
                        VariantId = movement.VariantId,
                        WarehouseId = movement.WarehouseId,
                        MovementType = movement.MovementType,
                        Quantity = -movement.Quantity,
                        ReferenceId = invoice.InvoiceId,
                        ReferenceType = "invoice_reversal",
                        Notes = $"Reversal for invoice {invoice.InvoiceNumber}",
                        CreatedAt = now
                    });

                    _context.StockLedgers.Add(new StockLedger
                    {
                        ProductId = movement.ProductId,
                        VariantId = movement.VariantId,
                        WarehouseId = movement.WarehouseId,
                        OpeningQty = openingQty,
                        ChangeQty = movement.Quantity,
                        ClosingQty = stock.Quantity,
                        TransactionType = "invoice_reversal",
                        TransactionId = invoice.InvoiceId,
                        CreatedAt = now
                    });
                }

                var itemProductIds = invoice.InvoiceItems?
                    .Where(item => item.ProductId.HasValue)
                    .Select(item => item.ProductId!.Value)
                    .Distinct()
                    .ToList() ?? new List<int>();

                if (itemProductIds.Count > 0)
                {
                    var products = await _context.Products
                        .Where(product => itemProductIds.Contains(product.ProductId))
                        .ToListAsync(cancellationToken);

                    foreach (var product in products)
                    {
                        product.UpdatedAt = now;
                    }
                }

                if (invoice.CustomerId.HasValue)
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(item => item.CustomerId == invoice.CustomerId.Value, cancellationToken);

                    if (customer != null)
                    {
                        customer.OutstandingBalance = Math.Max(
                            customer.OutstandingBalance - invoice.BalanceAmount,
                            0);
                        customer.UpdatedAt = now;
                    }

                    _context.CustomerLedgers.Add(new CustomerLedger
                    {
                        CustomerId = customer?.CustomerId ?? invoice.CustomerId.Value,
                        TransactionType = "invoice_cancel",
                        TransactionId = invoice.InvoiceId,
                        Debit = 0,
                        Credit = invoice.BalanceAmount,
                        Balance = customer?.OutstandingBalance ?? 0,
                        CreatedAt = now
                    });
                }

                invoice.Status = "Cancelled";
                invoice.IsCancelled = true;
                invoice.CancelledAt = now;
                invoice.CancellationReason = dto?.Reason;

                await _auditLogService.LogAsync(
                    "DELETE_INVOICE",
                    "Sales",
                    invoice.InvoiceId,
                    $"Invoice {invoice.InvoiceNumber} cancelled",
                    "invoices",
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(ApiResponse<object>.Ok(
                    new { invoiceId = id },
                    "Invoice cancelled successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Invoice cancel failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(
                    exception,
                    "Invoice cancel failed. InnerException: {InnerException}. TraceId: {TraceId}",
                    exception.InnerException?.ToString() ?? "No inner exception",
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
        }

        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> DownloadInvoicePdf(int id, CancellationToken cancellationToken)
        {
            var invoice = await FindInvoiceWithDetailsAsync(id, cancellationToken);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Invoice not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);

            return File(
                pdfBytes,
                "application/pdf",
                $"{invoice.InvoiceNumber}.pdf");
        }

        [HttpPost("{id:int}/send-email")]
        public async Task<IActionResult> SendInvoiceEmail(int id, CancellationToken cancellationToken)
        {
            var invoice = await FindInvoiceWithDetailsAsync(id, cancellationToken);

            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Invoice not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (invoice.Customer == null ||
                string.IsNullOrWhiteSpace(invoice.Customer.Email))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Customer email not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            try
            {
                var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
                var body = $@"
                    <h2>Invoice from IMS</h2>
                    <p>Dear {invoice.Customer.Name},</p>
                    <p>Please find attached your invoice <b>{invoice.InvoiceNumber}</b>.</p>
                    <p>Total Amount: {invoice.TotalAmount}</p>
                    <br/>
                    <p>Thank you for doing business with us.</p>";

                await _emailService.SendEmailAsync(
                    invoice.Customer.Email,
                    $"Invoice {invoice.InvoiceNumber}",
                    body,
                    pdfBytes,
                    $"{invoice.InvoiceNumber}.pdf");

                return Ok(ApiResponse<object>.Ok(
                    new { invoice.InvoiceId, invoice.InvoiceNumber },
                    "Invoice email sent successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Invoice email send failed. InnerException: {InnerException}. TraceId: {TraceId}",
                    exception.InnerException?.ToString() ?? "No inner exception",
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        $"Invoice email could not be sent. {GetInnermostMessage(exception)}",
                        traceId: HttpContext.TraceIdentifier));
            }
        }

        private async Task DeductStockForInvoiceLine(
            Invoice invoice,
            InvoiceLineDraft line,
            Product product,
            DateTime createdAt,
            CancellationToken cancellationToken)
        {
            var remainingQuantity = line.Quantity;
            var stockQuery = _context.Stocks
                .Where(stock =>
                    stock.ProductId == line.ProductId &&
                    stock.Quantity > 0);

            if (line.VariantId.HasValue)
            {
                stockQuery = stockQuery.Where(stock => stock.VariantId == line.VariantId);
            }

            var stocks = await stockQuery
                .OrderByDescending(stock => stock.Quantity)
                .ThenBy(stock => stock.WarehouseId)
                .ToListAsync(cancellationToken);

            foreach (var stock in stocks)
            {
                if (remainingQuantity <= 0)
                {
                    break;
                }

                var deductedQuantity = Math.Min(stock.Quantity, remainingQuantity);
                var openingQty = stock.Quantity;

                stock.Quantity -= deductedQuantity;
                remainingQuantity -= deductedQuantity;

                _context.StockMovements.Add(new StockMovement
                {
                    ProductId = line.ProductId,
                    VariantId = stock.VariantId,
                    WarehouseId = stock.WarehouseId,
                    MovementType = InvoiceSaleMovementType,
                    Quantity = deductedQuantity,
                    ReferenceId = invoice.InvoiceId,
                    ReferenceType = "invoice",
                    Notes = $"Stock deducted for invoice {invoice.InvoiceNumber}",
                    CreatedAt = createdAt
                });

                _context.StockLedgers.Add(new StockLedger
                {
                    ProductId = line.ProductId,
                    VariantId = stock.VariantId,
                    WarehouseId = stock.WarehouseId,
                    OpeningQty = openingQty,
                    ChangeQty = -deductedQuantity,
                    ClosingQty = stock.Quantity,
                    TransactionType = InvoiceSaleMovementType,
                    TransactionId = invoice.InvoiceId,
                    CreatedAt = createdAt
                });

                if (stock.Quantity <= (decimal)(product.ReorderLevel ?? 10))
                {
                    _context.Notifications.Add(new Notification
                    {
                        Title = "Low Stock Alert",
                        Message = $"{product.Name} stock is low. Remaining quantity: {stock.Quantity}",
                        Type = "warning",
                        IsRead = false,
                        CreatedAt = createdAt
                    });
                }
            }

            if (remainingQuantity > 0)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for {product.Name}. Remaining shortage: {remainingQuantity}.");
            }
        }

        private async Task<object?> GetInvoiceResponseAsync(int id, CancellationToken cancellationToken)
        {
            var invoice = await FindInvoiceWithDetailsAsync(id, cancellationToken);

            return invoice == null ? null : await ToInvoiceResponse(invoice, cancellationToken);
        }

        private async Task<Invoice?> FindInvoiceWithDetailsAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Include(invoice => invoice.Customer)
                .Include(invoice => invoice.InvoiceItems!)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(invoice => invoice.InvoiceId == id, cancellationToken);
        }

        private async Task<object> ToInvoiceResponse(Invoice invoice, CancellationToken cancellationToken)
        {
            var returnRows = await (
                from salesReturn in _context.SalesReturns.AsNoTracking()
                join returnItem in _context.SalesReturnItems.AsNoTracking()
                    on salesReturn.SalesReturnId equals returnItem.SalesReturnId
                where salesReturn.InvoiceId == invoice.InvoiceId &&
                    (salesReturn.Status == "Processed" || salesReturn.Status == "Refunded")
                select new
                {
                    ReturnId = salesReturn.SalesReturnId,
                    ReturnNumber = string.IsNullOrEmpty(salesReturn.ReturnNumber) ? $"RET-{salesReturn.SalesReturnId:000}" : salesReturn.ReturnNumber,
                    salesReturn.Status,
                    TotalAmount = (decimal?)salesReturn.TotalReturnAmount,
                    returnItem.ProductId,
                    returnItem.VariantId,
                    Quantity = (decimal?)returnItem.ReturnQuantity,
                    Price = (decimal?)returnItem.Price
                }
            ).ToListAsync(cancellationToken);

            var items = invoice.InvoiceItems?
                .Select(item =>
                {
                    var matchingReturns = returnRows
                        .Where(returnItem =>
                            returnItem.ProductId == item.ProductId &&
                            returnItem.VariantId == item.VariantId)
                        .ToList();
                    var returnedQuantity = matchingReturns.Sum(returnItem => returnItem.Quantity ?? 0);
                    var returnReferences = matchingReturns
                        .Select(returnItem => returnItem.ReturnNumber)
                        .Distinct()
                        .ToList();

                    return new
                    {
                        item.Id,
                        item.InvoiceId,
                        item.ProductId,
                        ProductName = item.Product?.Name ?? "Unknown product",
                        ProductSku = item.Product?.SKU,
                        ProductImageUrl = item.Product?.ImageUrl,
                        item.VariantId,
                        item.Quantity,
                        item.Price,
                        item.TaxPercent,
                        item.TaxAmount,
                        item.Total,
                        ReturnedQuantity = returnedQuantity,
                        ReturnedAmount = matchingReturns.Sum(returnItem => (returnItem.Quantity ?? 0) * (returnItem.Price ?? 0)),
                        ReturnReferences = returnReferences
                    };
                })
                .Cast<object>()
                .ToList() ?? new List<object>();

            var returnedAmount = returnRows.Sum(item => item.TotalAmount ?? 0);

            return new
            {
                Id = invoice.InvoiceId,
                invoice.InvoiceId,
                invoice.SoId,
                invoice.CustomerId,
                CustomerName = invoice.Customer?.Name ?? "No customer",
                CustomerEmail = invoice.Customer?.Email,
                invoice.InvoiceNumber,
                invoice.InvoiceDate,
                invoice.DueDate,
                invoice.Status,
                invoice.TotalAmount,
                invoice.PaidAmount,
                invoice.BalanceAmount,
                PaymentMethod = _context.CustomerPayments
                    .Where(p => p.InvoiceId == invoice.InvoiceId && !p.IsCancelled)
                    .OrderBy(p => p.PaymentId)
                    .Select(p => p.PaymentMethod)
                    .FirstOrDefault() ?? "N/A",
                ReturnedAmount = returnedAmount,
                AdjustedOutstanding = Math.Max(0m, invoice.BalanceAmount),
                ReturnStatus = returnedAmount <= 0
                    ? null
                    : returnedAmount >= invoice.TotalAmount
                        ? "Fully Returned"
                        : "Partially Returned",
                ReturnReferences = returnRows
                    .Select(item => item.ReturnNumber)
                    .Distinct()
                    .ToList(),
                ItemCount = items.Count,
                Items = items
            };
        }

        private async Task<string> GenerateInvoiceNumber(CancellationToken cancellationToken)
        {
            var prefix = $"INV-{DateTime.UtcNow:yyyyMMdd}-";
            var lastInvoiceNumber = await _context.Invoices
                .AsNoTracking()
                .Where(invoice =>
                    invoice.InvoiceNumber != null &&
                    invoice.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(invoice => invoice.InvoiceNumber)
                .Select(invoice => invoice.InvoiceNumber)
                .FirstOrDefaultAsync(cancellationToken);

            var sequence = 1;

            if (!string.IsNullOrWhiteSpace(lastInvoiceNumber) &&
                int.TryParse(lastInvoiceNumber.Split('-').LastOrDefault(), out var lastSequence))
            {
                sequence = lastSequence + 1;
            }

            while (true)
            {
                var candidate = $"{prefix}{sequence:000}";
                var exists = await _context.Invoices
                    .AnyAsync(invoice => invoice.InvoiceNumber == candidate, cancellationToken);

                if (!exists)
                {
                    return candidate;
                }

                sequence++;
            }
        }

        private static string ResolveInvoiceStatus(decimal paidAmount, decimal balance, DateTime? dueDate)
        {
            if (balance <= 0)
            {
                return "Paid";
            }

            if (paidAmount > 0)
            {
                return "Partially Paid";
            }

            return dueDate.HasValue && dueDate.Value.Date < DateTime.UtcNow.Date ? "Overdue" : "Sent";
        }

        private static string GetDbUpdateUserMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);

            if (detail.Contains("movement_type", StringComparison.OrdinalIgnoreCase))
            {
                return $"Invoice could not be saved because stock movement type is invalid. {detail}";
            }

            if (detail.Contains("status", StringComparison.OrdinalIgnoreCase) &&
                detail.Contains("data truncated", StringComparison.OrdinalIgnoreCase))
            {
                return $"Invoice could not be saved because the invoice status value is not supported by the database schema. {detail}";
            }

            if (detail.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("constraint", StringComparison.OrdinalIgnoreCase))
            {
                return $"Invoice could not be saved because a selected record is invalid. {detail}";
            }

            if (detail.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("unique", StringComparison.OrdinalIgnoreCase))
            {
                return $"Invoice could not be saved because duplicate data exists. {detail}";
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

        public record InvoiceLineDraft(
    int ProductId,
    int? VariantId,
    decimal Quantity,
    decimal Price,
    decimal TaxPercent,
    decimal TaxAmount);

        private sealed record StockDemand(
            int ProductId,
            int? VariantId,
            decimal Quantity);
    }
}
