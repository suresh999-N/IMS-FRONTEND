using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IMSBackend.Data;
using IMSBackend.DTOs.PurchaseReturns;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        private string GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ??
                   User.FindFirst(ClaimTypes.Email)?.Value ??
                   User.FindFirst("username")?.Value ??
                   "System";
        }

        private static PurchaseReturnResponseDto MapToDto(PurchaseReturn r)
        {
            return new PurchaseReturnResponseDto
            {
                ReturnId = r.PurchaseReturnId,

                ReturnNumber = !string.IsNullOrWhiteSpace(r.ReturnNumber)
                    ? r.ReturnNumber
                    : $"PR-{r.PurchaseReturnId:D5}",

                SupplierId = r.SupplierId,

                SupplierName = r.Supplier?.Name ?? "—",

                SupplierCode = r.Supplier?.SupplierCode ?? string.Empty,

                GrnId = r.GrnId,

                GrnNumber =
                    r.GoodsReceipt != null &&
                    !string.IsNullOrEmpty(r.GoodsReceipt.GrnNumber)
                        ? r.GoodsReceipt.GrnNumber
                        : $"GRN-{r.GrnId:D6}",

                GrnDate = r.GoodsReceipt?.ReceiptDate,

                ReturnDate = r.ReturnDate,

                TotalAmount = r.TotalReturnAmount,

                Reason = r.Reason,

                // These columns no longer exist in purchase_returns.
                Notes = null,

                Status = string.IsNullOrWhiteSpace(r.Status)
                    ? "Draft"
                    : r.Status,

                SubmittedBy = null,
                SubmittedAt = null,

                ApprovedBy = null,
                ApprovedAt = null,

                RejectedBy = null,
                RejectedAt = null,

                RejectionReason = null,

                CompletedBy = null,
                CompletedAt = null,

                CreatedBy = null,

                CreatedAt = r.CreatedAt,

                UpdatedAt = r.UpdatedAt ?? r.CreatedAt,

                ItemCount = r.Items?.Count ?? 0,

                TotalQuantity = r.Items?.Sum(i => i.ReturnQuantity) ?? 0,

                Items = r.Items?.Select(i => new PurchaseReturnItemResponseDto
                {
                    Id = i.PurchaseReturnItemId,

                    ReturnId = i.PurchaseReturnId,

                    ProductId = i.ProductId,

                    ProductName = i.Product?.Name
                        ?? $"Product #{i.ProductId}",

                    Sku = i.Product?.SKU ?? "—",

                    VariantId = i.VariantId,

                    VariantName =
                        i.ProductVariant?.VariantName
                        ?? i.ProductVariant?.SKU,

                    Quantity = i.ReturnQuantity,

                    Price = i.Price

                }).ToList()
                ?? new List<PurchaseReturnItemResponseDto>()
            };
        }

        // =========================================================
        // GET: api/PurchaseReturns
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetPurchaseReturns(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.PurchaseReturns
                    .Include(r => r.Supplier)
                    .Include(r => r.GoodsReceipt)
                    .Include(r => r.Items)
                        .ThenInclude(i => i.Product)
                    .Include(r => r.Items)
                        .ThenInclude(i => i.ProductVariant)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(status) &&
                    status != "all" &&
                    status != "All")
                {
                    var st = status.Trim().ToLower();

                    query = query.Where(r =>
                        r.Status != null &&
                        r.Status.ToLower() == st);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim().ToLower();

                    query = query.Where(r =>
                        (
                            r.Supplier != null &&
                            r.Supplier.Name != null &&
                            r.Supplier.Name.ToLower().Contains(s)
                        )
                        ||
                        (
                            r.GoodsReceipt != null &&
                            (
                                (
                                    r.GoodsReceipt.GrnNumber != null &&
                                    r.GoodsReceipt.GrnNumber
                                        .ToLower()
                                        .Contains(s)
                                )
                                ||
                                r.GoodsReceipt.GrnId
                                    .ToString()
                                    .Contains(s)
                            )
                        )
                        ||
                        r.PurchaseReturnId
                            .ToString()
                            .Contains(s)
                        ||
                        (
                            r.ReturnNumber != null &&
                            r.ReturnNumber
                                .ToLower()
                                .Contains(s)
                        )
                        ||
                        (
                            r.Reason != null &&
                            r.Reason
                                .ToLower()
                                .Contains(s)
                        )
                        ||
                        (
                            r.Status != null &&
                            r.Status
                                .ToLower()
                                .Contains(s)
                        )
                    );
                }

                var list = await query
                    .OrderByDescending(r => r.ReturnDate)
                    .ThenByDescending(r => r.PurchaseReturnId)
                    .ToListAsync();

                var response = list
                    .Select(MapToDto)
                    .ToList();

                return Ok(new
                {
                    success = true,
                    data = response,
                    message = "Purchase returns retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching purchase returns");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error loading purchase returns."
                });
            }
        }

        // =========================================================
        // GET: api/PurchaseReturns/5
        // =========================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseReturn(int id)
        {
            try
            {
                var ret = await _context.PurchaseReturns
                    .Include(r => r.Supplier)
                    .Include(r => r.GoodsReceipt)
                    .Include(r => r.Items)
                        .ThenInclude(i => i.Product)
                    .Include(r => r.Items)
                        .ThenInclude(i => i.ProductVariant)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        r => r.PurchaseReturnId == id);

                if (ret == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message =
                            $"Purchase return PR-{id:D5} not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = MapToDto(ret),
                    message =
                        "Purchase return details retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching purchase return #{Id}",
                    id);

                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Error loading purchase return details."
                });
            }
        }

        // =========================================================
        // GET: api/PurchaseReturns/suppliers
        // =========================================================

        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var suppliers = await _context.Suppliers
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.Name)
                    .Select(s => new
                    {
                        s.SupplierId,

                        Id = s.SupplierId,

                        s.Name,

                        s.SupplierCode,

                        s.Email,

                        s.Phone
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = suppliers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching suppliers");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error loading suppliers list."
                });
            }
        }

        // =========================================================
        // GET: api/PurchaseReturns/grns
        // =========================================================

        [HttpGet("grns")]
        public async Task<IActionResult> GetGoodsReceipts(
            [FromQuery] int? supplierId = null)
        {
            try
            {
                var query =
                    _context.GoodsReceipts.AsNoTracking();

                if (supplierId.HasValue &&
                    supplierId.Value > 0)
                {
                    query = query.Where(
                        g => g.SupplierId ==
                             supplierId.Value);
                }

                var list = await query
                    .OrderByDescending(g => g.ReceiptDate)
                    .Select(g => new
                    {
                        GrnId = g.GrnId,

                        Id = g.GrnId,

                        GrnNumber =
                            !string.IsNullOrEmpty(g.GrnNumber)
                                ? g.GrnNumber
                                : $"GRN-{g.GrnId:D6}",

                        ReceiptDate = g.ReceiptDate,

                        SupplierId = g.SupplierId ?? 0
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = list
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching goods receipts");

                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Error loading goods receipts list."
                });
            }
        }

        // =========================================================
        // GET: api/PurchaseReturns/grn/{grnId}/items
        // =========================================================

        [HttpGet("grn/{grnId}/items")]
        public async Task<IActionResult> GetGrnItems(int grnId)
        {
            try
            {
                var grnItems =
                    await _context.GoodsReceiptItems
                        .Where(gi => gi.GrnId == grnId)
                        .AsNoTracking()
                        .ToListAsync();

                if (!grnItems.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        data = new List<object>()
                    });
                }

                var productIds = grnItems
                    .Select(gi => gi.ProductId ?? 0)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                var variantIds = grnItems
                    .Select(gi => gi.VariantId ?? 0)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                var products = await _context.Products
                    .Where(p =>
                        productIds.Contains(p.ProductId))
                    .ToDictionaryAsync(
                        p => p.ProductId);

                var variants =
                    await _context.ProductVariants
                        .Where(v =>
                            variantIds.Contains(v.VariantId))
                        .ToDictionaryAsync(
                            v => v.VariantId);

                var items = grnItems.Select(i =>
                {
                    decimal qty =
                        i.QuantityReceived ?? 0;

                    decimal rawPrice =
                        i.Price ?? 0;

                    decimal discount =
                        i.Discount ?? 0;

                    decimal tax =
                        i.Tax ??
                        i.TaxPercentage ??
                        0;

                    decimal lineTotal =
                        i.LineTotal ?? 0;

                    decimal unitCost = 0;

                    if (lineTotal > 0 &&
                        qty > 0)
                    {
                        unitCost = Math.Round(
                            lineTotal / qty,
                            2);
                    }
                    else if (rawPrice > 0)
                    {
                        decimal taxable =
                            rawPrice *
                            (1m - (discount / 100m));

                        unitCost = Math.Round(
                            taxable *
                            (1m + (tax / 100m)),
                            2);
                    }

                    return new
                    {
                        GrnItemId = i.Id,

                        ProductId =
                            i.ProductId ?? 0,

                        ProductName =
                            (
                                i.ProductId.HasValue &&
                                products.ContainsKey(
                                    i.ProductId.Value)
                            )
                                ? products[
                                    i.ProductId.Value
                                  ].Name
                                : $"Product #{i.ProductId}",

                        Sku =
                            (
                                i.ProductId.HasValue &&
                                products.ContainsKey(
                                    i.ProductId.Value)
                            )
                                ? products[
                                    i.ProductId.Value
                                  ].SKU
                                : "—",

                        VariantId = i.VariantId,

                        VariantName =
                            (
                                i.VariantId.HasValue &&
                                variants.ContainsKey(
                                    i.VariantId.Value)
                            )
                                ? (
                                    variants[
                                        i.VariantId.Value
                                    ].VariantName
                                    ??
                                    variants[
                                        i.VariantId.Value
                                    ].SKU
                                  )
                                : null,

                        ReceivedQuantity = qty,

                        RawUnitPrice = rawPrice,

                        Discount = discount,

                        Tax = tax,

                        TaxPercentage = tax,

                        LineTotal = lineTotal,

                        UnitCost = unitCost,

                        FinalPurchasePrice = unitCost,

                        Price = unitCost,

                        UnitPrice = unitCost
                    };
                }).ToList();

                return Ok(new
                {
                    success = true,
                    data = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching GRN items for GRN #{GrnId}",
                    grnId);

                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Error loading GRN items."
                });
            }
        }

        // =========================================================
        // POST: api/PurchaseReturns
        // Create Draft
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseReturn(
            [FromBody] CreatePurchaseReturnDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data provided.",
                    errors = ModelState
                });
            }

            if (dto.Items == null ||
                !dto.Items.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "At least one return item must be added."
                });
            }

            using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                // -------------------------------------------------
                // Get the selected GRN.
                // -------------------------------------------------

                var grn =
                    await _context.GoodsReceipts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            g => g.GrnId == dto.GrnId);

                if (grn == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Selected GRN was not found."
                    });
                }

                // -------------------------------------------------
                // Get items from the selected GRN.
                // Price and received quantity come from GRN.
                // -------------------------------------------------

                var grnItems =
                    await _context.GoodsReceiptItems
                        .Where(
                            gi => gi.GrnId == dto.GrnId)
                        .AsNoTracking()
                        .ToListAsync();

                if (!grnItems.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message =
                            "No items found for the selected GRN."
                    });
                }

                var returnItems =
                    new List<PurchaseReturnItem>();

                decimal totalReturnAmount = 0;

                foreach (var dtoItem in dto.Items)
                {
                    var grnItem =
                        grnItems.FirstOrDefault(gi =>
                            (gi.ProductId ?? 0)
                                == dtoItem.ProductId
                            &&
                            gi.VariantId
                                == dtoItem.VariantId);

                    if (grnItem == null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message =
                                $"Product #{dtoItem.ProductId} was not found in the selected GRN."
                        });
                    }

                    decimal receivedQuantity =
                        grnItem.QuantityReceived ?? 0;

                    decimal rawPrice =
                        grnItem.Price ?? 0;

                    decimal discount =
                        grnItem.Discount ?? 0;

                    decimal tax =
                        grnItem.Tax ??
                        grnItem.TaxPercentage ??
                        0;

                    decimal lineTotal =
                        grnItem.LineTotal ?? 0;

                    decimal unitCost = 0;

                    if (lineTotal > 0 &&
                        receivedQuantity > 0)
                    {
                        unitCost = Math.Round(
                            lineTotal /
                            receivedQuantity,
                            2);
                    }
                    else if (rawPrice > 0)
                    {
                        decimal taxable =
                            rawPrice *
                            (1m -
                             (discount / 100m));

                        unitCost = Math.Round(
                            taxable *
                            (1m +
                             (tax / 100m)),
                            2);
                    }

                    decimal itemTotal =
                        Math.Round(
                            dtoItem.ReturnQuantity *
                            unitCost,
                            2);

                    returnItems.Add(
                        new PurchaseReturnItem
                        {
                            ProductId =
                                dtoItem.ProductId,

                            VariantId =
                                dtoItem.VariantId,

                            ReceivedQuantity =
                                receivedQuantity,

                            ReturnQuantity =
                                dtoItem.ReturnQuantity,

                            Price =
                                unitCost,

                            Total =
                                itemTotal,

                            CreatedAt =
                                DateTime.Now
                        });

                    totalReturnAmount += itemTotal;
                }

                var currentUser =
                    GetCurrentUserName();

                var purchaseReturn =
                    new PurchaseReturn
                    {
                        SupplierId =
                            dto.SupplierId,

                        GrnId =
                            dto.GrnId,

                        ReturnDate =
                                 dto.ReturnDate,

                        Reason =
                            dto.Reason ?? string.Empty,

                        TotalReturnAmount =
                            totalReturnAmount,

                        Status =
                            "Draft",

                        CreatedAt =
                            DateTime.Now,

                        UpdatedAt =
                            DateTime.Now,

                        Items =
                            returnItems
                    };

                // -------------------------------------------------
                // Temporary unique return number.
                // After insert, it is replaced with PR-{ID}.
                // -------------------------------------------------

                purchaseReturn.ReturnNumber =
                    $"TEMP-{Guid.NewGuid():N}";

                _context.PurchaseReturns.Add(
                    purchaseReturn);

                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // Generate final return number using DB ID.
                // -------------------------------------------------

                purchaseReturn.ReturnNumber =
                    $"PR-{purchaseReturn.PurchaseReturnId:D5}";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Created Purchase Return {ReturnNumber} in Draft status by {User}",
                    purchaseReturn.ReturnNumber,
                    currentUser);

                return Ok(new
                {
                    success = true,

                    data = new
                    {
                        returnId =
                            purchaseReturn
                                .PurchaseReturnId,

                        returnNumber =
                            purchaseReturn
                                .ReturnNumber
                    },

                    message =
                        $"Purchase Return {purchaseReturn.ReturnNumber} created as Draft."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error creating purchase return");

                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Error creating purchase return: "
                        + ex.Message
                });
            }
        }

        // =========================================================
        // POST: api/PurchaseReturns/{id}/submit
        // =========================================================

        [HttpPost("{id}/submit")]
        public async Task<IActionResult>
            SubmitPurchaseReturn(int id)
        {
            var ret =
                await _context.PurchaseReturns
                    .FirstOrDefaultAsync(
                        r => r.PurchaseReturnId == id);

            if (ret == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Purchase return not found."
                });
            }

            if (ret.Status != "Draft" &&
                ret.Status != "Rejected")
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"Cannot submit purchase return in '{ret.Status}' status."
                });
            }

            var currentUser =
                GetCurrentUserName();

            ret.Status =
                "Pending Approval";

            ret.UpdatedAt =
                DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase Return {ReturnNumber} submitted for approval by {User}",
                ret.ReturnNumber,
                currentUser);

            return Ok(new
            {
                success = true,
                message =
                    $"Purchase Return {ret.ReturnNumber} submitted for approval."
            });
        }

        // =========================================================
        // POST: api/PurchaseReturns/{id}/approve
        // =========================================================

        [HttpPost("{id}/approve")]
        public async Task<IActionResult>
            ApprovePurchaseReturn(int id)
        {
            var ret =
                await _context.PurchaseReturns
                    .Include(r => r.Items)
                    .FirstOrDefaultAsync(
                        r => r.PurchaseReturnId == id);

            if (ret == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Purchase return not found."
                });
            }

            if (ret.Status != "Pending Approval" &&
                ret.Status != "Draft")
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"Cannot approve purchase return in '{ret.Status}' status."
                });
            }

            using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var currentUser =
                    GetCurrentUserName();

                ret.Status =
                    "Approved";

                ret.UpdatedAt =
                    DateTime.Now;

                // -------------------------------------------------
                // Existing business logic:
                // stock deduction happens on approval.
                // -------------------------------------------------

                var grn =
                    await _context.GoodsReceipts
                        .FirstOrDefaultAsync(
                            g => g.GrnId == ret.GrnId);

                int defaultWarehouseId =
                    grn?.WarehouseId ?? 1;

                foreach (var item in ret.Items)
                {
                    var stock =
                        await _context.Stocks
                            .FirstOrDefaultAsync(s =>
                                s.WarehouseId
                                    == defaultWarehouseId
                                &&
                                s.ProductId
                                    == item.ProductId
                                &&
                                s.VariantId
                                    == item.VariantId);

                    if (stock != null)
                    {
                        stock.Quantity =
                            Math.Max(
                                0,
                                stock.Quantity -
                                item.ReturnQuantity);
                    }

                    var stockMovement =
                        new StockMovement
                        {
                            ProductId =
                                item.ProductId,

                            VariantId =
                                item.VariantId,

                            WarehouseId =
                                defaultWarehouseId,

                            MovementType =
                                "return_out",

                            Quantity =
                                item.ReturnQuantity,

                            ReferenceId =
                                ret.PurchaseReturnId,

                            ReferenceType =
                                "PurchaseReturn",

                            Notes =
                                $"Approved Purchase Return {ret.ReturnNumber} to Supplier #{ret.SupplierId}",

                            CreatedAt =
                                DateTime.Now
                        };

                    _context.StockMovements.Add(
                        stockMovement);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Purchase Return {ReturnNumber} approved by {User}",
                    ret.ReturnNumber,
                    currentUser);

                return Ok(new
                {
                    success = true,
                    message =
                        $"Purchase Return {ret.ReturnNumber} approved and inventory updated."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error approving purchase return #{Id}",
                    id);

                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Error approving purchase return."
                });
            }
        }

        // =========================================================
        // POST: api/PurchaseReturns/{id}/reject
        // =========================================================

        [HttpPost("{id}/reject")]
        public async Task<IActionResult>
            RejectPurchaseReturn(
                int id,
                [FromBody] RejectPurchaseReturnDto dto)
        {
            var ret =
                await _context.PurchaseReturns
                    .FirstOrDefaultAsync(
                        r => r.PurchaseReturnId == id);

            if (ret == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Purchase return not found."
                });
            }

            if (ret.Status != "Pending Approval")
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"Cannot reject purchase return in '{ret.Status}' status."
                });
            }

            var currentUser =
                GetCurrentUserName();

            ret.Status =
                "Rejected";

            ret.UpdatedAt =
                DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase Return {ReturnNumber} rejected by {User}. Reason: {Reason}",
                ret.ReturnNumber,
                currentUser,
                dto?.Reason);

            return Ok(new
            {
                success = true,
                message =
                    $"Purchase Return {ret.ReturnNumber} rejected."
            });
        }

        // =========================================================
        // POST: api/PurchaseReturns/{id}/complete
        // =========================================================

        [HttpPost("{id}/complete")]
        public async Task<IActionResult>
            CompletePurchaseReturn(int id)
        {
            var ret =
                await _context.PurchaseReturns
                    .FirstOrDefaultAsync(
                        r => r.PurchaseReturnId == id);

            if (ret == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Purchase return not found."
                });
            }

            if (ret.Status != "Approved")
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        $"Cannot complete purchase return in '{ret.Status}' status."
                });
            }

            var currentUser =
                GetCurrentUserName();

            ret.Status =
                "Completed";

            ret.UpdatedAt =
                DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Purchase Return {ReturnNumber} completed by {User}",
                ret.ReturnNumber,
                currentUser);

            return Ok(new
            {
                success = true,
                message =
                    $"Purchase Return {ret.ReturnNumber} marked as Completed."
            });
        }

        // =========================================================
        // DELETE: api/PurchaseReturns/5
        // =========================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult>
            DeletePurchaseReturn(int id)
        {
            try
            {
                var ret =
                    await _context.PurchaseReturns
                        .Include(r => r.Items)
                        .FirstOrDefaultAsync(
                            r =>
                                r.PurchaseReturnId == id);

                if (ret == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message =
                            "Purchase return not found."
                    });
                }

                string returnNumber =
                    ret.ReturnNumber;

                _context.PurchaseReturnItems
                    .RemoveRange(ret.Items);

                _context.PurchaseReturns
                    .Remove(ret);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message =
                        $"Purchase return {returnNumber} deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting purchase return #{Id}",
                    id);

                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Error deleting purchase return."
                });
            }
        }
    }
}