using IMSBackend.Data;
using IMSBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private const int ModuleResultLimit = 8;
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("global")]
        public async Task<ActionResult<IEnumerable<SearchResultDto>>> GlobalSearch(
            [FromQuery] string? query,
            CancellationToken cancellationToken)
        {
            var searchText = query?.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return Ok(Array.Empty<SearchResultDto>());
            }

            var likePattern = $"%{EscapeLikePattern(searchText)}%";
            var results = new List<SearchResultDto>();

            results.AddRange(await SearchProducts(likePattern, cancellationToken));
            results.AddRange(await SearchCustomers(likePattern, cancellationToken));
            results.AddRange(await SearchSuppliers(likePattern, cancellationToken));
            results.AddRange(await SearchUnits(likePattern, cancellationToken));
            results.AddRange(await SearchBrands(likePattern, cancellationToken));
            results.AddRange(await SearchCategories(likePattern, cancellationToken));
            results.AddRange(await SearchSubCategories(likePattern, cancellationToken));
            results.AddRange(await SearchInvoices(likePattern, cancellationToken));
            results.AddRange(await SearchInvoiceItems(likePattern, cancellationToken));
            results.AddRange(await SearchPurchaseOrders(likePattern, cancellationToken));
            results.AddRange(await SearchPurchaseOrderItems(likePattern, cancellationToken));
            results.AddRange(await SearchGoodsReceipts(likePattern, cancellationToken));
            results.AddRange(await SearchGoodsReceiptItems(likePattern, cancellationToken));
            results.AddRange(await SearchStockTransactions(likePattern, cancellationToken));
            results.AddRange(await SearchWarehouses(likePattern, cancellationToken));
            results.AddRange(await SearchSupplierPayments(likePattern, cancellationToken));
            results.AddRange(await SearchCustomerPayments(likePattern, cancellationToken));
            results.AddRange(await SearchUsers(likePattern, cancellationToken));
            results.AddRange(await SearchRoles(likePattern, cancellationToken));

            return Ok(results);
        }

        private Task<List<SearchResultDto>> SearchProducts(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Products
                .AsNoTracking()
                .Where(product =>
                    !product.IsDeleted &&
                    (
                        EF.Functions.Like(product.Name, likePattern, "\\") ||
                        EF.Functions.Like(product.SKU, likePattern, "\\") ||
                        EF.Functions.Like(product.Barcode, likePattern, "\\")
                    ))
                .OrderBy(product => product.Name)
                .Take(ModuleResultLimit)
                .Select(product => new SearchResultDto
                {
                    Type = "Product",
                    Id = product.ProductId,
                    Title = product.Name,
                    Subtitle = product.SKU ?? product.Barcode,
                    Route = $"/inventory/products/{product.ProductId}",
                    Icon = "package"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchCategories(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Categories
                .AsNoTracking()
                .Where(category =>
                    !category.IsDeleted &&
                    EF.Functions.Like(category.Name, likePattern, "\\"))
                .OrderBy(category => category.Name)
                .Take(ModuleResultLimit)
                .Select(category => new SearchResultDto
                {
                    Type = "Category",
                    Id = category.CategoryId,
                    Title = category.Name,
                    Subtitle = "Inventory category",
                    Route = "/inventory/categories",
                    Icon = "folder"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchSubCategories(string likePattern, CancellationToken cancellationToken)
        {
            return _context.SubCategories
                .AsNoTracking()
                .Where(subCategory =>
                    !subCategory.IsDeleted &&
                    EF.Functions.Like(subCategory.Name, likePattern, "\\"))
                .OrderBy(subCategory => subCategory.Name)
                .Take(ModuleResultLimit)
                .Select(subCategory => new SearchResultDto
                {
                    Type = "SubCategory",
                    Id = subCategory.SubCategoryId,
                    Title = subCategory.Name,
                    Subtitle = subCategory.Category != null ? subCategory.Category.Name : "Sub category",
                    Route = "/inventory/subcategories",
                    Icon = "layers"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchBrands(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Brands
                .AsNoTracking()
                .Where(brand =>
                    !brand.IsDeleted &&
                    EF.Functions.Like(brand.Name, likePattern, "\\"))
                .OrderBy(brand => brand.Name)
                .Take(ModuleResultLimit)
                .Select(brand => new SearchResultDto
                {
                    Type = "Brand",
                    Id = brand.BrandId,
                    Title = brand.Name,
                    Subtitle = "Product brand",
                    Route = "/inventory/brands",
                    Icon = "tag"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchCustomers(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Customers
                .AsNoTracking()
                .Where(customer =>
                    EF.Functions.Like(customer.Name, likePattern, "\\") ||
                    (customer.Email != null && EF.Functions.Like(customer.Email, likePattern, "\\")) ||
                    (customer.Phone != null && EF.Functions.Like(customer.Phone, likePattern, "\\")))
                .OrderBy(customer => customer.Name)
                .Take(ModuleResultLimit)
                .Select(customer => new SearchResultDto
                {
                    Type = "Customer",
                    Id = customer.CustomerId,
                    Title = customer.Name,
                    Subtitle = customer.Phone ?? customer.Email,
                    Route = $"/people/customers/{customer.CustomerId}",
                    Icon = "user"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchSuppliers(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Suppliers
                .AsNoTracking()
                .Where(supplier =>
                    !supplier.IsDeleted &&
                    (
                        (supplier.Name != null && EF.Functions.Like(supplier.Name, likePattern, "\\")) ||
                        (supplier.SupplierCode != null && EF.Functions.Like(supplier.SupplierCode, likePattern, "\\")) ||
                        (supplier.Category != null && EF.Functions.Like(supplier.Category, likePattern, "\\")) ||
                        (supplier.GstNumber != null && EF.Functions.Like(supplier.GstNumber, likePattern, "\\")) ||
                        (supplier.PanNumber != null && EF.Functions.Like(supplier.PanNumber, likePattern, "\\")) ||
                        (supplier.Email != null && EF.Functions.Like(supplier.Email, likePattern, "\\")) ||
                        (supplier.Phone != null && EF.Functions.Like(supplier.Phone, likePattern, "\\"))
                    ))
                .OrderBy(supplier => supplier.Name)
                .Take(ModuleResultLimit)
                .Select(supplier => new SearchResultDto
                {
                    Type = "Supplier",
                    Id = supplier.SupplierId,
                    Title = supplier.Name ?? "Supplier",
                    Subtitle = supplier.SupplierCode ?? supplier.Phone ?? supplier.Email ?? supplier.Status,
                    Route = $"/people/suppliers/{supplier.SupplierId}",
                    Icon = "building"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchUnits(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Units
                .AsNoTracking()
                .Where(unit =>
                    !unit.IsDeleted &&
                    (
                        EF.Functions.Like(unit.Name, likePattern, "\\") ||
                        EF.Functions.Like(unit.ShortName, likePattern, "\\")
                    ))
                .OrderBy(unit => unit.Name)
                .Take(ModuleResultLimit)
                .Select(unit => new SearchResultDto
                {
                    Type = "Unit",
                    Id = unit.UnitId,
                    Title = unit.Name,
                    Subtitle = unit.ShortName,
                    Route = "/inventory/units",
                    Icon = "ruler"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchInvoices(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from invoice in _context.Invoices.AsNoTracking()
                    join customer in _context.Customers.AsNoTracking()
                        on invoice.CustomerId equals customer.CustomerId into customerGroup
                    from customer in customerGroup.DefaultIfEmpty()
                    where !invoice.IsCancelled &&
                          (
                              (invoice.InvoiceNumber != null && EF.Functions.Like(invoice.InvoiceNumber, likePattern, "\\")) ||
                              (invoice.Status != null && EF.Functions.Like(invoice.Status, likePattern, "\\")) ||
                              (customer != null && EF.Functions.Like(customer.Name, likePattern, "\\"))
                          )
                    orderby invoice.InvoiceNumber
                    select new SearchResultDto
                    {
                        Type = "Invoice",
                        Id = invoice.InvoiceId,
                        Title = invoice.InvoiceNumber ?? $"Invoice #{invoice.InvoiceId}",
                        Subtitle = customer != null ? customer.Name : invoice.Status,
                        Route = $"/accounting/{invoice.InvoiceId}",
                        Icon = "receipt"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchInvoiceItems(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from item in _context.InvoiceItems.AsNoTracking()
                    join invoice in _context.Invoices.AsNoTracking()
                        on item.InvoiceId equals invoice.InvoiceId
                    join product in _context.Products.AsNoTracking()
                        on item.ProductId equals product.ProductId into productGroup
                    from product in productGroup.DefaultIfEmpty()
                    where !invoice.IsCancelled &&
                          product != null &&
                          !product.IsDeleted &&
                          (
                              EF.Functions.Like(product.Name, likePattern, "\\") ||
                              EF.Functions.Like(product.SKU, likePattern, "\\") ||
                              EF.Functions.Like(product.Barcode, likePattern, "\\") ||
                              (invoice.InvoiceNumber != null && EF.Functions.Like(invoice.InvoiceNumber, likePattern, "\\"))
                          )
                    orderby product.Name
                    select new SearchResultDto
                    {
                        Type = "InvoiceItem",
                        Id = item.Id,
                        Title = product.Name,
                        Subtitle = invoice.InvoiceNumber,
                        Route = $"/accounting/{invoice.InvoiceId}",
                        Icon = "receipt"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchPurchaseOrders(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from purchaseOrder in _context.PurchaseOrders.AsNoTracking()
                    join supplier in _context.Suppliers.AsNoTracking()
                        on purchaseOrder.SupplierId equals supplier.SupplierId into supplierGroup
                    from supplier in supplierGroup.DefaultIfEmpty()
                    where !purchaseOrder.IsCancelled &&
                          (
                              (purchaseOrder.PoNumber != null && EF.Functions.Like(purchaseOrder.PoNumber, likePattern, "\\")) ||
                              (purchaseOrder.Status != null && EF.Functions.Like(purchaseOrder.Status, likePattern, "\\")) ||
                              (supplier != null && !supplier.IsDeleted && supplier.Name != null && EF.Functions.Like(supplier.Name, likePattern, "\\"))
                          )
                    orderby purchaseOrder.PoNumber
                    select new SearchResultDto
                    {
                        Type = "PurchaseOrder",
                        Id = purchaseOrder.PoId,
                        Title = purchaseOrder.PoNumber ?? $"Purchase Order #{purchaseOrder.PoId}",
                        Subtitle = supplier != null ? supplier.Name : purchaseOrder.Status,
                        Route = $"/inventory/purchases/{purchaseOrder.PoId}",
                        Icon = "file"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchPurchaseOrderItems(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from item in _context.PurchaseOrderItems.AsNoTracking()
                    join purchaseOrder in _context.PurchaseOrders.AsNoTracking()
                        on item.PoId equals purchaseOrder.PoId
                    join product in _context.Products.AsNoTracking()
                        on item.ProductId equals product.ProductId into productGroup
                    from product in productGroup.DefaultIfEmpty()
                    where !purchaseOrder.IsCancelled &&
                          product != null &&
                          !product.IsDeleted &&
                          (
                              EF.Functions.Like(product.Name, likePattern, "\\") ||
                              EF.Functions.Like(product.SKU, likePattern, "\\") ||
                              EF.Functions.Like(product.Barcode, likePattern, "\\") ||
                              (purchaseOrder.PoNumber != null && EF.Functions.Like(purchaseOrder.PoNumber, likePattern, "\\"))
                          )
                    orderby product.Name
                    select new SearchResultDto
                    {
                        Type = "PurchaseOrderItem",
                        Id = item.Id,
                        Title = product.Name,
                        Subtitle = purchaseOrder.PoNumber,
                        Route = $"/inventory/purchases/{purchaseOrder.PoId}",
                        Icon = "file"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchGoodsReceipts(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from receipt in _context.GoodsReceipts.AsNoTracking()
                    join supplier in _context.Suppliers.AsNoTracking()
                        on receipt.SupplierId equals supplier.SupplierId into supplierGroup
                    from supplier in supplierGroup.DefaultIfEmpty()
                    join warehouse in _context.Warehouses.AsNoTracking()
                        on receipt.WarehouseId equals warehouse.WarehouseId into warehouseGroup
                    from warehouse in warehouseGroup.DefaultIfEmpty()
                    where !receipt.IsCancelled &&
                          (
                              (receipt.Status != null && EF.Functions.Like(receipt.Status, likePattern, "\\")) ||
                              (receipt.Notes != null && EF.Functions.Like(receipt.Notes, likePattern, "\\")) ||
                              (supplier != null && !supplier.IsDeleted && supplier.Name != null && EF.Functions.Like(supplier.Name, likePattern, "\\")) ||
                              (warehouse != null && EF.Functions.Like(warehouse.Name, likePattern, "\\"))
                          )
                    orderby receipt.GrnId descending
                    select new SearchResultDto
                    {
                        Type = "GoodsReceipt",
                        Id = receipt.GrnId,
                        Title = $"Goods Receipt #{receipt.GrnId}",
                        Subtitle = supplier != null ? supplier.Name : receipt.Status,
                        Route = "/inventory/goods-receipts",
                        Icon = "truck"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchGoodsReceiptItems(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from item in _context.GoodsReceiptItems.AsNoTracking()
                    join receipt in _context.GoodsReceipts.AsNoTracking()
                        on item.GrnId equals receipt.GrnId
                    join product in _context.Products.AsNoTracking()
                        on item.ProductId equals product.ProductId into productGroup
                    from product in productGroup.DefaultIfEmpty()
                    where !receipt.IsCancelled &&
                          product != null &&
                          !product.IsDeleted &&
                          (
                              EF.Functions.Like(product.Name, likePattern, "\\") ||
                              EF.Functions.Like(product.SKU, likePattern, "\\") ||
                              EF.Functions.Like(product.Barcode, likePattern, "\\")
                          )
                    orderby product.Name
                    select new SearchResultDto
                    {
                        Type = "GoodsReceiptItem",
                        Id = item.Id,
                        Title = product.Name,
                        Subtitle = $"Goods Receipt #{receipt.GrnId}",
                        Route = "/inventory/goods-receipts",
                        Icon = "truck"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchStockTransactions(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from movement in _context.StockMovements.AsNoTracking()
                    join product in _context.Products.AsNoTracking()
                        on movement.ProductId equals product.ProductId into productGroup
                    from product in productGroup.DefaultIfEmpty()
                    join warehouse in _context.Warehouses.AsNoTracking()
                        on movement.WarehouseId equals warehouse.WarehouseId into warehouseGroup
                    from warehouse in warehouseGroup.DefaultIfEmpty()
                    where product != null &&
                          !product.IsDeleted &&
                          (
                              (movement.MovementType != null && EF.Functions.Like(movement.MovementType, likePattern, "\\")) ||
                              (movement.ReferenceType != null && EF.Functions.Like(movement.ReferenceType, likePattern, "\\")) ||
                              (movement.Notes != null && EF.Functions.Like(movement.Notes, likePattern, "\\")) ||
                              EF.Functions.Like(product.Name, likePattern, "\\") ||
                              EF.Functions.Like(product.SKU, likePattern, "\\") ||
                              (warehouse != null && EF.Functions.Like(warehouse.Name, likePattern, "\\"))
                          )
                    orderby movement.CreatedAt descending
                    select new SearchResultDto
                    {
                        Type = "StockTransaction",
                        Id = movement.MovementId,
                        Title = product.Name,
                        Subtitle = movement.MovementType ?? movement.ReferenceType,
                        Route = "/inventory/stock",
                        Icon = "activity"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchWarehouses(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Warehouses
                .AsNoTracking()
                .Where(warehouse =>
                    EF.Functions.Like(warehouse.Name, likePattern, "\\") ||
                    EF.Functions.Like(warehouse.Location, likePattern, "\\") ||
                    EF.Functions.Like(warehouse.Status, likePattern, "\\"))
                .OrderBy(warehouse => warehouse.Name)
                .Take(ModuleResultLimit)
                .Select(warehouse => new SearchResultDto
                {
                    Type = "Warehouse",
                    Id = warehouse.WarehouseId,
                    Title = warehouse.Name,
                    Subtitle = warehouse.Location,
                    Route = "/warehouses",
                    Icon = "warehouse"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchSupplierPayments(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from payment in _context.SupplierPayments.AsNoTracking()
                    join supplier in _context.Suppliers.AsNoTracking()
                        on payment.SupplierId equals supplier.SupplierId into supplierGroup
                    from supplier in supplierGroup.DefaultIfEmpty()
                    where !payment.IsCancelled &&
                          (
                              (payment.ReferenceNumber != null && EF.Functions.Like(payment.ReferenceNumber, likePattern, "\\")) ||
                              (payment.PaymentMethod != null && EF.Functions.Like(payment.PaymentMethod, likePattern, "\\")) ||
                              (payment.Notes != null && EF.Functions.Like(payment.Notes, likePattern, "\\")) ||
                              (supplier != null && !supplier.IsDeleted && supplier.Name != null && EF.Functions.Like(supplier.Name, likePattern, "\\"))
                          )
                    orderby payment.PaymentDate descending
                    select new SearchResultDto
                    {
                        Type = "SupplierPayment",
                        Id = payment.PaymentId,
                        Title = payment.ReferenceNumber ?? $"Supplier Payment #{payment.PaymentId}",
                        Subtitle = supplier != null ? supplier.Name : payment.PaymentMethod,
                        Route = "/people/supplier-payments",
                        Icon = "payment"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchCustomerPayments(string likePattern, CancellationToken cancellationToken)
        {
            return (
                    from payment in _context.CustomerPayments.AsNoTracking()
                    join customer in _context.Customers.AsNoTracking()
                        on payment.CustomerId equals customer.CustomerId into customerGroup
                    from customer in customerGroup.DefaultIfEmpty()
                    join invoice in _context.Invoices.AsNoTracking()
                        on payment.InvoiceId equals invoice.InvoiceId into invoiceGroup
                    from invoice in invoiceGroup.DefaultIfEmpty()
                    where !payment.IsCancelled &&
                          (
                              (payment.ReferenceNumber != null && EF.Functions.Like(payment.ReferenceNumber, likePattern, "\\")) ||
                              (payment.PaymentMethod != null && EF.Functions.Like(payment.PaymentMethod, likePattern, "\\")) ||
                              (payment.Notes != null && EF.Functions.Like(payment.Notes, likePattern, "\\")) ||
                              (customer != null && EF.Functions.Like(customer.Name, likePattern, "\\")) ||
                              (invoice != null && invoice.InvoiceNumber != null && EF.Functions.Like(invoice.InvoiceNumber, likePattern, "\\"))
                          )
                    orderby payment.PaymentDate descending
                    select new SearchResultDto
                    {
                        Type = "CustomerPayment",
                        Id = payment.PaymentId,
                        Title = payment.ReferenceNumber ?? $"Customer Payment #{payment.PaymentId}",
                        Subtitle = customer != null ? customer.Name : payment.PaymentMethod,
                        Route = "/people/customer-payments",
                        Icon = "payment"
                    })
                .Take(ModuleResultLimit)
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchUsers(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Users
                .AsNoTracking()
                .Where(user =>
                    user.IsActive &&
                    (
                        EF.Functions.Like(user.Name, likePattern, "\\") ||
                        EF.Functions.Like(user.Email, likePattern, "\\") ||
                        EF.Functions.Like(user.Role, likePattern, "\\")
                    ))
                .OrderBy(user => user.Name)
                .Take(ModuleResultLimit)
                .Select(user => new SearchResultDto
                {
                    Type = "User",
                    Id = user.Id,
                    Title = user.Name,
                    Subtitle = user.Email,
                    Route = "/users",
                    Icon = "user"
                })
                .ToListAsync();
        }

        private Task<List<SearchResultDto>> SearchRoles(string likePattern, CancellationToken cancellationToken)
        {
            return _context.Roles
                .AsNoTracking()
                .Where(role =>
                    EF.Functions.Like(role.RoleName, likePattern, "\\") ||
                    (role.Description != null && EF.Functions.Like(role.Description, likePattern, "\\")))
                .OrderBy(role => role.RoleName)
                .Take(ModuleResultLimit)
                .Select(role => new SearchResultDto
                {
                    Type = "Role",
                    Id = role.RoleId,
                    Title = role.RoleName,
                    Subtitle = role.Description,
                    Route = "/roles",
                    Icon = "shield"
                })
                .ToListAsync();
        }

        private static string EscapeLikePattern(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_")
                .Replace("[", "\\[");
        }
    }
}
