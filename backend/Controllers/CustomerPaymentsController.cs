using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<CustomerPaymentsController> _logger;

        public CustomerPaymentsController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<CustomerPaymentsController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // =========================================
        // GET ALL PAYMENTS
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            try
            {
                var rows = await (
                    from p in _context.CustomerPayments
                    join c in _context.Customers
                        on p.CustomerId equals c.CustomerId
                    join i in _context.Invoices
                        on p.InvoiceId equals i.InvoiceId into invoiceGroup
                    from invoice in invoiceGroup.DefaultIfEmpty()
                    select new
                    {
                        paymentId = p.PaymentId,
                        invoiceId = p.InvoiceId,
                        invoiceNumber = invoice == null ? null : invoice.InvoiceNumber,
                        invoiceStatus = invoice == null ? null : invoice.Status,
                        invoiceAmount = invoice == null ? 0 : invoice.TotalAmount,
                        customer = c.Name,
                        amount = p.Amount,
                        paymentDate = p.PaymentDate,
                        paymentMethod = p.PaymentMethod,
                        referenceNumber = p.ReferenceNumber,
                        isCancelled = p.IsCancelled,
                        notes = p.Notes,
                        cancelledAt = p.CancelledAt,
                        cancellationReason = p.CancellationReason,
                    }
                ).ToListAsync();

                var data = rows.Select(p => new
                {
                    p.paymentId,
                    paymentNumber = BuildPaymentNumber(p.paymentDate, p.paymentId),
                    p.invoiceId,
                    p.invoiceNumber,
                    p.invoiceStatus,
                    p.invoiceAmount,
                    p.customer,
                    p.amount,
                    outstandingBefore = p.amount,
                    outstandingAfter = 0,
                    p.paymentDate,
                    p.paymentMethod,
                    p.referenceNumber,
                    status = p.isCancelled ? "Cancelled" : "Completed",
                    p.notes,
                    createdBy = "System",
                    createdAt = p.paymentDate,
                    p.cancelledAt,
                    p.cancellationReason
                }).ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer payment list failed. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to load payment data right now.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
        }

        // =========================================
        // CREATE PAYMENT
        // =========================================
        [HttpPost]
        public async Task<IActionResult> AddPayment(CustomerPaymentDto dto, CancellationToken cancellationToken)
        {
            if (dto.Amount <= 0)
            {
                return BadRequest("Payment amount must be greater than zero.");
            }

            var validationError = ValidateEditableFields(dto.ReferenceNumber, dto.Notes, dto.Status);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTime.UtcNow;
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(x => x.CustomerId == dto.CustomerId, cancellationToken);

                if (customer == null)
                    return NotFound("Customer not found");

                if (!string.IsNullOrWhiteSpace(dto.ReferenceNumber))
                {
                    var normalizedReference = dto.ReferenceNumber.Trim().ToLower();
                    var referenceExists = await _context.CustomerPayments
                        .AsNoTracking()
                        .AnyAsync(x =>
                            !x.IsCancelled &&
                            x.ReferenceNumber != null &&
                            x.ReferenceNumber.ToLower() == normalizedReference,
                            cancellationToken);

                    if (referenceExists)
                        return BadRequest("Payment reference number already exists.");
                }

                var outstandingBefore = customer.OutstandingBalance;

                if (dto.Amount > outstandingBefore)
                    return BadRequest($"Payment amount exceeds customer outstanding balance. Outstanding balance: {customer.OutstandingBalance}.");

                Invoice? invoice = null;

                if (dto.InvoiceId.HasValue)
                {
                    invoice = await _context.Invoices
                        .FirstOrDefaultAsync(x => x.InvoiceId == dto.InvoiceId.Value, cancellationToken);

                    if (invoice == null)
                        return NotFound("Invoice not found");

                    if (invoice.CustomerId != dto.CustomerId)
                        return BadRequest("Selected invoice does not belong to the selected customer.");

                    var remainingBalance = invoice.BalanceAmount;

                    if (dto.Amount > remainingBalance)
                        return BadRequest($"Payment amount exceeds remaining invoice balance. Remaining balance: {remainingBalance}.");

                    invoice.PaidAmount += dto.Amount;
                    invoice.BalanceAmount = invoice.TotalAmount - invoice.PaidAmount;

                    if (invoice.BalanceAmount < 0)
                        invoice.BalanceAmount = 0;

                    invoice.Status = ResolveInvoiceStatus(invoice.PaidAmount, invoice.BalanceAmount, invoice.DueDate);
                }

                // ================================
                // SAVE PAYMENT
                // ================================
                var payment = new CustomerPayment
                {
                    CustomerId = dto.CustomerId,
                    InvoiceId = dto.InvoiceId,
                    Amount = dto.Amount,
                    PaymentDate = dto.PaymentDate,
                    PaymentMethod = dto.PaymentMethod,
                    ReferenceNumber = dto.ReferenceNumber?.Trim(),
                    Notes = dto.Notes?.Trim(),
                };

                _context.CustomerPayments.Add(payment);

                // ================================
                // UPDATE OUTSTANDING BALANCE
                // ================================
                customer.OutstandingBalance -= dto.Amount;

                if (customer.OutstandingBalance < 0)
                    customer.OutstandingBalance = 0;

                customer.UpdatedAt = now;

                await _context.SaveChangesAsync(cancellationToken);

                // ================================
                // ADD LEDGER ENTRY
                // ================================
                var ledger = new CustomerLedger
                {
                    CustomerId = customer.CustomerId,
                    TransactionType = "payment",
                    TransactionId = payment.PaymentId,
                    Debit = 0,
                    Credit = dto.Amount,
                    Balance = customer.OutstandingBalance,
                    CreatedAt = now
                };

                _context.CustomerLedgers.Add(ledger);

                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "CUSTOMER_PAYMENT_CREATED",
                    "Payments",
                    payment.PaymentId,
                    $"Customer payment {payment.PaymentNumber} created",
                    "customer_payments",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(new
                {
                    message = "Payment added successfully",
                    data = new
                    {
                        paymentId = payment.PaymentId,
                        paymentNumber = payment.PaymentNumber
                    }
                });
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Customer payment creation failed. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to post payment right now.",
                    traceId = HttpContext.TraceIdentifier
                });
            } 
        }

        // =========================================
        // UPDATE PAYMENT
        // =========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, UpdateCustomerPaymentDto dto, CancellationToken cancellationToken)
        {
            if (dto.Amount <= 0)
            {
                return BadRequest("Payment amount must be greater than zero.");
            }

            var validationError = ValidateEditableFields(dto.ReferenceNumber, dto.Notes, dto.Status);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (string.Equals(dto.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return await VoidPayment(id, new CancelTransactionDto { Reason = "Cancelled from payment edit." }, cancellationToken);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var payment = await _context.CustomerPayments
                    .FirstOrDefaultAsync(x => x.PaymentId == id, cancellationToken);

                if (payment == null)
                    return NotFound("Payment not found.");

                if (payment.IsCancelled)
                    return BadRequest("Cancelled payments cannot be edited.");

                if (!string.IsNullOrWhiteSpace(dto.ReferenceNumber))
                {
                    var normalizedReference = dto.ReferenceNumber.Trim().ToLower();
                    var referenceExists = await _context.CustomerPayments
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.PaymentId != id &&
                            !x.IsCancelled &&
                            x.ReferenceNumber != null &&
                            x.ReferenceNumber.ToLower() == normalizedReference,
                            cancellationToken);

                    if (referenceExists)
                        return BadRequest("Payment reference number already exists.");
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(x => x.CustomerId == payment.CustomerId, cancellationToken);

                if (customer == null)
                    return NotFound("Customer not found.");

                var originalAmount = payment.Amount;
                var delta = dto.Amount - originalAmount;

                if (delta > customer.OutstandingBalance)
                    return BadRequest($"Payment amount exceeds customer outstanding balance. Outstanding balance: {customer.OutstandingBalance + originalAmount}.");

                if (payment.InvoiceId.HasValue)
                {
                    var invoice = await _context.Invoices
                        .FirstOrDefaultAsync(x => x.InvoiceId == payment.InvoiceId.Value, cancellationToken);

                    if (invoice != null)
                    {
                        var availableInvoiceBalance = invoice.BalanceAmount + originalAmount;

                        if (dto.Amount > availableInvoiceBalance)
                            return BadRequest($"Payment amount exceeds remaining invoice balance. Remaining balance: {availableInvoiceBalance}.");

                        invoice.PaidAmount = Math.Max(0m, invoice.PaidAmount + delta);
                        invoice.BalanceAmount = Math.Max(0m, invoice.TotalAmount - invoice.PaidAmount);
                        invoice.Status = ResolveInvoiceStatus(invoice.PaidAmount, invoice.BalanceAmount, invoice.DueDate);
                    }
                }

                customer.OutstandingBalance = Math.Max(0m, customer.OutstandingBalance - delta);
                customer.UpdatedAt = DateTime.UtcNow;

                payment.Amount = dto.Amount;
                payment.PaymentMethod = dto.PaymentMethod;
                payment.ReferenceNumber = dto.ReferenceNumber?.Trim();
                payment.Notes = dto.Notes?.Trim();

                _context.CustomerLedgers.Add(new CustomerLedger
                {
                    CustomerId = customer.CustomerId,
                    TransactionType = "payment_edit",
                    TransactionId = payment.PaymentId,
                    Debit = delta < 0 ? Math.Abs(delta) : 0,
                    Credit = delta > 0 ? delta : 0,
                    Balance = customer.OutstandingBalance,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "CUSTOMER_PAYMENT_UPDATED",
                    "Payments",
                    payment.PaymentId,
                    $"Customer payment {payment.PaymentNumber} updated",
                    "customer_payments",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(new { message = "Payment updated successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Customer payment update failed. PaymentId={PaymentId}, TraceId: {TraceId}", id, HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to update payment right now.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
        }

        // =========================================
        // VOID PAYMENT
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
            => await VoidPayment(id, dto, cancellationToken);

        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidPayment(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var payment = await _context.CustomerPayments
                    .FirstOrDefaultAsync(x => x.PaymentId == id, cancellationToken);

                if (payment == null)
                    return NotFound();

                if (payment.IsCancelled)
                    return BadRequest("Payment is already voided.");

                var now = DateTime.UtcNow;
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(x => x.CustomerId == payment.CustomerId, cancellationToken);

                if (customer != null)
                {
                    customer.OutstandingBalance += payment.Amount;
                    customer.UpdatedAt = now;
                }

                if (payment.InvoiceId.HasValue)
                {
                    var invoice = await _context.Invoices
                        .FirstOrDefaultAsync(x => x.InvoiceId == payment.InvoiceId.Value, cancellationToken);

                    if (invoice != null)
                    {
                        invoice.PaidAmount -= payment.Amount;

                        if (invoice.PaidAmount < 0)
                            invoice.PaidAmount = 0;

                        invoice.BalanceAmount = invoice.TotalAmount - invoice.PaidAmount;
                        invoice.Status = ResolveInvoiceStatus(invoice.PaidAmount, invoice.BalanceAmount, invoice.DueDate);
                    }
                }

                payment.IsCancelled = true;
                payment.CancelledAt = now;
                payment.CancellationReason = dto?.Reason;

                if (customer != null)
                {
                    _context.CustomerLedgers.Add(new CustomerLedger
                    {
                        CustomerId = customer.CustomerId,
                        TransactionType = "payment_void",
                        TransactionId = payment.PaymentId,
                        Debit = payment.Amount,
                        Credit = 0,
                        Balance = customer.OutstandingBalance,
                        CreatedAt = now
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(new
                {
                    message = "Payment voided successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Customer payment void failed. PaymentId={PaymentId}, TraceId: {TraceId}", id, HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to void payment right now.",
                    traceId = HttpContext.TraceIdentifier
                });
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

        private string ResolveCreatedBy(string? dtoCreatedBy)
        {
            return dtoCreatedBy?.Trim()
                ?? User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? "System";
        }

        private static string BuildPaymentNumber(DateTime paymentDate, int paymentId)
        {
            return $"PAY-{paymentDate:yyyyMMdd}-{paymentId:000}";
        }

        private static string NormalizePaymentStatus(string? status, string? referenceNumber)
        {
            var normalized = (status ?? "").Trim();

            if (string.Equals(normalized, "Cancelled", StringComparison.OrdinalIgnoreCase))
                return "Cancelled";

            if (
                string.Equals(normalized, "Completed", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Reconciled", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Success", StringComparison.OrdinalIgnoreCase))
                return "Completed";

            if (string.Equals(normalized, "Pending", StringComparison.OrdinalIgnoreCase))
                return "Pending";

            return "Completed";
        }

        private static string? ValidateEditableFields(string? referenceNumber, string? notes, string? status)
        {
            if ((referenceNumber?.Length ?? 0) > 100)
                return "Reference number must be 100 characters or fewer.";

            if ((notes?.Length ?? 0) > 500)
                return "Notes must be 500 characters or fewer.";

            if (!string.IsNullOrWhiteSpace(status))
            {
                var allowedStatuses = new[] { "Pending", "Completed", "Reconciled", "Success", "Cancelled" };

                if (!allowedStatuses.Any(item => string.Equals(item, status.Trim(), StringComparison.OrdinalIgnoreCase)))
                    return "Payment status must be Pending, Completed, Reconciled, Success, or Cancelled.";
            }

            return null;
        }
    }
}
