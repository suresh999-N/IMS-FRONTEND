using IMSBackend.Attributes;
using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<ProductController> _logger;
        private const string ProductRulesSectionKey = "product_rules";

        public ProductController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<ProductController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }


        [Permission("Products", "View")]
        [HttpGet]
        public async Task<IActionResult> GetProducts(
    int page = 1,
    int pageSize = 500,
    string? search = null,
    string sortBy = "productId",
    string sortOrder = "desc",
    bool? isArchived = false)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Products
            .Include(x => x.SubCategory)
            .Where(x => !x.IsDeleted && (!isArchived.HasValue || x.IsArchived == isArchived.Value))
            .AsNoTracking()
            .AsQueryable();

            // ================= SEARCH =================
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    x.SKU.ToLower().Contains(search));
            }

            // ================= TOTAL COUNT =================
            var totalRecords = await query.CountAsync();


            // ================= SORTING =================
            query = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("name", "asc") => query.OrderBy(x => x.Name),
                ("name", "desc") => query.OrderByDescending(x => x.Name),

                ("price", "asc") => query.OrderBy(x => x.Price),
                ("price", "desc") => query.OrderByDescending(x => x.Price),

                ("sku", "asc") => query.OrderBy(x => x.SKU),
                ("sku", "desc") => query.OrderByDescending(x => x.SKU),

                _ => query.OrderByDescending(x => x.ProductId)
            };

            // ================= PAGINATION =================
            var products = await query
                
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.ProductId,
                    x.Name,
                    x.SKU,
                    x.Barcode,

                    x.CategoryId,

                    CategoryName =
                        _context.Categories
                            .Where(c => c.CategoryId == x.CategoryId && !c.IsDeleted)
                            .Select(c => c.Name)
                            .FirstOrDefault(),



                    x.SubCategoryId,

                    SubCategoryName =
                        x.SubCategory != null && !x.SubCategory.IsDeleted
                            ? x.SubCategory.Name
                            : "No subcategory",

                    x.BrandId,

                    BrandName =
                        _context.Brands
                            .Where(brand => brand.BrandId == x.BrandId && !brand.IsDeleted)
                            .Select(brand => brand.Name)
                            .FirstOrDefault(),

                    x.UnitId,

                    UnitName =
                        _context.Units
                            .Where(unit => unit.UnitId == x.UnitId && !unit.IsDeleted)
                            .Select(unit => unit.Name)
                            .FirstOrDefault(),

                    x.Price,
                    x.CostPrice,
                    Stock = _context.Stocks
                        .Where(stock => stock.ProductId == x.ProductId)
                        .Sum(stock => (decimal?)stock.Quantity) ?? 0,
                    x.ReorderLevel,

                    x.SupplierId,

                    SupplierName =
                        _context.Suppliers
                            .Where(supplier => supplier.SupplierId == x.SupplierId)
                            .Select(supplier => supplier.Name)
                            .FirstOrDefault(),

                    x.WarehouseId,
                    WarehouseName =
                        _context.Warehouses
                            .Where(warehouse => warehouse.WarehouseId == x.WarehouseId)
                            .Select(warehouse => warehouse.Name)
                            .FirstOrDefault(),
                    x.Status,
                    x.IsArchived,
                    x.Description,
                    x.CreatedAt,
                    x.UpdatedAt,

                    imageUrl = x.ImageUrl
                })
                .ToListAsync();

            // ================= RESPONSE =================
            return Ok(new
            {
                page,
                pageSize,
                totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                data = products
            });
        }



        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(item =>
                    item.ProductId == id &&
                    !item.IsDeleted)
                .Select(item => new
                {
                    item.ProductId,
                    item.Name,
                    item.SKU,
                    item.Barcode,
                    item.CategoryId,
                    item.SubCategoryId,
                    item.BrandId,
                    item.UnitId,
                    item.Price,
                    item.CostPrice,
                    Stock = _context.Stocks
                        .Where(stock => stock.ProductId == item.ProductId)
                        .Sum(stock => (decimal?)stock.Quantity) ?? 0,
                    item.ReorderLevel,
                    item.SupplierId,
                    item.WarehouseId,
                    item.Status,
                    item.IsArchived,
                    item.Description,
                    item.CreatedAt,
                    item.UpdatedAt,
                    item.ImageUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Product was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(product, traceId: HttpContext.TraceIdentifier));
        }


        [Permission("Products", "Add")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product, CancellationToken cancellationToken)
        {
            product.SKU = NormalizeSku(product.SKU);
            try
            {
                product.Barcode = await ResolveBarcodeForCreate(product.Barcode, cancellationToken);
                product.WarehouseId = product.WarehouseId > 0 ? product.WarehouseId : null;
            }
            catch (ProductConflictException exception)
            {
                return Conflict(ApiResponse<object>.Fail(
                    exception.Message,
                    traceId: HttpContext.TraceIdentifier));
            }

            if (string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.SKU))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Product name and SKU are required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var productRules = await GetProductRuleSettings(cancellationToken);
            var duplicateNameError = await ValidateDuplicateProductName(
                product.Name,
                null,
                productRules,
                cancellationToken);

            if (duplicateNameError != null)
            {
                return Conflict(ApiResponse<object>.Fail(
                    duplicateNameError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var referenceError = await ValidateRequiredProductReferences(
                product.CategoryId,
                product.SubCategoryId,
                product.BrandId,
                product.UnitId,
                product.SupplierId,
                product.WarehouseId,
                null,
                productRules,
                cancellationToken);

            if (referenceError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    referenceError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var skuExists = await _context.Products
                .AsNoTracking()
                .AnyAsync(item => item.SKU == product.SKU, cancellationToken);

            if (skuExists)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "SKU already exists.",
                    traceId: HttpContext.TraceIdentifier));
            }

            product.Name = product.Name.Trim();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "CREATE_PRODUCT",
                    "Products",
                    product.ProductId,
                    $"Product created: {product.Name}",
                    "products",
                    cancellationToken);
            }
            catch (DbUpdateException exception) when (IsBarcodeUniqueViolation(exception))
            {
                LogDbUpdateException(exception, "Product create failed due to duplicate barcode.");
                return Conflict(ApiResponse<object>.Fail(
                    "Barcode already exists. Please regenerate barcode.",
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Product create failed due to database update error.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.ProductId },
                ApiResponse<Product>.Ok(product, "Product created.", HttpContext.TraceIdentifier));
        }

        [HttpPost("full")]
        public async Task<IActionResult> CreateFull(ProductDto dto, CancellationToken cancellationToken)
        {
            var productRules = await GetProductRuleSettings(cancellationToken);
            var validationError = ValidateProductDto(dto, productRules);
            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                traceId: HttpContext.TraceIdentifier));
            }

            dto.WarehouseId = await ResolveOpeningStockWarehouseId(
                dto.WarehouseId,
                dto.Stock,
                cancellationToken);

            var referenceError = await ValidateRequiredProductReferences(
                dto.CategoryId,
                dto.SubCategoryId,
                dto.BrandId,
                dto.UnitId,
                dto.SupplierId,
                dto.WarehouseId,
                dto.Stock,
                productRules,
                cancellationToken);

            if (referenceError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    referenceError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var variantError = await ValidateVariantDrafts(dto.Variants, productRules, cancellationToken);

            if (variantError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    variantError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var normalizedSku = NormalizeSku(dto.SKU);
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(item => item.SKU == normalizedSku, cancellationToken);

                var isNewProduct = product == null;
                var duplicateNameError = await ValidateDuplicateProductName(
                    dto.Name,
                    product?.ProductId,
                    productRules,
                    cancellationToken);

                if (duplicateNameError != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Conflict(ApiResponse<object>.Fail(
                        duplicateNameError,
                        traceId: HttpContext.TraceIdentifier));
                }

                var requestedBarcode = NormalizeBarcode(dto.Barcode);
                var barcode = isNewProduct
                    ? await ResolveBarcodeForCreate(requestedBarcode, cancellationToken)
                    : await ResolveBarcodeForUpdate(
                        string.IsNullOrWhiteSpace(requestedBarcode)
                            ? product?.Barcode
                            : requestedBarcode,
                        product!.ProductId,
                        cancellationToken);

                if (product == null)
                {
                    product = new Product
                    {
                        SKU = normalizedSku,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Products.Add(product);
                }

                ApplyProductDto(product, dto, normalizedSku, barcode);
                await _context.SaveChangesAsync(cancellationToken);

                await SyncOpeningStock(product, dto, isNewProduct, cancellationToken);
                await UpsertVariants(product, dto.Variants, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    isNewProduct ? "CREATE_PRODUCT" : "UPDATE_PRODUCT",
                    "Products",
                    product.ProductId,
                    isNewProduct ? $"Product created: {product.Name}" : $"Product updated: {product.Name}",
                    "products",
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Ok(ApiResponse<Product>.Ok(
                    product,
                    "Product saved successfully.",
                    HttpContext.TraceIdentifier));
            }
            catch (InvalidOperationException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest(ApiResponse<object>.Fail(
                    exception.Message,
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (ProductConflictException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Conflict(ApiResponse<object>.Fail(
                    exception.Message,
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception) when (IsBarcodeUniqueViolation(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Product save failed due to duplicate barcode.");
                return Conflict(ApiResponse<object>.Fail(
                    "Barcode already exists. Please regenerate barcode.",
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Product save failed due to database update error.");
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
                    "Product save failed. InnerException: {InnerException}. TraceId: {TraceId}",
                    exception.InnerException?.ToString() ?? "No inner exception",
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }
        }



        [Permission("Products", "Edit")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Product updated, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Product was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var normalizedSku = NormalizeSku(updated.SKU);
            if (string.IsNullOrWhiteSpace(updated.Name) || string.IsNullOrWhiteSpace(normalizedSku))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Product name and SKU are required.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var duplicateSku = await _context.Products
                .AsNoTracking()
                .AnyAsync(item => item.SKU == normalizedSku && item.ProductId != id, cancellationToken);

            if (duplicateSku)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "SKU already exists.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var productRules = await GetProductRuleSettings(cancellationToken);
            var duplicateNameError = await ValidateDuplicateProductName(
                updated.Name,
                id,
                productRules,
                cancellationToken);

            if (duplicateNameError != null)
            {
                return Conflict(ApiResponse<object>.Fail(
                    duplicateNameError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var referenceError = await ValidateRequiredProductReferences(
                updated.CategoryId,
                updated.SubCategoryId,
                updated.BrandId,
                updated.UnitId,
                updated.SupplierId,
                updated.WarehouseId,
                null,
                productRules,
                cancellationToken);

            if (referenceError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    referenceError,
                    traceId: HttpContext.TraceIdentifier));
            }

            string resolvedBarcode;
            try
            {
                resolvedBarcode = await ResolveBarcodeForUpdate(
                    updated.Barcode,
                    id,
                    cancellationToken);
            }
            catch (ProductConflictException exception)
            {
                return Conflict(ApiResponse<object>.Fail(
                    exception.Message,
                    traceId: HttpContext.TraceIdentifier));
            }

            product.Name = updated.Name.Trim();
            product.SKU = normalizedSku;
            product.Barcode = resolvedBarcode;
            product.CategoryId = updated.CategoryId;
            product.SubCategoryId = updated.SubCategoryId;
            product.BrandId = updated.BrandId;
            product.UnitId = updated.UnitId;
            product.Price = updated.Price;
            product.CostPrice = updated.CostPrice;
            product.ReorderLevel = updated.ReorderLevel;
            product.SupplierId = updated.SupplierId;
            product.WarehouseId = updated.WarehouseId;
            product.Status = string.IsNullOrWhiteSpace(updated.Status) ? product.Status : updated.Status.Trim();
            product.Description = updated.Description?.Trim() ?? string.Empty;
            product.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "UPDATE_PRODUCT",
                    "Products",
                    product.ProductId,
                    $"Product updated: {product.Name}",
                    "products",
                    cancellationToken);
            }
            catch (ProductConflictException exception)
            {
                return Conflict(ApiResponse<object>.Fail(
                    exception.Message,
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception) when (IsBarcodeUniqueViolation(exception))
            {
                LogDbUpdateException(exception, "Product update failed due to duplicate barcode.");
                return Conflict(ApiResponse<object>.Fail(
                    "Barcode already exists. Please regenerate barcode.",
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Product update failed due to database update error.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<Product>.Ok(
                product,
                "Product updated.",
                HttpContext.TraceIdentifier));
        }





        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(
            int id,
            [FromBody] Dictionary<string, JsonElement> updates,
            CancellationToken cancellationToken)
        {
            if (updates == null || updates.Count == 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "No product changes were submitted.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Product was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var changedReferenceFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var productRules = await GetProductRuleSettings(cancellationToken);

            foreach (var update in updates)
            {
                var field = NormalizePatchField(update.Key);

                try
                {
                    switch (field)
                    {
                        case "name":
                        {
                            var name = ReadOptionalString(update.Value)?.Trim();
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                return BadRequest(ApiResponse<object>.Fail(
                                    "Product name is required.",
                                    traceId: HttpContext.TraceIdentifier));
                            }

                            var duplicateNameError = await ValidateDuplicateProductName(
                                name,
                                id,
                                productRules,
                                cancellationToken);

                            if (duplicateNameError != null)
                            {
                                return Conflict(ApiResponse<object>.Fail(
                                    duplicateNameError,
                                    traceId: HttpContext.TraceIdentifier));
                            }

                            product.Name = name;
                            break;
                        }

                        case "sku":
                        {
                            var normalizedSku = NormalizeSku(ReadOptionalString(update.Value));
                            if (string.IsNullOrWhiteSpace(normalizedSku))
                            {
                                return BadRequest(ApiResponse<object>.Fail(
                                    "Product SKU is required.",
                                    traceId: HttpContext.TraceIdentifier));
                            }

                            var duplicateSku = await _context.Products
                                .AsNoTracking()
                                .AnyAsync(item => item.SKU == normalizedSku && item.ProductId != id, cancellationToken);

                            if (duplicateSku)
                            {
                                return Conflict(ApiResponse<object>.Fail(
                                    "SKU already exists.",
                                    traceId: HttpContext.TraceIdentifier));
                            }

                            product.SKU = normalizedSku;
                            break;
                        }

                        case "barcode":
                            product.Barcode = await ResolveBarcodeForUpdate(
                                ReadOptionalString(update.Value),
                                id,
                                cancellationToken);
                            break;

                        case "categoryid":
                            product.CategoryId = ReadNullableInt(update.Value, "Category");
                            changedReferenceFields.Add("categoryId");
                            break;

                        case "subcategoryid":
                            product.SubCategoryId = ReadNullableInt(update.Value, "SubCategory");
                            changedReferenceFields.Add("subCategoryId");
                            break;

                        case "brandid":
                            product.BrandId = ReadNullableInt(update.Value, "Brand");
                            changedReferenceFields.Add("brandId");
                            break;

                        case "unitid":
                            product.UnitId = ReadNullableInt(update.Value, "Unit");
                            changedReferenceFields.Add("unitId");
                            break;

                        case "price":
                            product.Price = ReadNullableDecimal(update.Value, "Price", allowZero: false);
                            break;

                        case "costprice":
                            product.CostPrice = ReadNullableDecimal(update.Value, "Cost", allowZero: true);
                            break;

                        case "stock":
                            return BadRequest(ApiResponse<object>.Fail(
                                "Product stock is system-managed from Stock Register and cannot be edited directly.",
                                traceId: HttpContext.TraceIdentifier));

                        case "reorderlevel":
                            product.ReorderLevel = ReadNullableInt(update.Value, "Reorder level", minValue: 0);
                            break;

                        case "supplierid":
                            product.SupplierId = ReadNullableInt(update.Value, "Supplier");
                            changedReferenceFields.Add("supplierId");
                            break;

                        case "warehouseid":
                            product.WarehouseId = ReadNullableInt(update.Value, "Warehouse");
                            changedReferenceFields.Add("warehouseId");
                            break;

                        case "status":
                        {
                            var status = ReadOptionalString(update.Value)?.Trim().ToLowerInvariant();
                            if (status != "active" && status != "inactive")
                            {
                                return BadRequest(ApiResponse<object>.Fail(
                                    "Status must be Active or Inactive.",
                                    traceId: HttpContext.TraceIdentifier));
                            }

                            product.Status = status;
                            break;
                        }

                        case "isarchived":
                            product.IsArchived = ReadBoolean(update.Value, "Archived");
                            break;

                        case "description":
                            product.Description = ReadOptionalString(update.Value)?.Trim() ?? string.Empty;
                            break;

                        case "image":
                        case "imageurl":
                            product.ImageUrl = ReadOptionalString(update.Value)?.Trim();
                            break;

                        default:
                            return BadRequest(ApiResponse<object>.Fail(
                                $"Field '{update.Key}' cannot be updated.",
                                traceId: HttpContext.TraceIdentifier));
                    }
                }
                catch (ArgumentException exception)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        exception.Message,
                        traceId: HttpContext.TraceIdentifier));
                }
                catch (ProductConflictException exception)
                {
                    return Conflict(ApiResponse<object>.Fail(
                        exception.Message,
                        traceId: HttpContext.TraceIdentifier));
                }
            }

            var patchReferenceError = await ValidatePatchProductReferences(
                product,
                changedReferenceFields,
                productRules,
                cancellationToken);

            if (patchReferenceError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    patchReferenceError,
                    traceId: HttpContext.TraceIdentifier));
            }

            product.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "UPDATE_PRODUCT",
                    "Products",
                    product.ProductId,
                    $"Product updated: {product.Name}",
                    "products",
                    cancellationToken);
            }
            catch (DbUpdateException exception) when (IsBarcodeUniqueViolation(exception))
            {
                LogDbUpdateException(exception, "Product patch failed due to duplicate barcode.");
                return Conflict(ApiResponse<object>.Fail(
                    "Barcode already exists. Please regenerate barcode.",
                    traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Product patch failed due to database update error.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<Product>.Ok(
                product,
                "Product changes saved.",
                HttpContext.TraceIdentifier));
        }



        [Permission("Products", "Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Product was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            _logger.LogInformation(
                "Product soft delete requested. ProductId: {ProductId}, IsDeleted: {IsDeleted}, TraceId: {TraceId}",
                id,
                product.IsDeleted,
                HttpContext.TraceIdentifier);

            try
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "DELETE_PRODUCT",
                    "Products",
                    product.ProductId,
                    $"Product deleted: {product.Name}",
                    "products",
                    cancellationToken);

                _logger.LogInformation(
                    "Product soft delete committed. ProductId: {ProductId}, TraceId: {TraceId}",
                    id,
                    HttpContext.TraceIdentifier);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Product delete failed due to database update error.");
                return Conflict(ApiResponse<object>.Fail(
                    GetProductDeleteDependencyMessage(exception),
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(
                null,
                "Product deleted.",
                HttpContext.TraceIdentifier));
        }

        [HttpGet("{id:int}/delete-dependencies")]
        public async Task<IActionResult> GetDeleteDependencies(int id, CancellationToken cancellationToken)
        {
            var dependencyReport = await BuildProductDeleteDependencyReport(id, cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                dependencyReport,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpGet("{id:int}/dependencies")]
        public async Task<IActionResult> GetDependencies(int id, CancellationToken cancellationToken)
        {
            var dependencyReport = await BuildProductDeleteDependencyReport(id, cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                dependencyReport,
                traceId: HttpContext.TraceIdentifier));
        }

        private async Task CleanupSalesAndInvoiceDependenciesForProduct(int productId, CancellationToken cancellationToken)
        {
            var invoiceIds = await _context.InvoiceItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId && item.InvoiceId.HasValue)
                .Select(item => item.InvoiceId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);
            var orphanInvoiceItems = await _context.InvoiceItems
                .Where(item => item.ProductId == productId && !item.InvoiceId.HasValue)
                .ToListAsync(cancellationToken);

            var salesOrderIds = await _context.SalesOrderItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId && item.SoId.HasValue)
                .Select(item => item.SoId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);
            var orphanSalesOrderItems = await _context.SalesOrderItems
                .Where(item => item.ProductId == productId && !item.SoId.HasValue)
                .ToListAsync(cancellationToken);
            _logger.LogInformation(
                "Product delete sales/invoice cleanup starting. ProductId: {ProductId}, InvoiceIds: {InvoiceIds}, SalesOrderIds: {SalesOrderIds}, OrphanInvoiceItems: {OrphanInvoiceItems}, OrphanSalesOrderItems: {OrphanSalesOrderItems}, TraceId: {TraceId}",
                productId,
                string.Join(",", invoiceIds),
                string.Join(",", salesOrderIds),
                orphanInvoiceItems.Count,
                orphanSalesOrderItems.Count,
                HttpContext.TraceIdentifier);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var invoiceId in invoiceIds)
                {
                    await DeleteInvoiceGraph(invoiceId, cancellationToken);
                }

                foreach (var salesOrderId in salesOrderIds)
                {
                    await DeleteSalesOrderGraph(salesOrderId, cancellationToken);
                }

                _context.InvoiceItems.RemoveRange(orphanInvoiceItems);
                _context.SalesOrderItems.RemoveRange(orphanSalesOrderItems);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                LogDbUpdateException(exception, "Product sales/invoice dependency cleanup failed.");
                throw;
            }
        }

        private async Task<object> CleanupDirectProductDependencies(int productId, CancellationToken cancellationToken)
        {
            var variantIds = await _context.ProductVariants
                .Where(item => item.ProductId == productId)
                .Select(item => item.VariantId)
                .ToListAsync(cancellationToken);

            var variantAttributeValues = await _context.VariantAttributeValues
                .Where(item => variantIds.Contains(item.VariantId))
                .ToListAsync(cancellationToken);
            var productVariants = await _context.ProductVariants
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var barcodes = await _context.Barcodes
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var stocks = await _context.Stocks
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var stockMovements = await _context.StockMovements
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var stockLedgers = await _context.StockLedgers
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var stockAuditItems = await _context.StockAuditItems
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var stockAdjustmentItems = await _context.StockAdjustmentItems
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var stockTransferItems = await _context.StockTransferItems
                .Where(item => item.ProductId == productId)
                .ToListAsync(cancellationToken);
            var auditLogs = await _context.AuditLogs
                .Where(item =>
                    item.RecordId == productId &&
                    (item.TableName == "products" ||
                     item.TableName == "product" ||
                     item.TableName == "stock" ||
                     item.TableName == "stock_movements" ||
                     item.TableName == "stock_ledger"))
                .ToListAsync(cancellationToken);

            var summary = new
            {
                ProductId = productId,
                VariantAttributeValues = variantAttributeValues.Count,
                ProductVariants = productVariants.Count,
                Barcodes = barcodes.Count,
                StockRegister = stocks.Count,
                StockMovements = stockMovements.Count,
                StockLedger = stockLedgers.Count,
                StockAuditItems = stockAuditItems.Count,
                StockAdjustmentItems = stockAdjustmentItems.Count,
                StockTransferItems = stockTransferItems.Count,
                AuditLogs = auditLogs.Count
            };

            _logger.LogInformation(
                "Product direct dependency cleanup. {@CleanupSummary}, TraceId: {TraceId}",
                summary,
                HttpContext.TraceIdentifier);

            _context.VariantAttributeValues.RemoveRange(variantAttributeValues);
            _context.StockTransferItems.RemoveRange(stockTransferItems);
            _context.StockAdjustmentItems.RemoveRange(stockAdjustmentItems);
            _context.StockAuditItems.RemoveRange(stockAuditItems);
            _context.StockLedgers.RemoveRange(stockLedgers);
            _context.StockMovements.RemoveRange(stockMovements);
            _context.Stocks.RemoveRange(stocks);
            _context.Barcodes.RemoveRange(barcodes);
            _context.ProductVariants.RemoveRange(productVariants);
            _context.AuditLogs.RemoveRange(auditLogs);

            return summary;
        }

        private async Task DeleteInvoiceGraph(int invoiceId, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(item => item.InvoiceItems!)
                .FirstOrDefaultAsync(item => item.InvoiceId == invoiceId, cancellationToken);

            if (invoice == null)
            {
                var orphanItems = await _context.InvoiceItems
                    .Where(item => item.InvoiceId == invoiceId)
                    .ToListAsync(cancellationToken);

                _context.InvoiceItems.RemoveRange(orphanItems);
                return;
            }

            var movements = await _context.StockMovements
                .Where(item => item.ReferenceId == invoiceId && item.ReferenceType == "invoice")
                .ToListAsync(cancellationToken);
            var ledgers = await _context.StockLedgers
                .Where(item => item.TransactionId == invoiceId &&
                    (item.TransactionType == "sale" ||
                     item.TransactionType == "invoice" ||
                     item.TransactionType == "invoice_delete"))
                .ToListAsync(cancellationToken);

            if (invoice.CustomerId.HasValue)
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(item => item.CustomerId == invoice.CustomerId.Value, cancellationToken);

                if (customer != null)
                {
                    customer.OutstandingBalance = Math.Max(customer.OutstandingBalance - invoice.BalanceAmount, 0);
                    customer.UpdatedAt = DateTime.UtcNow;
                }
            }

            var customerLedgers = await _context.CustomerLedgers
                .Where(item => item.TransactionType == "invoice" && item.TransactionId == invoiceId)
                .ToListAsync(cancellationToken);
            var payments = await _context.CustomerPayments
                .Where(item => item.InvoiceId == invoiceId)
                .ToListAsync(cancellationToken);
            var auditLogs = await _context.AuditLogs
                .Where(item => item.TableName == "invoices" && item.RecordId == invoiceId)
                .ToListAsync(cancellationToken);

            _context.CustomerPayments.RemoveRange(payments);
            _context.CustomerLedgers.RemoveRange(customerLedgers);
            _context.StockLedgers.RemoveRange(ledgers);
            _context.StockMovements.RemoveRange(movements);
            _context.AuditLogs.RemoveRange(auditLogs);
            _context.InvoiceItems.RemoveRange(invoice.InvoiceItems ?? new List<InvoiceItem>());
            _context.Invoices.Remove(invoice);
        }

        private async Task DeleteSalesOrderGraph(int salesOrderId, CancellationToken cancellationToken)
        {
            var salesOrder = await _context.SalesOrders
                .FirstOrDefaultAsync(item => item.SoId == salesOrderId, cancellationToken);
            var salesItems = await _context.SalesOrderItems
                .Where(item => item.SoId == salesOrderId)
                .ToListAsync(cancellationToken);
            var invoiceIds = await _context.Invoices
                .Where(item => item.SoId == salesOrderId)
                .Select(item => item.InvoiceId)
                .ToListAsync(cancellationToken);

            foreach (var invoiceId in invoiceIds)
            {
                await DeleteInvoiceGraph(invoiceId, cancellationToken);
            }

            var auditLogs = await _context.AuditLogs
                .Where(item => item.TableName == "sales_orders" && item.RecordId == salesOrderId)
                .ToListAsync(cancellationToken);

            _context.AuditLogs.RemoveRange(auditLogs);
            _context.SalesOrderItems.RemoveRange(salesItems);

            if (salesOrder != null)
            {
                _context.SalesOrders.Remove(salesOrder);
            }
        }



        private async Task<string> GetSalesInvoiceDependencyMessage(int productId, CancellationToken cancellationToken)
        {
            var invoiceIds = await _context.InvoiceItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.InvoiceId)
                .Distinct()
                .ToListAsync(cancellationToken);
            var salesOrderIds = await _context.SalesOrderItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.SoId)
                .Distinct()
                .ToListAsync(cancellationToken);
            var invoiceItemIds = await _context.InvoiceItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
            var salesItemIds = await _context.SalesOrderItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
            var messages = new List<string>();

            if (invoiceItemIds.Count > 0)
            {
                messages.Add($"InvoiceItems rows exist: Ids [{string.Join(",", invoiceItemIds)}]. ProductId={productId}. InvoiceIds=[{string.Join(",", invoiceIds.Select(item => item?.ToString() ?? "null"))}]");
            }

            if (salesItemIds.Count > 0)
            {
                messages.Add($"SalesItems rows exist: Ids [{string.Join(",", salesItemIds)}]. ProductId={productId}. SalesOrderIds=[{string.Join(",", salesOrderIds.Select(item => item?.ToString() ?? "null"))}]");
            }

            return messages.Count > 0
                ? string.Join("; ", messages)
                : $"Sales or invoice records exist for ProductId={productId}";
        }

        private async Task<object> BuildProductDeleteDependencyReport(int productId, CancellationToken cancellationToken)
        {
            var invoiceIds = await _context.InvoiceItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.InvoiceId)
                .Distinct()
                .ToListAsync(cancellationToken);
            var salesOrderIds = await _context.SalesOrderItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.SoId)
                .Distinct()
                .ToListAsync(cancellationToken);
            var invoiceItemIds = await _context.InvoiceItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
            var salesItemIds = await _context.SalesOrderItems
                .AsNoTracking()
                .Where(item => item.ProductId == productId)
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
            _logger.LogInformation(
                "Product dependency report. ProductId: {ProductId}, InvoiceItemIds: {InvoiceItemIds}, InvoiceIds: {InvoiceIds}, SalesItemIds: {SalesItemIds}, SalesOrderIds: {SalesOrderIds}, SalesTransactions: {SalesTransactions}, InvoiceDetails: {InvoiceDetails}, CustomerInvoiceLines: {CustomerInvoiceLines}, TraceId: {TraceId}",
                productId,
                string.Join(",", invoiceItemIds),
                string.Join(",", invoiceIds.Select(item => item?.ToString() ?? "null")),
                string.Join(",", salesItemIds),
                string.Join(",", salesOrderIds.Select(item => item?.ToString() ?? "null")),
                "No DbSet",
                "No DbSet",
                "No DbSet",
                HttpContext.TraceIdentifier);

            return new
            {
                productId,
                invoiceItems = invoiceItemIds,
                salesItems = salesItemIds,
                salesReturns = Array.Empty<int>(),
                invoiceItemDetails = new
                {
                    Count = await _context.InvoiceItems.AsNoTracking().CountAsync(item => item.ProductId == productId, cancellationToken),
                    InvoiceIds = invoiceIds,
                    RowIds = invoiceItemIds
                },
                salesItemDetails = new
                {
                    Count = await _context.SalesOrderItems.AsNoTracking().CountAsync(item => item.ProductId == productId, cancellationToken),
                    SalesOrderIds = salesOrderIds,
                    RowIds = salesItemIds
                },
                salesReturnDetails = new
                {
                    Count = 0,
                    SalesReturnIds = Array.Empty<int>(),
                    RowIds = Array.Empty<int>()
                },
                salesInvoices = invoiceIds.Where(item => item.HasValue).Select(item => item!.Value).ToList(),
                salesTransactions = Array.Empty<int>(),
                invoiceDetails = Array.Empty<int>(),
                customerInvoiceLines = Array.Empty<int>(),
                unavailableTables = new[]
                {
                    "SalesTransactions",
                    "InvoiceDetails",
                    "CustomerInvoiceLines"
                }
            };
        }

        private async Task SyncOpeningStock(
            Product product,
            ProductDto dto,
            bool isNewProduct,
            CancellationToken cancellationToken)
        {
            if (dto.Stock == null)
            {
                return;
            }

            var quantity = Math.Max(dto.Stock.Value, 0);
            if (quantity > 0 && (dto.WarehouseId == null || dto.WarehouseId <= 0))
            {
                throw new InvalidOperationException("Warehouse is required when opening stock is added.");
            }

            if (dto.WarehouseId == null || dto.WarehouseId <= 0)
            {
                return;
            }

            var warehouseId = dto.WarehouseId.Value;
            var stock = await _context.Stocks.FirstOrDefaultAsync(
                item =>
                    item.ProductId == product.ProductId &&
                    item.WarehouseId == warehouseId &&
                    item.VariantId == null,
                cancellationToken);

            if (stock == null)
            {
                stock = new Stock
                {
                    ProductId = product.ProductId,
                    WarehouseId = warehouseId,
                    Quantity = quantity,
                    ReservedQuantity = 0
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                stock.Quantity = quantity;
            }

            if (!isNewProduct || quantity <= 0)
            {
                return;
            }

            _context.StockMovements.Add(new StockMovement
            {
                ProductId = product.ProductId,
                WarehouseId = warehouseId,
                MovementType = "OPENING",
                Quantity = quantity,
                ReferenceId = product.ProductId,
                ReferenceType = "opening_stock",
                Notes = "Opening stock from product creation",
                CreatedAt = DateTime.UtcNow
            });

            _context.StockLedgers.Add(new StockLedger
            {
                ProductId = product.ProductId,
                WarehouseId = warehouseId,
                OpeningQty = 0,
                ChangeQty = quantity,
                ClosingQty = quantity,
                TransactionType = "Opening Stock",
                TransactionId = product.ProductId,
                CreatedAt = DateTime.UtcNow
            });
        }

        private async Task UpsertVariants(
            Product product,
            IReadOnlyCollection<ProductVariantCreateDto>? variants,
            CancellationToken cancellationToken)
        {
            if (variants == null || variants.Count == 0)
            {
                return;
            }

            var variantDrafts = variants.ToList();

            for (var index = 0; index < variantDrafts.Count; index++)
            {
                var draft = variantDrafts[index];
                var variantSku = NormalizeSku(
                    string.IsNullOrWhiteSpace(draft.SKU)
                        ? $"{product.SKU}-VAR-{index + 1}"
                        : draft.SKU);
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(
                        item => item.ProductId == product.ProductId && item.SKU == variantSku,
                        cancellationToken);

                if (variant == null)
                {
                    variant = new ProductVariant
                    {
                        ProductId = product.ProductId,
                        SKU = variantSku
                    };
                    _context.ProductVariants.Add(variant);
                }

                variant.VariantName = string.IsNullOrWhiteSpace(draft.VariantName)
                    ? "Default"
                    : draft.VariantName.Trim();
                variant.Price = (product.Price ?? 0) + (draft.PriceDelta ?? 0);
                variant.CostPrice = product.CostPrice;

                await _context.SaveChangesAsync(cancellationToken);
                await UpsertVariantAttributes(variant.VariantId, draft.Attributes, cancellationToken);
            }
        }

        private async Task UpsertVariantAttributes(
            int variantId,
            IReadOnlyCollection<VariantAttributeValueDto>? attributes,
            CancellationToken cancellationToken)
        {
            var desiredAttributes = (attributes ?? [])
                .Where(attribute => attribute.AttributeId > 0 && attribute.ValueId > 0)
                .GroupBy(attribute => new { attribute.AttributeId, attribute.ValueId })
                .Select(group => group.First())
                .ToList();

            var existingAttributes = await _context.VariantAttributeValues
                .Where(item => item.VariantId == variantId)
                .ToListAsync(cancellationToken);

            if (desiredAttributes.Count == 0)
            {
                if (existingAttributes.Count > 0)
                {
                    _context.VariantAttributeValues.RemoveRange(existingAttributes);
                }

                return;
            }

            foreach (var existingAttribute in existingAttributes)
            {
                var stillRequested = desiredAttributes.Any(
                    attribute =>
                        attribute.AttributeId == existingAttribute.AttributeId &&
                        attribute.ValueId == existingAttribute.ValueId);

                if (!stillRequested)
                {
                    _context.VariantAttributeValues.Remove(existingAttribute);
                }
            }

            foreach (var attribute in desiredAttributes)
            {
                var valueExists = await _context.AttributeValues
                    .AsNoTracking()
                    .AnyAsync(
                        item =>
                            item.AttributeId == attribute.AttributeId &&
                            item.ValueId == attribute.ValueId,
                        cancellationToken);

                if (!valueExists)
                {
                    throw new InvalidOperationException(
                        $"Invalid value {attribute.ValueId} for attribute {attribute.AttributeId}.");
                }

                var alreadyExists = existingAttributes.Any(
                    item =>
                        item.AttributeId == attribute.AttributeId &&
                        item.ValueId == attribute.ValueId);

                if (!alreadyExists)
                {
                    _context.VariantAttributeValues.Add(new VariantAttributeValue
                    {
                        VariantId = variantId,
                        AttributeId = attribute.AttributeId,
                        ValueId = attribute.ValueId
                    });
                }
            }
        }

        private static void ApplyProductDto(
            Product product,
            ProductDto dto,
            string normalizedSku,
            string barcode)
        {
            product.Name = dto.Name.Trim();
            product.SKU = normalizedSku;
            product.Barcode = barcode;
            product.CategoryId = dto.CategoryId;
            product.SubCategoryId = dto.SubCategoryId;
            product.BrandId = dto.BrandId;
            product.UnitId = dto.UnitId;
            product.Price = dto.Price;
            product.CostPrice = dto.CostPrice;
            product.ReorderLevel = dto.ReorderLevel;
            product.SupplierId = dto.SupplierId;
            product.WarehouseId = dto.WarehouseId;
            product.Status = string.IsNullOrWhiteSpace(dto.Status) ? "active" : dto.Status.Trim();
            product.Description = dto.Description?.Trim() ?? string.Empty;
            product.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl)
                ? product.ImageUrl
                : dto.ImageUrl.Trim();
            product.UpdatedAt = DateTime.UtcNow;
        }

        private async Task<int?> ResolveOpeningStockWarehouseId(
            int? warehouseId,
            int? stock,
            CancellationToken cancellationToken)
        {
            if ((stock ?? 0) <= 0)
            {
                return warehouseId;
            }

            if (warehouseId.HasValue && warehouseId.Value > 0)
            {
                return warehouseId.Value;
            }

            return await _context.Warehouses
                .AsNoTracking()
                .OrderBy(warehouse => warehouse.WarehouseId)
                .Select(warehouse => (int?)warehouse.WarehouseId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private sealed class ProductRuleSettings
        {
            public bool BrandRequired { get; init; } = true;
            public bool CategoryRequired { get; init; } = true;
            public bool SubCategoryRequired { get; init; } = false;
            public bool DuplicateProductNameAllowed { get; init; } = false;
            public bool AllowProductVariants { get; init; } = true;
            public bool AttributeRequiredForVariants { get; init; } = true;
        }

        private async Task<ProductRuleSettings> GetProductRuleSettings(CancellationToken cancellationToken)
        {
            var rules = await _context.SystemSettingRules
                .AsNoTracking()
                .Include(rule => rule.Section)
                .Where(rule => rule.Section != null && rule.Section.SectionKey == ProductRulesSectionKey)
                .ToListAsync(cancellationToken);

            bool ReadToggle(string ruleKey, bool fallback)
            {
                var rule = rules.FirstOrDefault(item =>
                    string.Equals(item.RuleKey, ruleKey, StringComparison.OrdinalIgnoreCase));

                return rule?.IsEnabled ?? fallback;
            }

            return new ProductRuleSettings
            {
                BrandRequired = ReadToggle("brandRequired", true),
                CategoryRequired = ReadToggle("categoryRequired", true),
                SubCategoryRequired = ReadToggle("subCategoryRequired", false),
                DuplicateProductNameAllowed = ReadToggle("duplicateProductNameAllowed", false),
                AllowProductVariants = ReadToggle("allowProductVariants", true),
                AttributeRequiredForVariants = ReadToggle("attributeRequiredForVariants", true)
            };
        }

        private static string? ValidateProductDto(ProductDto dto, ProductRuleSettings rules)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return "Product name is required.";
            }

            if (string.IsNullOrWhiteSpace(dto.SKU))
            {
                return "Product SKU is required.";
            }

            if ((rules.CategoryRequired || rules.SubCategoryRequired) && (dto.CategoryId == null || dto.CategoryId <= 0))
            {
                return "Category is required.";
            }

            if (rules.BrandRequired && (dto.BrandId == null || dto.BrandId <= 0))
            {
                return "Brand is required.";
            }

            if (rules.SubCategoryRequired && (dto.SubCategoryId == null || dto.SubCategoryId <= 0))
            {
                return "SubCategory is required.";
            }

            if (dto.UnitId == null || dto.UnitId <= 0)
            {
                return "Unit is required.";
            }

            if (dto.Price < 0 || dto.CostPrice < 0 || dto.Stock < 0 || dto.ReorderLevel < 0)
            {
                return "Product amounts and quantities cannot be negative.";
            }

            return null;
        }

        private static string NormalizePatchField(string field)
            => field.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();

        private static string? ReadOptionalString(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            return element.ValueKind == JsonValueKind.String
                ? element.GetString()
                : element.ToString();
        }

        private static int? ReadNullableInt(JsonElement element, string fieldName, int? minValue = null)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            int value;
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var numberValue))
            {
                value = numberValue;
            }
            else
            {
                var stringValue = ReadOptionalString(element)?.Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null;
                }

                if (!int.TryParse(stringValue, out value))
                {
                    throw new ArgumentException($"{fieldName} must be a whole number.");
                }
            }

            if (minValue.HasValue && value < minValue.Value)
            {
                throw new ArgumentException($"{fieldName} cannot be negative.");
            }

            return value;
        }

        private static bool ReadBoolean(JsonElement element, string fieldName)
        {
            if (element.ValueKind == JsonValueKind.True)
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.False)
            {
                return false;
            }

            var stringValue = ReadOptionalString(element)?.Trim();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"{fieldName} must be true or false.");
            }

            if (bool.TryParse(stringValue, out var booleanValue))
            {
                return booleanValue;
            }

            if (stringValue == "1")
            {
                return true;
            }

            if (stringValue == "0")
            {
                return false;
            }

            throw new ArgumentException($"{fieldName} must be true or false.");
        }

        private static decimal? ReadNullableDecimal(JsonElement element, string fieldName, bool allowZero)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            decimal value;
            if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out var numberValue))
            {
                value = numberValue;
            }
            else
            {
                var stringValue = ReadOptionalString(element)?.Trim();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null;
                }

                if (!decimal.TryParse(
                    stringValue,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out value))
                {
                    throw new ArgumentException($"{fieldName} must be a valid amount.");
                }
            }

            if (value < 0 || (!allowZero && value == 0))
            {
                throw new ArgumentException(
                    allowZero
                        ? $"{fieldName} cannot be negative."
                        : $"{fieldName} must be greater than zero.");
            }

            return value;
        }

        private static string NormalizeSku(string? sku)
            => sku?.Trim().ToUpperInvariant() ?? string.Empty;

        private static string NormalizeBarcode(string? barcode)
            => barcode?.Trim().ToUpperInvariant() ?? string.Empty;

        private async Task<string> ResolveBarcodeForCreate(
            string? barcode,
            CancellationToken cancellationToken)
        {
            var normalizedBarcode = NormalizeBarcode(barcode);

            if (string.IsNullOrWhiteSpace(normalizedBarcode))
            {
                return await GenerateUniqueBarcode(cancellationToken);
            }

            var barcodeExists = await BarcodeExists(
                normalizedBarcode,
                excludingProductId: null,
                cancellationToken);

            return barcodeExists
                ? await GenerateUniqueBarcode(cancellationToken)
                : normalizedBarcode;
        }

        private async Task<string> ResolveBarcodeForUpdate(
            string? barcode,
            int productId,
            CancellationToken cancellationToken)
        {
            var normalizedBarcode = NormalizeBarcode(barcode);

            if (string.IsNullOrWhiteSpace(normalizedBarcode))
            {
                return await GenerateUniqueBarcode(cancellationToken);
            }

            var barcodeExists = await BarcodeExists(
                normalizedBarcode,
                productId,
                cancellationToken);

            if (barcodeExists)
            {
                throw new ProductConflictException(
                    "Barcode already exists. Please regenerate barcode.");
            }

            return normalizedBarcode;
        }

        private async Task<bool> BarcodeExists(
            string barcode,
            int? excludingProductId,
            CancellationToken cancellationToken)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(product => product.Barcode == barcode);

            if (excludingProductId.HasValue)
            {
                query = query.Where(product => product.ProductId != excludingProductId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        private async Task<string> GenerateUniqueBarcode(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow;
            var prefix = $"BAR-{today:yyyyMMdd}-";
            var existingBarcodes = await _context.Products
                .AsNoTracking()
                .Where(product => product.Barcode.StartsWith(prefix))
                .Select(product => product.Barcode)
                .ToListAsync(cancellationToken);
            var usedBarcodes = existingBarcodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var nextSequence = existingBarcodes
                .Select(ReadBarcodeSequence)
                .DefaultIfEmpty(0)
                .Max() + 1;

            for (var attempt = 0; attempt < 1000; attempt++)
            {
                var candidate = $"{prefix}{nextSequence + attempt:000}";

                if (
                    !usedBarcodes.Contains(candidate) &&
                    !await BarcodeExists(candidate, excludingProductId: null, cancellationToken))
                {
                    return candidate;
                }
            }

            var fallbackBarcode = $"{prefix}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            if (await BarcodeExists(fallbackBarcode, excludingProductId: null, cancellationToken))
            {
                throw new ProductConflictException(
                    "Barcode already exists. Please regenerate barcode.");
            }

            return fallbackBarcode;
        }

        private static int ReadBarcodeSequence(string? barcode)
        {
            var lastSegment = barcode?.Split('-').LastOrDefault();
            return int.TryParse(lastSegment, out var sequence) ? sequence : 0;
        }

        private async Task<string?> ValidateVariantDrafts(
            IReadOnlyCollection<ProductVariantCreateDto>? variants,
            ProductRuleSettings rules,
            CancellationToken cancellationToken)
        {
            if (variants == null || variants.Count == 0)
            {
                return null;
            }

            if (!rules.AllowProductVariants && variants.Count > 1)
            {
                return "Product variants are disabled in System Settings.";
            }

            var seenSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var variant in variants)
            {
                if ((variant.PriceDelta ?? 0) < 0)
                {
                    return "Variant price adjustment cannot be negative.";
                }

                if ((variant.StockDelta ?? 0) < 0)
                {
                    return "Variant stock quantity cannot be negative.";
                }

                var normalizedSku = NormalizeSku(variant.SKU);
                if (!string.IsNullOrWhiteSpace(normalizedSku) && !seenSkus.Add(normalizedSku))
                {
                    return $"Duplicate variant SKU '{normalizedSku}' in submitted product variants.";
                }

                var attributes = (variant.Attributes ?? [])
                    .Where(attribute => attribute.AttributeId > 0 || attribute.ValueId > 0)
                    .ToList();

                if (
                    rules.AttributeRequiredForVariants &&
                    attributes.Count == 0 &&
                    !string.Equals(variant.VariantName?.Trim(), "Default", StringComparison.OrdinalIgnoreCase))
                {
                    return "Variant attribute is required by System Settings.";
                }

                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeId <= 0 || attribute.ValueId <= 0)
                    {
                        return "Variant attributes must include both attribute and value.";
                    }

                    var valueMatchesAttribute = await _context.AttributeValues
                        .AsNoTracking()
                        .AnyAsync(
                            value =>
                                value.AttributeId == attribute.AttributeId &&
                                value.ValueId == attribute.ValueId,
                            cancellationToken);

                    if (!valueMatchesAttribute)
                    {
                        return $"Invalid value {attribute.ValueId} for attribute {attribute.AttributeId}.";
                    }
                }
            }

            return null;
        }

        private async Task<string?> ValidateRequiredProductReferences(
            int? categoryId,
            int? subCategoryId,
            int? brandId,
            int? unitId,
            int? supplierId,
            int? warehouseId,
            int? stock,
            ProductRuleSettings rules,
            CancellationToken cancellationToken)
        {
            if ((rules.CategoryRequired || rules.SubCategoryRequired) && (categoryId == null || categoryId <= 0))
            {
                return "Category is required.";
            }

            if (rules.BrandRequired && (brandId == null || brandId <= 0))
            {
                return "Brand is required.";
            }

            if (rules.SubCategoryRequired && (subCategoryId == null || subCategoryId <= 0))
            {
                return "SubCategory is required.";
            }

            if (unitId == null || unitId <= 0)
            {
                return "Unit is required.";
            }

            if ((stock ?? 0) > 0 && (warehouseId == null || warehouseId <= 0))
            {
                return "Warehouse is required when opening stock is added.";
            }

            return await ValidateExistingProductReferences(
                categoryId,
                subCategoryId,
                brandId,
                unitId,
                supplierId,
                warehouseId,
                cancellationToken);
        }

        private async Task<string?> ValidatePatchProductReferences(
            Product product,
            ISet<string> changedFields,
            ProductRuleSettings rules,
            CancellationToken cancellationToken)
        {
            if (changedFields.Count == 0)
            {
                return null;
            }

            if ((rules.CategoryRequired || rules.SubCategoryRequired) && (product.CategoryId == null || product.CategoryId <= 0))
            {
                return "Category is required.";
            }

            if (rules.BrandRequired && (product.BrandId == null || product.BrandId <= 0))
            {
                return "Brand is required.";
            }

            if (rules.SubCategoryRequired && (product.SubCategoryId == null || product.SubCategoryId <= 0))
            {
                return "SubCategory is required.";
            }

            if (product.UnitId == null || product.UnitId <= 0)
            {
                return "Unit is required.";
            }

            return await ValidateExistingProductReferences(
                product.CategoryId,
                product.SubCategoryId,
                product.BrandId,
                product.UnitId,
                product.SupplierId,
                product.WarehouseId,
                cancellationToken);
        }

        private async Task<string?> ValidateExistingProductReferences(
            int? categoryId,
            int? subCategoryId,
            int? brandId,
            int? unitId,
            int? supplierId,
            int? warehouseId,
            CancellationToken cancellationToken)
        {
            if (
                categoryId.HasValue &&
                !await _context.Categories
                    .AsNoTracking()
                    .AnyAsync(category => category.CategoryId == categoryId.Value && !category.IsDeleted, cancellationToken))
            {
                return "Selected category does not exist.";
            }

            if (
                brandId.HasValue &&
                !await _context.Brands
                    .AsNoTracking()
                    .AnyAsync(brand => brand.BrandId == brandId.Value && !brand.IsDeleted, cancellationToken))
            {
                return "Selected brand does not exist.";
            }

            if (
                unitId.HasValue &&
                !await _context.Units
                    .AsNoTracking()
                    .AnyAsync(unit => unit.UnitId == unitId.Value && !unit.IsDeleted, cancellationToken))
            {
                return "Selected unit does not exist.";
            }

            if (
                supplierId.HasValue &&
                supplierId.Value > 0 &&
                !await _context.Suppliers
                    .AsNoTracking()
                    .AnyAsync(supplier => supplier.SupplierId == supplierId.Value, cancellationToken))
            {
                return "Selected supplier does not exist.";
            }

            if (
                warehouseId.HasValue &&
                warehouseId.Value > 0 &&
                !await _context.Warehouses
                    .AsNoTracking()
                    .AnyAsync(warehouse => warehouse.WarehouseId == warehouseId.Value, cancellationToken))
            {
                return "Selected warehouse does not exist.";
            }

            if (subCategoryId.HasValue)
            {
                if (subCategoryId <= 0)
                {
                    return "Selected subcategory is invalid.";
                }

                var subCategory = await _context.SubCategories
                    .AsNoTracking()
                    .Where(item => item.SubCategoryId == subCategoryId.Value && !item.IsDeleted)
                    .Select(item => new { item.CategoryId })
                    .FirstOrDefaultAsync(cancellationToken);

                if (subCategory == null)
                {
                    return "Selected subcategory does not exist.";
                }

                if (categoryId.HasValue && subCategory.CategoryId != categoryId.Value)
                {
                    return "Selected subcategory does not belong to the selected category.";
                }
            }

            return null;
        }

        private async Task<string?> ValidateDuplicateProductName(
            string? name,
            int? excludingProductId,
            ProductRuleSettings rules,
            CancellationToken cancellationToken)
        {
            if (rules.DuplicateProductNameAllowed || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var normalizedName = name.Trim().ToLower();
            var exists = await _context.Products
                .AsNoTracking()
                .AnyAsync(
                    product =>
                        !product.IsDeleted &&
                        product.Name.ToLower() == normalizedName &&
                        (!excludingProductId.HasValue || product.ProductId != excludingProductId.Value),
                    cancellationToken);

            return exists ? "Product name already exists." : null;
        }

        private static bool IsBarcodeUniqueViolation(DbUpdateException exception)
        {
            var detail = $"{exception.Message} {exception.InnerException?.Message}"
                .ToLowerInvariant();

            return detail.Contains("barcode") &&
                (detail.Contains("duplicate") || detail.Contains("unique"));
        }

        private static string GetDbUpdateUserMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);
            var normalizedDetail = detail.ToLowerInvariant();

            if (normalizedDetail.Contains("barcode") &&
                (normalizedDetail.Contains("duplicate") || normalizedDetail.Contains("unique")))
            {
                return "Barcode already exists. Please regenerate barcode.";
            }

            if (normalizedDetail.Contains("foreign key") || normalizedDetail.Contains("constraint"))
            {
                return $"Product could not be saved because a selected reference is invalid. {detail}";
            }

            if (normalizedDetail.Contains("movement_type") ||
                normalizedDetail.Contains("data truncated"))
            {
                return $"Product could not be saved because the stock movement type is not allowed by the database column. {detail}";
            }

            return detail;
        }

        private static string GetProductDeleteDependencyMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);
            var normalizedDetail = detail.ToLowerInvariant();

            if (normalizedDetail.Contains("invoice_items"))
            {
                return "InvoiceItems records exist";
            }

            if (normalizedDetail.Contains("sales_order_items"))
            {
                return "SalesItems records exist";
            }

            if (normalizedDetail.Contains("goods_receipt"))
            {
                return "Goods receipt records exist";
            }

            if (normalizedDetail.Contains("purchase_order"))
            {
                return "Purchase order records exist";
            }

            if (normalizedDetail.Contains("stock_movements") || normalizedDetail.Contains("stock movement"))
            {
                return "Stock movement records exist";
            }

            if (normalizedDetail.Contains("stock_ledger") || normalizedDetail.Contains("stock ledger"))
            {
                return "Stock ledger records exist";
            }

            if (normalizedDetail.Contains("stock"))
            {
                return "Stock register records exist";
            }

            if (normalizedDetail.Contains("variant_attribute_values"))
            {
                return "Variant attribute records exist";
            }

            if (normalizedDetail.Contains("product_variants"))
            {
                return "Product variant records exist";
            }

            if (normalizedDetail.Contains("barcodes"))
            {
                return "Barcode records exist";
            }

            return "Product cannot be deleted because dependent records exist.";
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

        private sealed class ProductConflictException : Exception
        {
            public ProductConflictException(string message) : base(message)
            {
            }
        }






        // =========================
        // UPLOAD PRODUCT IMAGE
        // =========================
        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(
            int id,
            IFormFile file)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.ProductId == id && !x.IsDeleted);

            if (product == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Product not found",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "No file uploaded",
                    traceId: HttpContext.TraceIdentifier));
            }

            var extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Product image must be JPG, PNG, or WebP.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var fileName =
                Guid.NewGuid() + extension;

            var folderPath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "products");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath =
                Path.Combine(folderPath, fileName);

            using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            product.ImageUrl =
                $"/uploads/products/{fileName}";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Product image save failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(
                new { imageUrl = product.ImageUrl },
                "Image uploaded successfully",
                HttpContext.TraceIdentifier));
        }
    }
}
