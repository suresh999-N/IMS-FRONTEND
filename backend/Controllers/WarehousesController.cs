using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WarehousesController> _logger;

        public WarehousesController(AppDbContext context, ILogger<WarehousesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var warehouses = await _context.Warehouses
                .AsNoTracking()
                .Where(w => !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(w => w.WarehouseId)
                .ToListAsync(cancellationToken);

            var stocks = await _context.Stocks
                .AsNoTracking()
                .Where(s => s.Quantity > 0)
                .Join(
                    _context.Products,
                    s => s.ProductId,
                    p => p.ProductId,
                    (s, p) => new
                    {
                        WarehouseId = s.WarehouseId,
                        ProductId = p.ProductId,
                        ProductName = p.Name,
                        SKU = p.SKU,
                        Quantity = s.Quantity
                    })
                .ToListAsync(cancellationToken);

            var result = warehouses.Select(w =>
            {
                // Only stocks belonging to THIS warehouse
                var warehouseStocks = stocks
                    .Where(s => s.WarehouseId == w.WarehouseId)
                    .GroupBy(s => new
                    {
                        s.ProductId,
                        s.ProductName,
                        s.SKU
                    })
                    .Select(g => new
                    {
                        productId = g.Key.ProductId,
                        productName = g.Key.ProductName,
                        sku = g.Key.SKU,
                        quantity = g.Sum(x => x.Quantity)
                    })
                    .Where(x => x.quantity > 0)
                    .ToList();

                return new
                {
                    id = w.WarehouseId,
                    name = w.Name,
                    warehouseCode = w.WarehouseCode,
                    location = w.Location,
                    managerName = w.ManagerName,
                    phone = w.Phone,
                    email = w.Email,
                    status = w.Status,
                    createdAt = w.CreatedAt,
                    updatedAt = w.UpdatedAt,

                    // IMPORTANT:
                    // Products belong ONLY to this warehouse
                    products = warehouseStocks,

                    stockUnits = warehouseStocks.Sum(x => x.quantity),

                    rackCount = _context.Racks
                        .AsNoTracking()
                        .Count(r => r.WarehouseId == w.WarehouseId),

                    binCount = _context.Bins
                        .AsNoTracking()
                        .Count(b => b.WarehouseId == w.WarehouseId)
                };
            }).ToList();

            return Ok(result);
        }

        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var warehouse = _context.Warehouses
    .FirstOrDefault(w =>
        w.WarehouseId == id &&
        !w.IsDeleted);
            if (warehouse == null) return NotFound();

            return Ok(warehouse);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public IActionResult Create(CreateWarehouseDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Name = request.Name.Trim();
            request.WarehouseCode = request.WarehouseCode.Trim().ToUpper();
            request.Location = request.Location.Trim();
            request.ManagerName = request.ManagerName.Trim();
            request.Phone = request.Phone.Trim();
            request.Email = request.Email.Trim().ToLower();

            if (_context.Warehouses.Any(w =>
                !w.IsDeleted &&
                w.Name.ToLower() == request.Name.ToLower()))
            {
                return Conflict(new
                {
                    message = "Warehouse name already exists."
                });
            }

            if (_context.Warehouses.Any(w =>
                !w.IsDeleted &&
                w.WarehouseCode.ToUpper() == request.WarehouseCode))
            {
                return Conflict(new
                {
                    message = "Warehouse code already exists."
                });
            }

            if (_context.Warehouses.Any(w =>
                !w.IsDeleted &&
                w.Phone == request.Phone))
            {
                return Conflict(new
                {
                    message = "Phone number already exists."
                });
            }

            if (_context.Warehouses.Any(w =>
                !w.IsDeleted &&
                w.Email.ToLower() == request.Email))
            {
                return Conflict(new
                {
                    message = "Email already exists."
                });
            }

            var warehouse = new Warehouse
            {
                Name = request.Name,
                WarehouseCode = request.WarehouseCode,
                Location = request.Location,
                ManagerName = request.ManagerName,
                Phone = request.Phone,
                Email = request.Email,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();

            return Ok(warehouse);
        }

        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateWarehouseDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var warehouse = _context.Warehouses
                .FirstOrDefault(w =>
                    w.WarehouseId == id &&
                    !w.IsDeleted);

            if (warehouse == null)
            {
                return NotFound(new
                {
                    message = "Warehouse not found."
                });
            }

            request.Name = request.Name.Trim();
            request.WarehouseCode = request.WarehouseCode.Trim().ToUpper();
            request.Location = request.Location.Trim();
            request.ManagerName = request.ManagerName.Trim();
            request.Phone = request.Phone.Trim();
            request.Email = request.Email.Trim().ToLower();

            if (_context.Warehouses.Any(w =>
                w.WarehouseId != id &&
                !w.IsDeleted &&
                w.Name.ToLower() == request.Name.ToLower()))
            {
                return Conflict(new
                {
                    message = "Warehouse name already exists."
                });
            }

            if (_context.Warehouses.Any(w =>
                w.WarehouseId != id &&
                !w.IsDeleted &&
                w.WarehouseCode == request.WarehouseCode))
            {
                return Conflict(new
                {
                    message = "Warehouse code already exists."
                });
            }

            if (_context.Warehouses.Any(w =>
                w.WarehouseId != id &&
                !w.IsDeleted &&
                w.Phone == request.Phone))
            {
                return Conflict(new
                {
                    message = "Phone number already exists."
                });
            }

            if (_context.Warehouses.Any(w =>
                w.WarehouseId != id &&
                !w.IsDeleted &&
                w.Email.ToLower() == request.Email))
            {
                return Conflict(new
                {
                    message = "Email already exists."
                });
            }

            warehouse.Name = request.Name;
            warehouse.WarehouseCode = request.WarehouseCode;
            warehouse.Location = request.Location;
            warehouse.ManagerName = request.ManagerName;
            warehouse.Phone = request.Phone;
            warehouse.Email = request.Email;
            warehouse.Status = request.Status;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok(warehouse);
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var warehouse = _context.Warehouses
                .FirstOrDefault(w =>
                    w.WarehouseId == id &&
                    !w.IsDeleted);

            if (warehouse == null)
            {
                return NotFound(new
                {
                    message = "Warehouse not found."
                });
            }

            var hasRacks = _context.Racks.Any(r => r.WarehouseId == id);

            if (hasRacks)
            {
                return Conflict(new
                {
                    message = "Warehouse cannot be deleted because racks exist."
                });
            }

            var hasBins = _context.Bins.Any(b => b.WarehouseId == id);

            if (hasBins)
            {
                return Conflict(new
                {
                    message = "Warehouse cannot be deleted because bins exist."
                });
            }

            var hasStock = _context.Stocks.Any(s => s.WarehouseId == id);

            if (hasStock)
            {
                return Conflict(new
                {
                    message = "Warehouse cannot be deleted because stock exists."
                });
            }

            warehouse.IsDeleted = true;
            warehouse.DeletedAt = DateTime.UtcNow;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok(new
            {
                message = "Warehouse deleted successfully."
            });
        }



        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            var summary = new WarehouseSummaryDto
            {
                Warehouses = _context.Warehouses.Count(w => !w.IsDeleted),

                StockUnits = _context.Stocks
                    .Sum(s => (decimal?)s.Quantity) ?? 0,

                Racks = _context.Racks.Count(),

                Bins = _context.Bins.Count()
            };

            return Ok(summary);
        }



        [HttpGet("{id}/details")]
        public IActionResult GetWarehouseDetails(int id)
        {
            // 1. Find warehouse
            var warehouse = _context.Warehouses
                .FirstOrDefault(w =>
                    w.WarehouseId == id &&
                    !w.IsDeleted);

            if (warehouse == null)
                return NotFound("Warehouse not found");

            _logger.LogInformation(
                "Warehouse details requested. WarehouseId={WarehouseId}, Name={WarehouseName}",
                warehouse.WarehouseId,
                warehouse.Name);

            // 2. Count racks belonging to this warehouse
            var totalRacks = _context.Racks
                .Count(r => r.WarehouseId == id);

            // 3. Count bins belonging to this warehouse
            var totalBins = _context.Bins
                .Count(b => b.WarehouseId == id);

            // 4. Get warehouse stock from STOCK table
            //    This represents total stock in the warehouse,
            //    whether it has been put away into a bin or not.
            var stockProducts = (
                from s in _context.Stocks
                join p in _context.Products
                    on s.ProductId equals p.ProductId
                where s.WarehouseId == id

                      && s.Quantity > 0
                select new
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    VariantId = s.VariantId,
                    Quantity = s.Quantity
                }
            ).ToList();

            // 5. Number of distinct products currently in warehouse stock
            var totalProducts = stockProducts
                .Select(x => x.ProductId)
                .Distinct()
                .Count();

            // 6. Total warehouse stock
            var totalStockUnits = stockProducts
                .Sum(x => x.Quantity);

            // 7. Get bin/rack information for products that have been put away
            var binStockProducts = (
                from bs in _context.BinStocks
                join p in _context.Products
                    on bs.ProductId equals p.ProductId
                join b in _context.Bins
                    on bs.BinId equals b.BinId
                join r in _context.Racks
                    on b.RackId equals r.RackId
                where bs.WarehouseId == id
                      && bs.Quantity > 0
                select new
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    RackCode = r.RackCode,
                    BinCode = b.BinCode,
                    Quantity = bs.Quantity
                }
            ).ToList();

            // 8. Build product response
            //    Products come from STOCK.
            //    Rack/Bin information comes from BIN STOCK.
            var products = stockProducts
                .Select(stock =>
                {
                    var binStock = binStockProducts
                        .FirstOrDefault(bs => bs.ProductId == stock.ProductId);

                    return new WarehouseProductDto
                    {
                        ProductName = stock.ProductName,

                        RackCode = binStock?.RackCode ?? "",

                        BinCode = binStock?.BinCode ?? "",

                        Quantity = stock.Quantity
                    };
                })
                .ToList();

            // 9. Build final response
            var result = new WarehouseDetailsDto
            {
                WarehouseId = warehouse.WarehouseId,
                WarehouseName = warehouse.Name,

                TotalRacks = totalRacks,
                TotalBins = totalBins,

                TotalProducts = totalProducts,

                // IMPORTANT:
                // This now comes from STOCK, not BIN STOCK.
                TotalStockUnits = totalStockUnits,

                Products = products
            };

            return Ok(result);
        }

        [HttpGet("{id:int}/products")]
        public async Task<IActionResult> GetWarehouseProducts(
            int id,
            CancellationToken cancellationToken)
        {
            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    w => w.WarehouseId == id && !w.IsDeleted,
                    cancellationToken);

            if (warehouse == null)
            {
                return NotFound(new
                {
                    message = "Warehouse not found."
                });
            }

            var products = await _context.Stocks
                .AsNoTracking()
                .Where(s =>
                    s.WarehouseId == id &&
                    s.Quantity > 0)
                .Join(
                    _context.Products,
                    stock => stock.ProductId,
                    product => product.ProductId,
                    (stock, product) => new
                    {
                        productId = product.ProductId,
                        name = product.Name,
                        sku = product.SKU,
                        quantity = stock.Quantity,
                        availableQuantity = stock.AvailableQuantity
                    })
                .ToListAsync(cancellationToken);

            return Ok(new
            {
                warehouseId = warehouse.WarehouseId,
                warehouseName = warehouse.Name,
                products = products
            });
        }

        // =====================================================
        // CREATE WAREHOUSE STOCK FROM APPROVED GRN
        // GRN Approval -> Warehouse Stock Creation
        // =====================================================
        [HttpPost("stock/from-grn/{grnId:int}")]
        public async Task<IActionResult> CreateStockFromApprovedGrn(
            int grnId,
            CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                // 1. Get GRN
                var grn = await _context.GoodsReceipts
                    .FirstOrDefaultAsync(
                        x => x.GrnId == grnId,
                        cancellationToken);

                if (grn == null)
                {
                    return NotFound(new
                    {
                        message = "GRN not found."
                    });
                }

                // 2. GRN must be approved
                if (!string.Equals(
                        grn.Status,
                        "approved",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        message = "GRN must be approved before warehouse stock can be created.",
                        currentStatus = grn.Status
                    });
                }

                // 3. Warehouse is required
                if (!grn.WarehouseId.HasValue || grn.WarehouseId.Value <= 0)
                {
                    return BadRequest(new
                    {
                        message = "GRN does not have a valid warehouse."
                    });
                }

                var warehouseId = grn.WarehouseId.Value;

                // 4. Verify warehouse
                var warehouseExists = await _context.Warehouses
                    .AnyAsync(
                        x => x.WarehouseId == warehouseId &&
                             !x.IsDeleted,
                        cancellationToken);

                if (!warehouseExists)
                {
                    return BadRequest(new
                    {
                        message = "Warehouse does not exist."
                    });
                }

                // 5. Get GRN items
                var grnItems = await _context.GoodsReceiptItems
                    .Where(x => x.GrnId == grnId)
                    .ToListAsync(cancellationToken);

                if (grnItems.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "GRN does not contain any items."
                    });
                }

                var createdStocks = new List<object>();

                foreach (var item in grnItems)
                {
                    if (!item.ProductId.HasValue ||
                        item.ProductId.Value <= 0)
                    {
                        return BadRequest(new
                        {
                            message = $"GRN item {item.Id} has an invalid ProductId."
                        });
                    }

                    var productId = item.ProductId.Value;
                    var variantId = item.VariantId;
                    var quantity = item.QuantityReceived ?? 0;

                    if (quantity <= 0)
                    {
                        return BadRequest(new
                        {
                            message = $"GRN item {item.Id} has invalid received quantity."
                        });
                    }

                    // =====================================================
                    // IMPORTANT:
                    // Validate product + variant combination.
                    // This prevents Product 1 from accidentally becoming
                    // VariantId = NULL stock.
                    // =====================================================

                    if (variantId.HasValue)
                    {
                        var variantExists = await _context.ProductVariants
                            .AnyAsync(
                                x => x.VariantId == variantId.Value &&
                                     x.ProductId == productId,
                                cancellationToken);

                        if (!variantExists)
                        {
                            return BadRequest(new
                            {
                                message =
                                    $"VariantId {variantId.Value} does not belong to ProductId {productId}."
                            });
                        }
                    }

                    // =====================================================
                    // Find existing stock for EXACT:
                    // Product + Variant + Warehouse
                    // =====================================================

                    var stock = await _context.Stocks
                        .FirstOrDefaultAsync(
                            x =>
                                x.ProductId == productId &&
                                x.VariantId == variantId &&
                                x.WarehouseId == warehouseId,
                            cancellationToken);

                    decimal openingQuantity;

                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            ProductId = productId,
                            VariantId = variantId,
                            WarehouseId = warehouseId,
                            Quantity = quantity,
                            ReservedQuantity = 0
                        };

                        _context.Stocks.Add(stock);

                        openingQuantity = 0;
                    }
                    else
                    {
                        openingQuantity = stock.Quantity;

                        stock.Quantity += quantity;
                    }

                    var closingQuantity = stock.Quantity;

                    // =====================================================
                    // STOCK LEDGER
                    // =====================================================

                    _context.StockLedgers.Add(new StockLedger
                    {
                        ProductId = productId,
                        VariantId = variantId,
                        WarehouseId = warehouseId,

                        OpeningQty = openingQuantity,
                        ChangeQty = quantity,
                        ClosingQty = closingQuantity,

                        TransactionType = "PURCHASE",
                        TransactionId = grnId,
                        CreatedAt = DateTime.UtcNow
                    });

                    // =====================================================
                    // STOCK MOVEMENT
                    // =====================================================

                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = productId,
                        VariantId = variantId,
                        WarehouseId = warehouseId,

                        MovementType = "PURCHASE",
                        Quantity = quantity,

                        ReferenceId = grnId,
                        ReferenceType = "goods_receipt",

                        Notes = $"Stock created from approved GRN {grnId}",

                        CreatedAt = DateTime.UtcNow
                    });

                    createdStocks.Add(new
                    {
                        productId,
                        variantId,
                        warehouseId,
                        quantityAdded = quantity,
                        totalQuantity = closingQuantity
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(new
                {
                    success = true,
                    message = "Warehouse stock created successfully from approved GRN.",
                    grnId,
                    warehouseId,
                    stocks = createdStocks
                });
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(
                    exception,
                    "Warehouse stock creation failed for GRN {GrnId}",
                    grnId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Warehouse stock creation failed.",
                    error = exception.InnerException?.Message ??
                            exception.Message
                });
            }
        }

    }
}
