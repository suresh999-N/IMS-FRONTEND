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
    public class SupplierPaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<SupplierPaymentsController> _logger;

        public SupplierPaymentsController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<SupplierPaymentsController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // =========================================
        // GET: api/SupplierPayments
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetSupplierPayments()
        {
            try
            {
                var data = await (
                    from p in _context.SupplierPayments
                    join s in _context.Suppliers
                        on p.SupplierId equals s.SupplierId
                    where !p.IsCancelled
                    select new
                    {
                        p.PaymentId,
                        supplier = s.Name,
                        p.PoId,
                        p.Amount,
                        p.PaymentDate,
                        p.PaymentMethod,
                        p.ReferenceNumber,
                        Status = p.PoId == null ? "Unreconciled" : "Reconciled",
                        p.Notes
                    }
                ).ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier payment list failed. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to load payment data right now.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
        }

        // =========================================
        // POST: api/SupplierPayments
        // =========================================
        [HttpPost]
        public async Task<IActionResult> CreateSupplierPayment(SupplierPaymentDto dto, CancellationToken cancellationToken)
        {
            if (dto.Amount <= 0)
            {
                return BadRequest("Payment amount must be greater than zero.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var supplierExists = await _context.Suppliers
                    .AsNoTracking()
                    .AnyAsync(x => x.SupplierId == dto.SupplierId, cancellationToken);

                if (!supplierExists)
                    return NotFound("Supplier not found");

                if (!string.IsNullOrWhiteSpace(dto.ReferenceNumber))
                {
                    var normalizedReference = dto.ReferenceNumber.Trim().ToLower();
                    var referenceExists = await _context.SupplierPayments
                        .AsNoTracking()
                        .AnyAsync(x =>
                            !x.IsCancelled &&
                            x.ReferenceNumber != null &&
                            x.ReferenceNumber.ToLower() == normalizedReference,
                            cancellationToken);

                    if (referenceExists)
                        return BadRequest("Payment reference number already exists.");
                }

                PurchaseOrder? po = null;

                if (dto.PoId > 0)
                {
                    po = await _context.PurchaseOrders
                        .FirstOrDefaultAsync(x => x.PoId == dto.PoId, cancellationToken);

                    if (po == null)
                        return NotFound("Purchase order not found");

                    if (po.SupplierId != dto.SupplierId)
                        return BadRequest("Selected purchase order does not belong to the selected supplier.");

                    var paidAmount = await _context.SupplierPayments
                        .Where(x =>
                            x.PoId == dto.PoId &&
                            !x.IsCancelled)
                        .SumAsync(x => x.Amount ?? 0, cancellationToken);
                    var remainingBalance = (po.TotalAmount ?? 0) - paidAmount;

                    if (dto.Amount > remainingBalance)
                        return BadRequest($"Payment amount exceeds purchase order balance. Remaining balance: {remainingBalance}.");
                }

                var now = DateTime.UtcNow;
                var payment = new SupplierPayment
                {
                    SupplierId = dto.SupplierId,
                    PoId = dto.PoId > 0 ? dto.PoId : null,
                    Amount = dto.Amount,
                    PaymentDate = dto.PaymentDate,
                    PaymentMethod = dto.PaymentMethod,
                    ReferenceNumber = dto.ReferenceNumber,
                    Notes = dto.Notes
                };

                _context.SupplierPayments.Add(payment);
                await _context.SaveChangesAsync(cancellationToken);

                if (po != null)
                {
                    await ValidatePurchaseOrderPaymentStatus(po, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await _auditLogService.LogAsync(
                    "SUPPLIER_PAYMENT_CREATED",
                    "Payments",
                    payment.PaymentId,
                    $"Supplier payment {payment.PaymentId} created",
                    "supplier_payments",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(new
                {
                    message = "Supplier payment recorded successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Supplier payment creation failed. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to post payment right now.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
        }

        // =========================================
        // DELETE: api/SupplierPayments/5
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplierPayment(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
            => await VoidSupplierPayment(id, dto, cancellationToken);

        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidSupplierPayment(int id, [FromBody] CancelTransactionDto? dto, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var payment = await _context.SupplierPayments
                    .FirstOrDefaultAsync(x => x.PaymentId == id, cancellationToken);

                if (payment == null)
                    return NotFound();

                if (payment.IsCancelled)
                    return BadRequest("Supplier payment is already voided.");

                var now = DateTime.UtcNow;
                payment.IsCancelled = true;
                payment.CancelledAt = now;
                payment.CancellationReason = dto?.Reason;
                await _context.SaveChangesAsync(cancellationToken);

                if (payment.PoId.HasValue)
                {
                    var po = await _context.PurchaseOrders
                        .FirstOrDefaultAsync(x => x.PoId == payment.PoId.Value, cancellationToken);

                    if (po != null)
                    {
                        await ValidatePurchaseOrderPaymentStatus(po, cancellationToken);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(new
                {
                    message = "Supplier payment voided successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Supplier payment void failed. PaymentId={PaymentId}, TraceId: {TraceId}", id, HttpContext.TraceIdentifier);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to void payment right now.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
        }

        private async Task ValidatePurchaseOrderPaymentStatus(PurchaseOrder po, CancellationToken cancellationToken)
        {
            var paidAmount = await _context.SupplierPayments
                .Where(x =>
                    x.PoId == po.PoId &&
                    !x.IsCancelled)
                .SumAsync(x => x.Amount ?? 0, cancellationToken);
            var totalAmount = po.TotalAmount ?? 0;

            if (paidAmount > totalAmount)
            {
                throw new InvalidOperationException("Supplier payments exceed purchase order total.");
            }
        }
    }
}
