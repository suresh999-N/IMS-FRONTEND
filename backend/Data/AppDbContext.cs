
using IMSBackend.Models;
using IMSBackend.Models.SystemSettings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IMSBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<PendingUser> PendingUsers { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductAttribute> Attributes { get; set; }
        public DbSet<VariantAttributeValue> VariantAttributeValues { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierContact> SupplierContacts { get; set; }
        public DbSet<SupplierAddress> SupplierAddresses { get; set; }
        public DbSet<SupplierPaymentTerm> SupplierPaymentTerms { get; set; }
        public DbSet<SupplierBankAccount> SupplierBankAccounts { get; set; }
        public DbSet<SupplierDocument> SupplierDocuments { get; set; }

        public DbSet<Rack> Racks { get; set; }
        public DbSet<Bin> Bins { get; set; }
        public DbSet<BinStock> BinStocks { get; set; }
        public DbSet<PutawayAudit> PutawayAudits { get; set; }
        public DbSet<BinTransferAudit> BinTransferAudits { get; set; }
        public DbSet<WarehouseTransferAudit> WarehouseTransferAudits { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockLedger> StockLedgers { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<StockAdjustmentItem> StockAdjustmentItems { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferItem> StockTransferItems { get; set; }
        public DbSet<StockAudit> StockAudits { get; set; }
        public DbSet<StockAuditItem> StockAuditItems { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<CustomerActivity> CustomerActivities { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerPaymentTerm> CustomerPaymentTerms { get; set; }
        public DbSet<CustomerBankDetail> CustomerBankDetails { get; set; }

        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }
        public DbSet<CustomerLedger> CustomerLedgers { get; set; }

       
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        // Purchase
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }

        public DbSet<SupplierPayment> SupplierPayments { get; set; }

        

        
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<SystemSettingSection> SystemSettingSections { get; set; }
        public DbSet<SystemSettingRule> SystemSettingRules { get; set; }

        public DbSet<PurchaseIndent> PurchaseIndents { get; set; }
        public DbSet<PurchaseIndentItem> PurchaseIndentItems { get; set; }

        public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public DbSet<PurchaseReturnItem> PurchaseReturnItems { get; set; }

        public DbSet<SalesReturn> SalesReturns { get; set; }
        public DbSet<SalesReturnItem> SalesReturnItems { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GoodsReceipt>()
              .HasIndex(g => g.GrnNumber)
              .IsUnique();


            // =========================================================
            // USER / AUTHENTICATION
            // =========================================================

            modelBuilder.Entity<UserToken>()
                .ToTable("user_tokens");

            modelBuilder.Entity<LoginHistory>()
                .ToTable("login_history");

            modelBuilder.Entity<LoginHistory>()
                .HasKey(x => x.LoginHistoryId);

            modelBuilder.Entity<LoginHistory>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================================
            // ROLE
            // =========================================================

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasKey(x => x.RoleId);

                entity.Property(x => x.RoleId)
                    .HasColumnName("role_id");

                entity.Property(x => x.RoleName)
                    .HasColumnName("role_name");
            });


            // =========================================================
            // MODULE
            // =========================================================

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("modules");

                entity.HasKey(x => x.ModuleId);

                entity.Property(x => x.ModuleId)
                    .HasColumnName("ModuleId");

                entity.Property(x => x.ModuleKey)
                    .HasColumnName("ModuleKey");

                entity.Property(x => x.ModuleName)
                    .HasColumnName("ModuleName");

                entity.Property(x => x.Category)
                    .HasColumnName("Category");

                entity.Property(x => x.Description)
                    .HasColumnName("Description");

                entity.Property(x => x.DisplayOrder)
                    .HasColumnName("DisplayOrder");

                entity.Property(x => x.IsActive)
                    .HasColumnName("IsActive");
            });


            // =========================================================
            // ROLE PERMISSION
            // =========================================================

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions");

                entity.HasKey(x => x.PermissionId);

                entity.HasOne(x => x.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Module)
                    .WithMany(m => m.RolePermissions)
                    .HasForeignKey(x => x.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================================================
            // USER & PENDING USER
            // =========================================================

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(user => user.Name).HasMaxLength(50);
                entity.Property(user => user.Email).HasMaxLength(254);
                entity.Property(user => user.PhoneNumber).HasMaxLength(10);
                entity.HasIndex(user => user.Email).IsUnique();
                entity.HasIndex(user => user.PhoneNumber).IsUnique();
            });

            modelBuilder.Entity<PendingUser>(entity =>
            {
                entity.Property(user => user.Name).HasMaxLength(50);
                entity.Property(user => user.Email).HasMaxLength(254);
                entity.Property(user => user.PhoneNumber).HasMaxLength(10);
            });


            // =========================================================
            // OTP
            // =========================================================

            modelBuilder.Entity<Otp>()
                .Property(otp => otp.Email)
                .HasMaxLength(256);

            modelBuilder.Entity<Otp>()
                .Property(otp => otp.Code)
                .HasMaxLength(12);

            modelBuilder.Entity<Otp>()
                .HasIndex(otp => new { otp.Email, otp.Code });


            // =========================================================
            // PRODUCT
            // =========================================================

            modelBuilder.Entity<Product>()
                .Property(product => product.SKU)
                .HasMaxLength(128);

            modelBuilder.Entity<Product>()
                .Property(product => product.Barcode)
                .HasMaxLength(128);

            modelBuilder.Entity<Product>()
                .Property(product => product.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(product => product.CostPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .HasIndex(product => product.SKU)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(product => product.Barcode)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(product => product.IsArchived);


            // =========================================================
            // CUSTOMER
            // =========================================================

            modelBuilder.Entity<Customer>()
                .Property(customer => customer.CustomerCode)
                .HasMaxLength(64);

            modelBuilder.Entity<Customer>()
                .HasIndex(customer => customer.CustomerCode)
                .IsUnique();


            // =========================================================
            // CATEGORY
            // =========================================================

            modelBuilder.Entity<Category>()
                .Property(category => category.Name)
                .HasMaxLength(160);

            modelBuilder.Entity<Category>()
                .Property(category => category.Status)
                .HasMaxLength(32)
                .HasDefaultValue("Active");

            modelBuilder.Entity<Category>()
                .HasMany(category => category.SubCategories)
                .WithOne(subCategory => subCategory.Category)
                .HasForeignKey(subCategory => subCategory.CategoryId);


            // =========================================================
            // SUB CATEGORY
            // =========================================================

            modelBuilder.Entity<SubCategory>()
                .Property(subCategory => subCategory.Name)
                .HasMaxLength(160);

            modelBuilder.Entity<SubCategory>()
                .Property(subCategory => subCategory.Status)
                .HasMaxLength(32);


            // =========================================================
            // PRODUCT VARIANT
            // =========================================================

            modelBuilder.Entity<ProductVariant>()
                .Property(variant => variant.SKU)
                .HasMaxLength(128);

            modelBuilder.Entity<ProductVariant>()
                .Property(variant => variant.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductVariant>()
                .Property(variant => variant.CostPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductVariant>()
                .HasIndex(variant => new { variant.ProductId, variant.SKU })
                .IsUnique();


            // =========================================================
            // BRAND
            // =========================================================

            modelBuilder.Entity<Brand>()
                .Property(brand => brand.Name)
                .HasMaxLength(160);

            modelBuilder.Entity<Brand>()
                .Property(brand => brand.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Brand>()
                .HasIndex(brand => brand.Name);


            // =========================================================
            // UNIT
            // =========================================================

            modelBuilder.Entity<Unit>()
                .Property(unit => unit.Name)
                .HasMaxLength(120);

            modelBuilder.Entity<Unit>()
                .Property(unit => unit.ShortName)
                .HasMaxLength(32);

            modelBuilder.Entity<Unit>()
                .HasIndex(unit => unit.Name);

            modelBuilder.Entity<Unit>()
                .HasIndex(unit => unit.ShortName);


            // =========================================================
            // VARIANT ATTRIBUTE
            // =========================================================

            modelBuilder.Entity<VariantAttributeValue>()
                .HasIndex(item => new
                {
                    item.VariantId,
                    item.AttributeId,
                    item.ValueId
                })
                .IsUnique();


            // =========================================================
            // STOCK
            // =========================================================

            modelBuilder.Entity<Stock>()
                .Property(stock => stock.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Stock>()
                .Property(stock => stock.ReservedQuantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Stock>()
                .Property(stock => stock.AvailableQuantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Stock>()
                .HasIndex(stock => new
                {
                    stock.ProductId,
                    stock.WarehouseId,
                    stock.VariantId
                })
                .IsUnique();


            // =========================================================
            // STOCK MOVEMENT
            // =========================================================

            modelBuilder.Entity<StockMovement>()
                .Property(movement => movement.MovementType)
                .HasMaxLength(20);

            modelBuilder.Entity<StockMovement>()
                .Property(movement => movement.ReferenceType)
                .HasMaxLength(40);

            modelBuilder.Entity<StockMovement>()
                .Property(movement => movement.Quantity)
                .HasPrecision(18, 2);


            // =========================================================
            // STOCK LEDGER
            // =========================================================

            modelBuilder.Entity<StockLedger>()
                .Property(ledger => ledger.OpeningQty)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockLedger>()
                .Property(ledger => ledger.ChangeQty)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockLedger>()
                .Property(ledger => ledger.ClosingQty)
                .HasPrecision(18, 2);


            // =========================================================
            // WAREHOUSE / PUTAWAY
            // =========================================================

            modelBuilder.Entity<PutawayAudit>()
                .Property(audit => audit.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PutawayAudit>()
                .Property(audit => audit.UserName)
                .HasMaxLength(256);

            modelBuilder.Entity<BinTransferAudit>()
                .Property(audit => audit.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BinTransferAudit>()
                .Property(audit => audit.UserName)
                .HasMaxLength(256);

            modelBuilder.Entity<WarehouseTransferAudit>()
                .Property(audit => audit.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<WarehouseTransferAudit>()
                .Property(audit => audit.UserName)
                .HasMaxLength(256);


            // =========================================================
            // INVOICE
            // =========================================================

            modelBuilder.Entity<Invoice>()
                .Property(invoice => invoice.Status)
                .HasMaxLength(32);

            modelBuilder.Entity<Invoice>()
                .Property(invoice => invoice.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(invoice => invoice.PaidAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(invoice => invoice.BalanceAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(item => item.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(item => item.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(item => item.Total)
                .HasPrecision(18, 2);

            // =========================================================
            // CUSTOMER FINANCIAL
            // =========================================================

            modelBuilder.Entity<Customer>()
                .Property(customer => customer.CreditLimit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Customer>()
                .Property(customer => customer.OutstandingBalance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerContact>()
                .HasIndex(contact => contact.CustomerId);

            modelBuilder.Entity<CustomerAddress>()
                .HasIndex(address => address.CustomerId);

            modelBuilder.Entity<CustomerPaymentTerm>()
                .Property(term => term.CreditLimit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerPaymentTerm>()
                .HasIndex(term => term.CustomerId);

            modelBuilder.Entity<CustomerBankDetail>()
                .HasIndex(bank => bank.CustomerId);

            modelBuilder.Entity<CustomerLedger>()
                .Property(ledger => ledger.Debit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerLedger>()
                .Property(ledger => ledger.Credit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerLedger>()
                .Property(ledger => ledger.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerPayment>()
                .Property(payment => payment.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerPayment>()
                .Property(payment => payment.ReferenceNumber)
                .HasMaxLength(100);

            modelBuilder.Entity<CustomerPayment>()
                .Property(payment => payment.Notes)
                .HasMaxLength(500);


            // =========================================================
            // SUPPLIER
            // =========================================================

            modelBuilder.Entity<Supplier>()
                .Property(supplier => supplier.Email)
                .HasMaxLength(256);

            modelBuilder.Entity<Supplier>()
                .HasIndex(supplier => supplier.Email);


            // =========================================================
            // WAREHOUSE
            // =========================================================

            modelBuilder.Entity<Warehouse>()
                .Property(warehouse => warehouse.Name)
                .HasMaxLength(160);

            modelBuilder.Entity<Warehouse>()
                .HasIndex(warehouse => warehouse.Name);


            // =========================================================
            // PURCHASE INDENT
            // =========================================================

            modelBuilder.Entity<PurchaseIndent>()
                .HasMany(x => x.Items)
                .WithOne(x => x.PurchaseIndent)
                .HasForeignKey(x => x.PurchaseIndentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseIndent>()
                .HasOne(x => x.RequestedByUser)
                .WithMany()
                .HasForeignKey(x => x.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseIndentItem>()
                .HasOne(x => x.Product)
                .WithMany(x => x.PurchaseIndentItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseIndentItem>()
                .HasOne(x => x.Unit)
                .WithMany(x => x.PurchaseIndentItems)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseIndent>()
                .HasIndex(x => x.IndentNumber)
                .IsUnique();

            modelBuilder.Entity<PurchaseIndent>()
                .Property(x => x.TotalQuantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseIndentItem>()
                .Property(x => x.RequiredQty)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseIndentItem>()
                .Property(x => x.AvailableStock)
                .HasPrecision(18, 2);

            // =========================================================
            // PURCHASE RETURN
            // =========================================================

            modelBuilder.Entity<PurchaseReturn>()
                .HasMany(x => x.Items)
                .WithOne(x => x.PurchaseReturn)
                .HasForeignKey(x => x.PurchaseReturnId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseReturn>()
                .HasIndex(x => x.ReturnNumber)
                .IsUnique();

            modelBuilder.Entity<PurchaseReturn>()
                .Property(x => x.TotalReturnAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseReturnItem>()
                .Property(x => x.ReceivedQuantity)
                .HasPrecision(18, 3);

            modelBuilder.Entity<PurchaseReturnItem>()
                .Property(x => x.ReturnQuantity)
                .HasPrecision(18, 3);

            modelBuilder.Entity<PurchaseReturnItem>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseReturnItem>()
                .Property(x => x.Total)
                .HasPrecision(18, 2);

            // =========================================================
            // SALES RETURN
            // =========================================================

            modelBuilder.Entity<SalesReturn>()
                .HasMany(x => x.Items)
                .WithOne(x => x.SalesReturn)
                .HasForeignKey(x => x.SalesReturnId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesReturn>()
                .HasIndex(x => x.ReturnNumber)
                .IsUnique();

            modelBuilder.Entity<SalesReturn>()
                .Property(x => x.GrandTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesReturnItem>()
                .Property(x => x.InvoicedQuantity)
                .HasPrecision(18, 3);

            modelBuilder.Entity<SalesReturnItem>()
                .Property(x => x.ReturnQuantity)
                .HasPrecision(18, 3);

            modelBuilder.Entity<SalesReturnItem>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesReturnItem>()
                .Property(x => x.Total)
                .HasPrecision(18, 2);
        }
    }
}

