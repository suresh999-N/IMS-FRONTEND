using IMSBackend.Data;
using IMSBackend.DTOs.PurchaseIndents;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseIndentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<PurchaseIndentsController> _logger;
        private static readonly IReadOnlyDictionary<int, string> DepartmentNames = new Dictionary<int, string>
        {
            [1] = "Production",
            [2] = "Inventory",
            [3] = "Sales",
            [4] = "Purchase",
            [5] = "Finance",
            [6] = "Admin"
        };

        public PurchaseIndentsController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<PurchaseIndentsController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreatePurchaseIndentDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (dto.Items == null || !dto.Items.Any())
                {
                    return BadRequest(new
                    {
                        message = "At least one item is required."
                    });
                }

                // Generate Indent Number
                var today = DateTime.Today;

                var count = await _context.PurchaseIndents
                    .CountAsync(x => x.CreatedAt!.Value.Date == today);

                var indentNumber = $"IND-{today:yyyyMMdd}-{(count + 1):D3}";

                var purchaseIndent = new PurchaseIndent
                {
                    IndentNumber = indentNumber,
                    IndentDate = dto.IndentDate,
                    RequiredDate = dto.RequiredDate,
                    RequestedBy = dto.RequestedBy,
                    DepartmentId = dto.DepartmentId,
                    SupplierId = dto.SupplierId,
                    ApprovedBy = null,
                    Priority = dto.Priority ?? "Medium",
                    Status = "Pending",
                    Remarks = dto.Remarks,
                    TotalItems = dto.Items.Count,
                    TotalQuantity = dto.Items.Sum(x => x.RequiredQty),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    IsDeleted = false
                };

                _context.PurchaseIndents.Add(purchaseIndent);

                await _context.SaveChangesAsync();

                foreach (var item in dto.Items)
                {
                    var indentItem = new PurchaseIndentItem
                    {
                        PurchaseIndentId = purchaseIndent.PurchaseIndentId,
                        ProductId = item.ProductId,
                        RequiredQty = item.RequiredQty,
                        UnitId = item.UnitId,
                        AvailableStock = item.AvailableStock,
                        RequiredDate = item.RequiredDate,
                        Remarks = item.Remarks
                    };

                    _context.PurchaseIndentItems.Add(indentItem);
                }

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "CREATE_PURCHASE_INDENT",
                    "PurchaseIndents",
                    purchaseIndent.PurchaseIndentId,
                    $"Purchase Indent Created : {purchaseIndent.IndentNumber}",
                    "purchase_indents");

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Purchase Indent created successfully.",
                    purchaseIndentId = purchaseIndent.PurchaseIndentId,
                    indentNumber = purchaseIndent.IndentNumber
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Purchase Indent creation failed.");

                return StatusCode(500, new
                {
                    message = "Purchase Indent creation failed."
                });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.PurchaseIndents
                .Where(x => !x.IsDeleted);

            var totalRecords = await query.CountAsync();

            var indents = await _context.PurchaseIndents
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.PurchaseIndentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var data = await BuildPurchaseIndentResponses(indents);

            return Ok(new
            {
                success = true,
                data,
                totalRecords,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var purchaseIndent = await _context.PurchaseIndents
                .AsNoTracking()
                .Where(x => x.PurchaseIndentId == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (purchaseIndent == null)
            {
                return NotFound(new
                {
                    message = "Purchase Indent not found."
                });
            }

            var data = await BuildPurchaseIndentResponse(purchaseIndent);

            return Ok(data);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePurchaseIndentDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var purchaseIndent = await _context.PurchaseIndents
                    .FirstOrDefaultAsync(x => x.PurchaseIndentId == id && !x.IsDeleted);

                if (purchaseIndent == null)
                {
                    return NotFound(new
                    {
                        message = "Purchase Indent not found."
                    });
                }


                if (purchaseIndent.Status == "Converted")
                {
                    return BadRequest(new
                    {
                        message = "Converted Purchase Indent cannot be updated."
                    });
                }


                if (dto.Items == null || !dto.Items.Any())
                {
                    return BadRequest(new
                    {
                        message = "At least one item is required."
                    });
                }

                purchaseIndent.IndentDate = dto.IndentDate;
                purchaseIndent.RequiredDate = dto.RequiredDate;
                purchaseIndent.RequestedBy = dto.RequestedBy;
                purchaseIndent.DepartmentId = dto.DepartmentId;
                purchaseIndent.SupplierId = dto.SupplierId;
                purchaseIndent.Priority = dto.Priority;
                purchaseIndent.Remarks = dto.Remarks;
                purchaseIndent.TotalItems = dto.Items.Count;
                purchaseIndent.TotalQuantity = dto.Items.Sum(x => x.RequiredQty);
                purchaseIndent.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var oldItems = _context.PurchaseIndentItems
                    .Where(x => x.PurchaseIndentId == id);

                _context.PurchaseIndentItems.RemoveRange(oldItems);

                await _context.SaveChangesAsync();

                foreach (var item in dto.Items)
                {
                    await _context.PurchaseIndentItems.AddAsync(new PurchaseIndentItem
                    {
                        PurchaseIndentId = id,
                        ProductId = item.ProductId,
                        RequiredQty = item.RequiredQty,
                        UnitId = item.UnitId,
                        AvailableStock = item.AvailableStock,
                        RequiredDate = item.RequiredDate,
                        Remarks = item.Remarks
                    });
                }

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "UPDATE_PURCHASE_INDENT",
                    "PurchaseIndents",
                    purchaseIndent.PurchaseIndentId,
                    $"Purchase Indent Updated : {purchaseIndent.IndentNumber}",
                    "purchase_indents");

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Purchase Indent updated successfully."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Purchase Indent update failed.");

                return StatusCode(500, new
                {
                    message = "Purchase Indent update failed."
                });
            }
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var purchaseIndent = await _context.PurchaseIndents
                .FirstOrDefaultAsync(x => x.PurchaseIndentId == id && !x.IsDeleted);

            if (purchaseIndent == null)
            {
                return NotFound(new
                {
                    message = "Purchase Indent not found."
                });
            }


            if (purchaseIndent.Status == "Converted")
            {
                return BadRequest(new
                {
                    message = "Converted Purchase Indent cannot be deleted."
                });
            }

            purchaseIndent.IsDeleted = true;
            purchaseIndent.DeletedAt = DateTime.UtcNow;
            purchaseIndent.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "DELETE_PURCHASE_INDENT",
                    "PurchaseIndents",
                    purchaseIndent.PurchaseIndentId,
                    $"Purchase Indent Deleted : {purchaseIndent.IndentNumber}",
                    "purchase_indents");

                return Ok(new
                {
                    message = "Purchase Indent deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Purchase Indent delete failed.");

                return StatusCode(500, new
                {
                    message = "Purchase Indent delete failed."
                });
            }
        }



        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var query = _context.PurchaseIndents
                .Where(x => !x.IsDeleted);

            var totalIndents = await query.CountAsync();

            var pending = await query
                .CountAsync(x => x.Status == "Pending");

            var approved = await query
                .CountAsync(x => x.Status == "Approved");

            var totalItemsRequested = await query
                .SumAsync(x => (decimal?)x.TotalQuantity) ?? 0;

            return Ok(new
            {
                totalIndents,
                pending,
                approved,
                totalItemsRequested
            });
        }




        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var purchaseIndent = await _context.PurchaseIndents
                .FirstOrDefaultAsync(x => x.PurchaseIndentId == id && !x.IsDeleted);

            if (purchaseIndent == null)
            {
                return NotFound(new
                {
                    message = "Purchase Indent not found."
                });
            }


            if (purchaseIndent.Status == "Rejected")
            {
                return BadRequest(new
                {
                    message = "Rejected Purchase Indent cannot be approved."
                });
            }


            if (purchaseIndent.Status == "Approved")
            {
                return BadRequest(new
                {
                    message = "Purchase Indent is already approved."
                });
            }

            purchaseIndent.Status = "Approved";
            purchaseIndent.ApprovedBy = GetCurrentUserId() ?? purchaseIndent.ApprovedBy;
            purchaseIndent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "APPROVE_PURCHASE_INDENT",
                "PurchaseIndents",
                purchaseIndent.PurchaseIndentId,
                $"Purchase Indent Approved : {purchaseIndent.IndentNumber}",
                "purchase_indents");

            return Ok(new
            {
                message = "Purchase Indent approved successfully."
            });
        }



        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id, RejectPurchaseIndentDto dto)
        {
            var purchaseIndent = await _context.PurchaseIndents
                .FirstOrDefaultAsync(x => x.PurchaseIndentId == id && !x.IsDeleted);

            if (purchaseIndent == null)
            {
                return NotFound(new
                {
                    message = "Purchase Indent not found."
                });
            }


            if (purchaseIndent.Status == "Approved")
            {
                return BadRequest(new
                {
                    message = "Approved Purchase Indent cannot be rejected."
                });
            }

            if (purchaseIndent.Status == "Converted")
            {
                return BadRequest(new
                {
                    message = "Converted Purchase Indent cannot be rejected."
                });
            }

            purchaseIndent.Status = "Rejected";
            purchaseIndent.Remarks = dto.Reason;
            purchaseIndent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "REJECT_PURCHASE_INDENT",
                "PurchaseIndents",
                purchaseIndent.PurchaseIndentId,
                $"Purchase Indent Rejected : {purchaseIndent.IndentNumber}",
                "purchase_indents");

            return Ok(new
            {
                message = "Purchase Indent rejected successfully."
            });
        }



        [HttpPost("{id}/convert-po")]
        public async Task<IActionResult> ConvertToPurchaseOrder(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var purchaseIndent = await _context.PurchaseIndents
                    .FirstOrDefaultAsync(x => x.PurchaseIndentId == id && !x.IsDeleted);

                if (purchaseIndent == null)
                {
                    return NotFound(new
                    {
                        message = "Purchase Indent not found."
                    });
                }

                if (purchaseIndent.Status == "Converted")
                {
                    return BadRequest(new
                    {
                        message = "Purchase Indent has already been converted to a Purchase Order."
                    });
                }


                if (purchaseIndent.Status != "Approved")
                {
                    return BadRequest(new
                    {
                        message = "Only approved Purchase Indents can be converted."
                    });
                }

                var items = await _context.PurchaseIndentItems
                .Include(x => x.Product)
                .Where(x => x.PurchaseIndentId == id)
                .ToListAsync();


                var totalAmount = items.Sum(item =>

                 item.RequiredQty * (item.Product?.CostPrice ?? 0)

                 );



                var conversionErrors = await ValidateConversionAsync(purchaseIndent, items);

                if (conversionErrors.Any())
                {
                    return BadRequest(new
                    {
                        message = $"Cannot convert until {string.Join(", ", conversionErrors)} {(conversionErrors.Count == 1 ? "is" : "are")} available."
                    });
                }

                var today = DateTime.Today;

                var count = await _context.PurchaseOrders
                    .CountAsync(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Date == today);

                var poNumber = $"PO-{today:yyyyMMdd}-{(count + 1):D3}";
                var sourceNote = $"Source Purchase Indent: {purchaseIndent.IndentNumber} (ID {purchaseIndent.PurchaseIndentId})";
                var notes = string.IsNullOrWhiteSpace(purchaseIndent.Remarks)
                    ? sourceNote
                    : $"{purchaseIndent.Remarks.Trim()}\n\n{sourceNote}";

                var po = new PurchaseOrder
                {
                    SupplierId = purchaseIndent.SupplierId,
                    PoNumber = poNumber,
                    OrderDate = DateTime.UtcNow,
                    ExpectedDate = purchaseIndent.RequiredDate,
                    Status = "Pending",
                    ReceivingStatus = "Pending",
                    TotalAmount = totalAmount,
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow,
                    IsCancelled = false
                };

                _context.PurchaseOrders.Add(po);

                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    var price = item.Product?.CostPrice ?? 0;
                    var itemTotal = item.RequiredQty * price;

                    _context.PurchaseOrderItems.Add(new PurchaseOrderItem
                    {
                        PoId = po.PoId,
                        ProductId = item.ProductId,
                        Quantity = item.RequiredQty,
                        ReceivedQuantity = 0,
                        Price = price,
                        Total = itemTotal
                    });
                }

                await _context.SaveChangesAsync();

                purchaseIndent.Status = "Converted";
                purchaseIndent.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "CONVERT_PURCHASE_ORDER",
                    "PurchaseOrders",
                    po.PoId,
                    $"Purchase Order created from {purchaseIndent.IndentNumber}",
                    "purchase_orders");

                await _auditLogService.LogAsync(
                    "CONVERT_PURCHASE_INDENT",
                    "PurchaseIndents",
                    purchaseIndent.PurchaseIndentId,
                    $"Purchase Indent {purchaseIndent.IndentNumber} converted to Purchase Order {po.PoNumber}",
                    "purchase_indents");

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Purchase Order created successfully.",
                    purchaseOrderId = po.PoId,
                    poNumber = po.PoNumber,
                    sourceIndentId = purchaseIndent.PurchaseIndentId,
                    sourceIndentNumber = purchaseIndent.IndentNumber
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Purchase Order conversion failed.");

                return StatusCode(500, new
                {
                    message = "Purchase Order conversion failed."
                });
            }
        }

        private async Task<List<object>> BuildPurchaseIndentResponses(List<PurchaseIndent> indents)
        {
            var result = new List<object>();

            foreach (var indent in indents)
            {
                result.Add(await BuildPurchaseIndentResponse(indent));
            }

            return result;
        }

        private async Task<object> BuildPurchaseIndentResponse(PurchaseIndent indent)
        {
            var items = await _context.PurchaseIndentItems
                .AsNoTracking()
                .Where(i => i.PurchaseIndentId == indent.PurchaseIndentId)
                .Include(i => i.Product)
                .Include(i => i.Unit)
                .Select(i => new
                {
                    purchaseIndentItemId = i.PurchaseIndentItemId,
                    productId = i.ProductId,
                    productName = i.Product.Name,
                    productSku = i.Product.SKU,
                    unitId = i.UnitId,
                    unitName = i.Unit.Name,
                    requiredQty = i.RequiredQty,
                    quantity = i.RequiredQty,
                    availableStock = i.AvailableStock,
                    requiredDate = i.RequiredDate,
                    remarks = i.Remarks
                })
                .ToListAsync();

            var auditLogs = await _context.AuditLogs
                .AsNoTracking()
                .Where(log =>
                    log.RecordId == indent.PurchaseIndentId &&
                    (log.TableName == "purchase_indents" || log.Module == "PurchaseIndents"))
                .OrderBy(log => log.CreatedAt)
                .ToListAsync();

            var userIds = auditLogs
                .Select(log => log.UserId)
                .Append(indent.RequestedBy)
                .Append(indent.ApprovedBy)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var userNames = await _context.Users
                .AsNoTracking()
                .Where(user => userIds.Contains(user.Id))
                .ToDictionaryAsync(
                    user => user.Id,
                    user => string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name);

            var supplier = indent.SupplierId.HasValue
                ? await _context.Suppliers
                    .AsNoTracking()
                    .Where(item => item.SupplierId == indent.SupplierId.Value)
                    .Select(item => new { item.SupplierId, item.Name, item.Email, item.Phone })
                    .FirstOrDefaultAsync()
                : null;

            var relatedPurchaseOrder = await _context.PurchaseOrders
                .AsNoTracking()
                .Where(order =>
                    !order.IsCancelled &&
                    order.Notes != null &&
                    indent.IndentNumber != null &&
                    order.Notes.Contains(indent.IndentNumber))
                .OrderByDescending(order => order.PoId)
                .Select(order => new
                {
                    purchaseOrderId = order.PoId,
                    poId = order.PoId,
                    poNumber = order.PoNumber,
                    status = order.Status,
                    orderDate = order.OrderDate
                })
                .FirstOrDefaultAsync();

            var createLog = FindAudit(auditLogs, "CREATE_PURCHASE_INDENT");
            var updateLog = FindAudit(auditLogs, "UPDATE_PURCHASE_INDENT");
            var approveLog = FindAudit(auditLogs, "APPROVE_PURCHASE_INDENT");
            var rejectLog = FindAudit(auditLogs, "REJECT_PURCHASE_INDENT");
            var convertLog = FindAudit(auditLogs, "CONVERT_PURCHASE_INDENT");

            string? UserName(int? id)
            {
                return id.HasValue && userNames.TryGetValue(id.Value, out var name)
                    ? name
                    : null;
            }

            string? LogUserName(AuditLog? log)
            {
                return log?.UserId != null ? UserName(log.UserId) : null;
            }

            var createdBy = createLog?.UserId ?? indent.RequestedBy;
            var updatedBy = updateLog?.UserId;
            var approvedBy = approveLog?.UserId ?? indent.ApprovedBy;
            var rejectedBy = rejectLog?.UserId;
            var convertedBy = convertLog?.UserId;

            var activityHistory = auditLogs
                .Select(log => new
                {
                    action = log.Action,
                    actorId = log.UserId,
                    actorName = LogUserName(log),
                    description = log.Description,
                    createdAt = log.CreatedAt
                })
                .ToList();

            return new
            {
                purchaseIndentId = indent.PurchaseIndentId,
                indentId = indent.PurchaseIndentId,
                indentNumber = indent.IndentNumber,
                indentDate = indent.IndentDate,
                requiredDate = indent.RequiredDate,
                requestedBy = indent.RequestedBy,
                requestedById = indent.RequestedBy,
                requestedByName = UserName(indent.RequestedBy),
                departmentId = indent.DepartmentId,
                departmentName = DepartmentNames.TryGetValue(indent.DepartmentId, out var departmentName) ? departmentName : null,
                supplierId = indent.SupplierId,
                supplierName = supplier?.Name,
                supplierEmail = supplier?.Email,
                supplierPhone = supplier?.Phone,
                approvedBy = approvedBy,
                approvedByName = UserName(approvedBy),
                priority = indent.Priority,
                status = indent.Status,
                remarks = indent.Remarks,
                totalItems = indent.TotalItems,
                totalQuantity = indent.TotalQuantity,
                createdBy = createdBy,
                createdByName = UserName(createdBy),
                updatedBy = updatedBy,
                updatedByName = UserName(updatedBy),
                rejectedBy = rejectedBy,
                rejectedByName = UserName(rejectedBy),
                convertedBy = convertedBy,
                convertedByName = UserName(convertedBy),
                createdDate = createLog?.CreatedAt ?? indent.CreatedAt,
                createdAt = createLog?.CreatedAt ?? indent.CreatedAt,
                updatedDate = updateLog?.CreatedAt ?? indent.UpdatedAt,
                updatedAt = updateLog?.CreatedAt ?? indent.UpdatedAt,
                approvedDate = approveLog?.CreatedAt,
                approvedAt = approveLog?.CreatedAt,
                rejectedDate = rejectLog?.CreatedAt,
                rejectedAt = rejectLog?.CreatedAt,
                convertedDate = convertLog?.CreatedAt,
                convertedAt = convertLog?.CreatedAt,
                purchaseOrderId = relatedPurchaseOrder?.purchaseOrderId,
                relatedPurchaseOrderId = relatedPurchaseOrder?.purchaseOrderId,
                purchaseOrderNumber = relatedPurchaseOrder?.poNumber,
                poNumber = relatedPurchaseOrder?.poNumber,
                relatedPurchaseOrder,
                items,
                attachments = Array.Empty<object>(),
                activityHistory
            };
        }

        private static AuditLog? FindAudit(IEnumerable<AuditLog> logs, string action)
        {
            return logs
                .Where(log => string.Equals(log.Action, action, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(log => log.CreatedAt)
                .FirstOrDefault();
        }

        private async Task<List<string>> ValidateConversionAsync(PurchaseIndent purchaseIndent, List<PurchaseIndentItem> items)
        {
            var errors = new List<string>();

            if (!purchaseIndent.SupplierId.HasValue || purchaseIndent.SupplierId.Value <= 0)
            {
                errors.Add("supplier");
            }
            else
            {
                var supplierExists = await _context.Suppliers
                    .AsNoTracking()
                    .AnyAsync(item => item.SupplierId == purchaseIndent.SupplierId.Value);

                if (!supplierExists)
                {
                    errors.Add("valid supplier");
                }
            }

            if (purchaseIndent.DepartmentId <= 0)
            {
                errors.Add("department");
            }

            if (!items.Any())
            {
                errors.Add("line items");
                return errors;
            }

            var invalidLines = items
                .Select((item, index) => new { item, index })
                .Where(entry => entry.item.ProductId <= 0 || entry.item.RequiredQty <= 0)
                .Select(entry => $"valid product and quantity on line {entry.index + 1}")
                .ToList();

            errors.AddRange(invalidLines);

            return errors.Distinct().ToList();
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


    }
}
