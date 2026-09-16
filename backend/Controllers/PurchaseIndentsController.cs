using IMSBackend.Data;
using IMSBackend.DTOs.PurchaseIndents;
using IMSBackend.Models;
using IMSBackend.Services;
using IMSBackend.Attributes;
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
        [Permission("purchaseIndent", "add")]
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

                if (dto.IndentDate != default && dto.IndentDate.Date > DateTime.UtcNow.Date)
                {
                    return BadRequest(new
                    {
                        message = "Indent date cannot be beyond the current date."
                    });
                }

                // Stock availability validation: validate unavailable items have proper justification
                var unavailableProductNames = new List<string>();
                foreach (var item in dto.Items)
                {
                    var availableStock = await _context.Stocks
                        .Where(s => s.ProductId == item.ProductId)
                        .SumAsync(s => (decimal?)s.AvailableQuantity) ?? 0m;

                    if (availableStock <= 0m)
                    {
                        var prod = await _context.Products.FindAsync(item.ProductId);
                        unavailableProductNames.Add(prod?.Name ?? $"Product #{item.ProductId}");
                    }
                }

                if (unavailableProductNames.Any() && string.IsNullOrWhiteSpace(dto.Remarks))
                {
                    return BadRequest(new
                    {
                        message = $"Stock validation failed: {string.Join(", ", unavailableProductNames)} {(unavailableProductNames.Count == 1 ? "is" : "are")} unavailable (Available Stock: 0). Justification is required in Remarks before raising an indent for unavailable items."
                    });
                }





                // Generate Indent Number
                var today = DateTime.UtcNow.Date;
                var todayPrefix = $"IND-{today:yyyyMMdd}-";



                var lastIndentNumber = await _context.PurchaseIndents
                .Where(x => x.IndentNumber != null && x.IndentNumber.StartsWith(todayPrefix))
                .OrderByDescending(x => x.PurchaseIndentId)
                .Select(x => x.IndentNumber)
                .FirstOrDefaultAsync();



                int nextNumber = 1;



                if (!string.IsNullOrEmpty(lastIndentNumber))
                {
                    var numberPart = lastIndentNumber.Substring(todayPrefix.Length);



                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }



                var indentNumber = $"{todayPrefix}{nextNumber:D3}";



                var requiredDate = dto.RequiredDate <= dto.IndentDate
                    ? dto.IndentDate.AddDays(7)
                    : dto.RequiredDate;

                var purchaseIndent = new PurchaseIndent
                {
                    IndentNumber = indentNumber,
                    IndentDate = dto.IndentDate,
                    RequiredDate = requiredDate,
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

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        message = ex.InnerException?.Message ?? ex.Message
                    });
                }

                foreach (var item in dto.Items)
                {
                    var availableStock = await _context.Stocks
                    .Where(s => s.ProductId == item.ProductId)
                    .SumAsync(s => (decimal?)s.AvailableQuantity) ?? 0m;

                    var itemRequiredDate = item.RequiredDate > dto.IndentDate
                        ? item.RequiredDate
                        : requiredDate;

                    var indentItem = new PurchaseIndentItem
                    {
                        PurchaseIndentId = purchaseIndent.PurchaseIndentId,
                        ProductId = item.ProductId,
                        RequiredQty = item.RequiredQty,
                        UnitId = item.UnitId,
                        AvailableStock = availableStock,
                        RequiredDate = itemRequiredDate,
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
        [Permission("purchaseIndent", "view")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Purchase Indents list.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching Purchase Indents."
                });
            }
        }

        [HttpGet("{id}")]
        [Permission("purchaseIndent", "view")]
        public async Task<IActionResult> GetById(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Purchase Indent by ID: {Id}", id);
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching the Purchase Indent."
                });
            }
        }







        [HttpPut("{id}")]
        [Permission("purchaseIndent", "edit")]
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

                if (dto.IndentDate != default && dto.IndentDate.Date > DateTime.UtcNow.Date)
                {
                    return BadRequest(new
                    {
                        message = "Indent date cannot be beyond the current date."
                    });
                }

                // Stock availability validation: validate unavailable items have proper justification
                var unavailableProductNames = new List<string>();
                foreach (var item in dto.Items)
                {
                    var availableStock = await _context.Stocks
                        .Where(s => s.ProductId == item.ProductId)
                        .SumAsync(s => (decimal?)s.AvailableQuantity) ?? 0m;

                    if (availableStock <= 0m)
                    {
                        var prod = await _context.Products.FindAsync(item.ProductId);
                        unavailableProductNames.Add(prod?.Name ?? $"Product #{item.ProductId}");
                    }
                }

                if (unavailableProductNames.Any() && string.IsNullOrWhiteSpace(dto.Remarks))
                {
                    return BadRequest(new
                    {
                        message = $"Stock validation failed: {string.Join(", ", unavailableProductNames)} {(unavailableProductNames.Count == 1 ? "is" : "are")} unavailable (Available Stock: 0). Justification is required in Remarks before raising an indent for unavailable items."
                    });
                }



                var requiredDate = dto.RequiredDate <= dto.IndentDate
                    ? dto.IndentDate.AddDays(7)
                    : dto.RequiredDate;

                purchaseIndent.IndentDate = dto.IndentDate;
                purchaseIndent.RequiredDate = requiredDate;
                purchaseIndent.RequestedBy = dto.RequestedBy;
                purchaseIndent.DepartmentId = dto.DepartmentId;
                purchaseIndent.SupplierId = dto.SupplierId;
                purchaseIndent.Priority = dto.Priority;
                purchaseIndent.Remarks = dto.Remarks;
                purchaseIndent.TotalItems = dto.Items.Count;
                purchaseIndent.TotalQuantity = dto.Items.Sum(x => x.RequiredQty);
                purchaseIndent.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var oldItems = await _context.PurchaseIndentItems
                .Where(x => x.PurchaseIndentId == id)
                .ToListAsync();

                _context.PurchaseIndentItems.RemoveRange(oldItems);

                await _context.SaveChangesAsync();

                foreach (var item in dto.Items)
                {
                    var availableStock = await _context.Stocks
                    .Where(s => s.ProductId == item.ProductId)
                    .SumAsync(s => (decimal?)s.AvailableQuantity) ?? 0m;

                    var itemRequiredDate = item.RequiredDate > dto.IndentDate
                        ? item.RequiredDate
                        : requiredDate;

                    await _context.PurchaseIndentItems.AddAsync(new PurchaseIndentItem
                    {
                        PurchaseIndentId = id,
                        ProductId = item.ProductId,
                        RequiredQty = item.RequiredQty,
                        UnitId = item.UnitId,
                        AvailableStock = availableStock,
                        RequiredDate = itemRequiredDate,
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
        [Permission("purchaseIndent", "delete")]
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
        [Permission("purchaseIndent", "view")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var query = _context.PurchaseIndents
                .Where(x => !x.IsDeleted);

                var totalIndents = await query.CountAsync();

                var pending = await query
                .CountAsync(x => x.Status == "Pending");

                var approved = await query
                .CountAsync(x => x.Status == "Approved");

                var converted = await query
                .CountAsync(x => x.Status == "Converted");

                var rejected = await query
                .CountAsync(x => x.Status == "Rejected");

                var totalItemsRequested = await query
                .SumAsync(x => (decimal?)x.TotalQuantity) ?? 0;

                return Ok(new
                {
                    totalIndents,
                    pending,
                    approved,
                    converted,
                    rejected,
                    totalItemsRequested
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Purchase Indent dashboard.");
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching Purchase Indent dashboard statistics."
                });
            }
        }

        [HttpPut("{id}/approve")]
        [Permission("purchaseIndent", "edit")]
        public async Task<IActionResult> Approve(int id)
        {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving Purchase Indent ID: {Id}", id);
                return StatusCode(500, new
                {
                    message = "An error occurred while approving the Purchase Indent."
                });
            }
        }

        [HttpPut("{id}/reject")]
        [Permission("purchaseIndent", "edit")]
        public async Task<IActionResult> Reject(int id, RejectPurchaseIndentDto dto)
        {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting Purchase Indent ID: {Id}", id);
                return StatusCode(500, new
                {
                    message = "An error occurred while rejecting the Purchase Indent."
                });
            }
        }





        [HttpPost("{id}/convert-po")]
        [Permission("purchaseIndent", "edit")]
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







                var today = DateTime.UtcNow.Date;
                var todayPrefix = $"PO-{today:yyyyMMdd}-";



                var lastPoNumber = await _context.PurchaseOrders
                .Where(x => x.PoNumber != null && x.PoNumber.StartsWith(todayPrefix))
                .OrderByDescending(x => x.PoId)
                .Select(x => x.PoNumber)
                .FirstOrDefaultAsync();



                int nextNumber = 1;



                if (!string.IsNullOrEmpty(lastPoNumber))
                {
                    var numberPart = lastPoNumber.Substring(todayPrefix.Length);



                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }



                var poNumber = $"{todayPrefix}{nextNumber:D3}";



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



        private class IndentItemInfoInternal
        {
            public int PurchaseIndentId { get; set; }
            public int PurchaseIndentItemId { get; set; }
            public int ProductId { get; set; }
            public string? ProductName { get; set; }
            public string? ProductSku { get; set; }
            public int UnitId { get; set; }
            public string? UnitName { get; set; }
            public decimal RequiredQty { get; set; }
            public decimal AvailableStock { get; set; }
            public DateTime RequiredDate { get; set; }
            public string? Remarks { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal CostPrice { get; set; }
            public decimal Price { get; set; }
            public decimal Amount { get; set; }
        }

        private class SupplierInfoInternal
        {
            public int SupplierId { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }

        private class RelatedPoInternal
        {
            public int PurchaseOrderId { get; set; }
            public int PoId { get; set; }
            public string? PoNumber { get; set; }
            public string? Status { get; set; }
            public DateTime? OrderDate { get; set; }
            public string? Notes { get; set; }
        }

        private async Task<List<object>> BuildPurchaseIndentResponses(List<PurchaseIndent> indents)
        {
            if (indents == null || !indents.Any())
            {
                return new List<object>();
            }

            var indentIds = indents.Select(x => x.PurchaseIndentId).ToList();

            var allItems = await _context.PurchaseIndentItems
                .AsNoTracking()
                .Where(i => indentIds.Contains(i.PurchaseIndentId))
                .Include(i => i.Product)
                .Include(i => i.Unit)
                .Select(i => new IndentItemInfoInternal
                {
                    PurchaseIndentId = i.PurchaseIndentId,
                    PurchaseIndentItemId = i.PurchaseIndentItemId,
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : null,
                    ProductSku = i.Product != null ? i.Product.SKU : null,
                    UnitId = i.UnitId,
                    UnitName = i.Unit != null ? i.Unit.Name : null,
                    RequiredQty = i.RequiredQty,
                    AvailableStock = i.AvailableStock,
                    RequiredDate = i.RequiredDate,
                    Remarks = i.Remarks,
                    UnitPrice = i.Product != null ? (i.Product.CostPrice ?? i.Product.Price ?? 0m) : 0m,
                    CostPrice = i.Product != null ? (i.Product.CostPrice ?? 0m) : 0m,
                    Price = i.Product != null ? (i.Product.Price ?? 0m) : 0m,
                    Amount = i.RequiredQty * (i.Product != null ? (i.Product.CostPrice ?? i.Product.Price ?? 0m) : 0m)
                })
                .ToListAsync();

            var itemProductIds = allItems.Select(i => i.ProductId).Distinct().ToList();
            var productsDict = itemProductIds.Any()
                ? await _context.Products
                    .AsNoTracking()
                    .Where(p => itemProductIds.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId, p => p)
                : new Dictionary<int, Product>();

            var variantsDict = itemProductIds.Any()
                ? (await _context.ProductVariants
                    .AsNoTracking()
                    .Where(pv => itemProductIds.Contains(pv.ProductId))
                    .ToListAsync())
                    .GroupBy(pv => pv.ProductId)
                    .ToDictionary(g => g.Key, g => g.ToList())
                : new Dictionary<int, List<ProductVariant>>();

            foreach (var item in allItems)
            {
                if (item.UnitPrice <= 0m)
                {
                    decimal unitPrice = 0m;
                    decimal costPrice = 0m;
                    decimal price = 0m;

                    if (productsDict.TryGetValue(item.ProductId, out var prod))
                    {
                        costPrice = prod.CostPrice ?? 0m;
                        price = prod.Price ?? 0m;
                        unitPrice = costPrice > 0m ? costPrice : price;
                        if (string.IsNullOrWhiteSpace(item.ProductName)) item.ProductName = prod.Name;
                        if (string.IsNullOrWhiteSpace(item.ProductSku)) item.ProductSku = prod.SKU;
                    }

                    if (unitPrice <= 0m && variantsDict.TryGetValue(item.ProductId, out var variants))
                    {
                        var matchVariant = variants.FirstOrDefault(v => (v.CostPrice ?? 0m) > 0m || (v.Price ?? 0m) > 0m);
                        if (matchVariant != null)
                        {
                            costPrice = matchVariant.CostPrice ?? costPrice;
                            price = matchVariant.Price ?? price;
                            unitPrice = costPrice > 0m ? costPrice : price;
                        }
                    }

                    if (unitPrice > 0m)
                    {
                        item.UnitPrice = unitPrice;
                        item.CostPrice = costPrice > 0m ? costPrice : unitPrice;
                        item.Price = price > 0m ? price : unitPrice;
                        item.Amount = item.RequiredQty * unitPrice;
                    }
                }
                else if (item.Amount <= 0m)
                {
                    item.Amount = item.RequiredQty * item.UnitPrice;
                }
            }

            var itemsByIndentId = allItems
                .GroupBy(i => i.PurchaseIndentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 2. Bulk query AuditLogs
            var allAuditLogs = await _context.AuditLogs
                .AsNoTracking()
                .Where(log => log.RecordId.HasValue && indentIds.Contains(log.RecordId.Value) &&
                             (log.TableName == "purchase_indents" || log.Module == "PurchaseIndents"))
                .OrderBy(log => log.CreatedAt)
                .ToListAsync();

            var auditLogsByIndentId = allAuditLogs
                .GroupBy(log => log.RecordId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3. Bulk query Users
            var userIds = allAuditLogs
                .Select(log => log.UserId)
                .Concat(indents.Select(x => (int?)x.RequestedBy))
                .Concat(indents.Select(x => x.ApprovedBy))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var userNames = userIds.Any()
                ? (await _context.Users
                    .AsNoTracking()
                    .Where(user => userIds.Contains(user.Id))
                    .Select(u => new { u.Id, u.Name, u.Email })
                    .ToListAsync())
                    .GroupBy(u => u.Id)
                    .ToDictionary(
                        g => g.Key,
                        g => string.IsNullOrWhiteSpace(g.First().Name) ? g.First().Email : g.First().Name)
                : new Dictionary<int, string>();

            // 4. Bulk query Suppliers
            var supplierIds = indents
                .Where(x => x.SupplierId.HasValue)
                .Select(x => x.SupplierId!.Value)
                .Distinct()
                .ToList();

            var supplierDict = supplierIds.Any()
                ? (await _context.Suppliers
                    .AsNoTracking()
                    .Where(s => supplierIds.Contains(s.SupplierId))
                    .Select(s => new SupplierInfoInternal
                    {
                        SupplierId = s.SupplierId,
                        Name = s.Name,
                        Email = s.Email,
                        Phone = s.Phone
                    })
                    .ToListAsync())
                    .GroupBy(s => s.SupplierId)
                    .ToDictionary(g => g.Key, g => g.First())
                : new Dictionary<int, SupplierInfoInternal>();

            // 5. Bulk query Related Purchase Orders
            var indentNumbers = indents
                .Where(x => !string.IsNullOrWhiteSpace(x.IndentNumber))
                .Select(x => x.IndentNumber!)
                .Distinct()
                .ToList();

            var candidateOrders = indentNumbers.Any()
                ? await _context.PurchaseOrders
                    .AsNoTracking()
                    .Where(order => !order.IsCancelled && order.Notes != null && order.Notes != "")
                    .OrderByDescending(order => order.PoId)
                    .Select(order => new RelatedPoInternal
                    {
                        PurchaseOrderId = order.PoId,
                        PoId = order.PoId,
                        PoNumber = order.PoNumber,
                        Status = order.Status,
                        OrderDate = order.OrderDate,
                        Notes = order.Notes
                    })
                    .ToListAsync()
                : new List<RelatedPoInternal>();

            var purchaseOrders = candidateOrders
                .Where(order => order.Notes != null && indentNumbers.Any(num => order.Notes.Contains(num)))
                .ToList();

            string? GetUserName(int? id)
            {
                return id.HasValue && userNames.TryGetValue(id.Value, out var name) ? name : null;
            }

            string? GetLogUserName(AuditLog? log)
            {
                return log?.UserId != null ? GetUserName(log.UserId) : null;
            }

            var result = new List<object>();

            foreach (var indent in indents)
            {
                var items = itemsByIndentId.TryGetValue(indent.PurchaseIndentId, out var indentItems)
                    ? indentItems
                    : new List<IndentItemInfoInternal>();

                var auditLogs = auditLogsByIndentId.TryGetValue(indent.PurchaseIndentId, out var logs)
                    ? logs
                    : new List<AuditLog>();

                var supplier = indent.SupplierId.HasValue && supplierDict.TryGetValue(indent.SupplierId.Value, out var s)
                    ? s
                    : null;

                var relatedPo = !string.IsNullOrWhiteSpace(indent.IndentNumber)
                    ? purchaseOrders.FirstOrDefault(po => po.Notes != null && po.Notes.Contains(indent.IndentNumber))
                    : null;

                var createLog = FindAudit(auditLogs, "CREATE_PURCHASE_INDENT");
                var updateLog = FindAudit(auditLogs, "UPDATE_PURCHASE_INDENT");
                var approveLog = FindAudit(auditLogs, "APPROVE_PURCHASE_INDENT");
                var rejectLog = FindAudit(auditLogs, "REJECT_PURCHASE_INDENT");
                var convertLog = FindAudit(auditLogs, "CONVERT_PURCHASE_INDENT");

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
                        actorName = GetLogUserName(log),
                        description = log.Description,
                        createdAt = log.CreatedAt
                    })
                    .ToList();

                var relatedPurchaseOrderObj = relatedPo != null ? new
                {
                    purchaseOrderId = relatedPo.PurchaseOrderId,
                    poId = relatedPo.PoId,
                    poNumber = relatedPo.PoNumber,
                    status = relatedPo.Status,
                    orderDate = relatedPo.OrderDate
                } : null;

                decimal estimatedValue = items.Sum(x => x.Amount);

                result.Add(new
                {
                    purchaseIndentId = indent.PurchaseIndentId,
                    indentId = indent.PurchaseIndentId,
                    indentNumber = indent.IndentNumber,
                    indentDate = indent.IndentDate,
                    requiredDate = indent.RequiredDate,
                    requestedBy = indent.RequestedBy,
                    requestedById = indent.RequestedBy,
                    requestedByName = GetUserName(indent.RequestedBy),
                    departmentId = indent.DepartmentId,
                    departmentName = DepartmentNames.TryGetValue(indent.DepartmentId, out var departmentName) ? departmentName : null,
                    supplierId = indent.SupplierId,
                    supplierName = supplier?.Name,
                    supplierEmail = supplier?.Email,
                    supplierPhone = supplier?.Phone,
                    approvedBy = approvedBy,
                    approvedByName = GetUserName(approvedBy),
                    priority = indent.Priority,
                    status = indent.Status,
                    remarks = indent.Remarks,
                    totalItems = indent.TotalItems,
                    totalQuantity = indent.TotalQuantity,
                    estimatedValue = estimatedValue,
                    totalAmount = estimatedValue,
                    createdBy = createdBy,
                    createdByName = GetUserName(createdBy),
                    updatedBy = updatedBy,
                    updatedByName = GetUserName(updatedBy),
                    rejectedBy = rejectedBy,
                    rejectedByName = GetUserName(rejectedBy),
                    convertedBy = convertedBy,
                    convertedByName = GetUserName(convertedBy),
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
                    purchaseOrderId = relatedPurchaseOrderObj?.purchaseOrderId,
                    relatedPurchaseOrderId = relatedPurchaseOrderObj?.purchaseOrderId,
                    purchaseOrderNumber = relatedPurchaseOrderObj?.poNumber,
                    poNumber = relatedPurchaseOrderObj?.poNumber,
                    relatedPurchaseOrder = relatedPurchaseOrderObj,
                    items = items.Select(i => new
                    {
                        purchaseIndentItemId = i.PurchaseIndentItemId,
                        productId = i.ProductId,
                        productName = i.ProductName,
                        productSku = i.ProductSku,
                        unitId = i.UnitId,
                        unitName = i.UnitName,
                        requiredQty = i.RequiredQty,
                        quantity = i.RequiredQty,
                        availableStock = i.AvailableStock,
                        requiredDate = i.RequiredDate,
                        remarks = i.Remarks,
                        unitPrice = i.UnitPrice,
                        rate = i.UnitPrice,
                        costPrice = i.CostPrice,
                        price = i.Price,
                        amount = i.Amount
                    }).ToList(),
                    attachments = Array.Empty<object>(),
                    activityHistory
                });
            }

            return result;
        }

        private async Task<object> BuildPurchaseIndentResponse(PurchaseIndent indent)
        {
            var list = await BuildPurchaseIndentResponses(new List<PurchaseIndent> { indent });
            return list.FirstOrDefault() ?? new object();
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
