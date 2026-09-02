using IMSBackend.Data;
using IMSBackend.DTOs.Suppliers;
using IMSBackend.Infrastructure;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<SuppliersController> _logger;
        private readonly IWebHostEnvironment _environment;
        private const long SupplierDocumentMaxFileSize = 10 * 1024 * 1024;
        private const long SupplierDocumentMultipartBodyLimit = 12 * 1024 * 1024;
        private static readonly HashSet<string> SupplierDocumentAllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png"
        };

        private sealed class SupplierDocumentPromotionResult
        {
            public List<(string TemporaryPath, string PermanentPath)> MovedFiles { get; } = [];

            public List<string> ReplacedPermanentFiles { get; } = [];
        }

        public SuppliersController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<SuppliersController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
            _environment = environment;
        }

        private static string TrimToMax(string? value, int maxLength)
        {
            var normalizedValue = string.IsNullOrWhiteSpace(value)
                ? "supplier-document"
                : value.Trim();

            return normalizedValue.Length <= maxLength
                ? normalizedValue
                : normalizedValue[..maxLength];
        }

        private static void VerifySupplierDocumentUploadFolder(string uploadFolder)
        {
            var probePath = Path.Combine(uploadFolder, $".write-check-{Guid.NewGuid():N}.tmp");
            System.IO.File.WriteAllText(probePath, "ok");
            System.IO.File.Delete(probePath);
        }

        private static void TryDeleteSupplierDocumentFile(string? fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath) || !System.IO.File.Exists(fullFilePath))
            {
                return;
            }

            try
            {
                System.IO.File.Delete(fullFilePath);
            }
            catch
            {
                // Best-effort cleanup only. The original upload error is logged by the caller.
            }
        }

        private static string GetSupplierDocumentFullPath(string? filePath)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                string.IsNullOrWhiteSpace(filePath) ? string.Empty : filePath.TrimStart('/'));
        }

        private static string GetSupplierDocumentUploadFolder(bool isTemporary)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                isTemporary ? Path.Combine("temp", "suppliers") : "suppliers");
        }

        private static string GetSupplierDocumentRelativePath(string storedFileName, bool isTemporary)
        {
            return isTemporary
                ? $"/uploads/temp/suppliers/{storedFileName}"
                : $"/uploads/suppliers/{storedFileName}";
        }

        private static void TryRestorePromotedSupplierDocumentFiles(SupplierDocumentPromotionResult promotion)
        {
            foreach (var movedFile in promotion.MovedFiles)
            {
                try
                {
                    if (!System.IO.File.Exists(movedFile.PermanentPath))
                    {
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(movedFile.TemporaryPath)!);

                    if (System.IO.File.Exists(movedFile.TemporaryPath))
                    {
                        System.IO.File.Delete(movedFile.TemporaryPath);
                    }

                    System.IO.File.Move(movedFile.PermanentPath, movedFile.TemporaryPath);
                }
                catch
                {
                    // Best-effort rollback. The database transaction is rolled back by the caller.
                }
            }
        }

        private void DeletePromotedSupplierDocumentReplacements(SupplierDocumentPromotionResult promotion)
        {
            foreach (var fullPath in promotion.ReplacedPermanentFiles)
            {
                TryDeleteSupplierDocumentFile(fullPath);
            }
        }

        private async Task<SupplierDocumentPromotionResult> PromotePendingSupplierDocumentsAsync(int supplierId)
        {
            var promotion = new SupplierDocumentPromotionResult();
            var now = DateTime.UtcNow;
            var pendingDocuments = await _context.SupplierDocuments
                .Where(x =>
                    x.SupplierId == supplierId &&
                    x.IsTemporary &&
                    !x.IsDeleted)
                .OrderBy(x => x.UploadedAt)
                .ToListAsync();

            if (pendingDocuments.Count == 0)
            {
                return promotion;
            }

            var permanentFolder = GetSupplierDocumentUploadFolder(false);
            Directory.CreateDirectory(permanentFolder);
            VerifySupplierDocumentUploadFolder(permanentFolder);

            foreach (var document in pendingDocuments)
            {
                var temporaryPath = GetSupplierDocumentFullPath(document.FilePath);
                var permanentPath = Path.Combine(permanentFolder, document.StoredFileName);

                if (!System.IO.File.Exists(temporaryPath))
                {
                    throw new FileNotFoundException("Staged supplier document file was not found.", temporaryPath);
                }

                if (System.IO.File.Exists(permanentPath))
                {
                    permanentPath = Path.Combine(
                        permanentFolder,
                        $"{Guid.NewGuid():N}{Path.GetExtension(document.StoredFileName)}");
                    document.StoredFileName = Path.GetFileName(permanentPath);
                }

                System.IO.File.Move(temporaryPath, permanentPath);
                promotion.MovedFiles.Add((temporaryPath, permanentPath));

                if (document.DocumentType == SupplierDocumentTypes.Gst ||
                    document.DocumentType == SupplierDocumentTypes.Pan ||
                    document.DocumentType == SupplierDocumentTypes.Agreement)
                {
                    var replacedDocuments = await _context.SupplierDocuments
                        .Where(x =>
                            x.SupplierId == supplierId &&
                            x.DocumentType == document.DocumentType &&
                            !x.IsTemporary &&
                            !x.IsDeleted)
                        .ToListAsync();

                    foreach (var replacedDocument in replacedDocuments)
                    {
                        replacedDocument.IsDeleted = true;
                        replacedDocument.DeletedAt = now;
                        replacedDocument.Status = "replaced";
                        promotion.ReplacedPermanentFiles.Add(GetSupplierDocumentFullPath(replacedDocument.FilePath));
                    }
                }

                document.FilePath = GetSupplierDocumentRelativePath(document.StoredFileName, false);
                document.Status = "uploaded";
                document.IsTemporary = false;
                document.UploadedAt = now;

                _logger.LogInformation(
                    "Supplier staged document promoted. SupplierId={SupplierId}, DocumentId={DocumentId}, DocumentType={DocumentType}, PermanentPath={PermanentPath}, TraceId={TraceId}",
                    supplierId,
                    document.DocumentId,
                    document.DocumentType,
                    permanentPath,
                    HttpContext.TraceIdentifier);
            }

            return promotion;
        }

        private async Task CleanupSupplierTempDocumentsAsync(int supplierId)
        {
            var tempDocuments = await _context.SupplierDocuments
                .Where(x =>
                    x.SupplierId == supplierId &&
                    x.IsTemporary)
                .ToListAsync();

            foreach (var document in tempDocuments)
            {
                TryDeleteSupplierDocumentFile(GetSupplierDocumentFullPath(document.FilePath));
            }

            if (tempDocuments.Count > 0)
            {
                _context.SupplierDocuments.RemoveRange(tempDocuments);
                await _context.SaveChangesAsync();
            }
        }

        private ObjectResult SupplierDocumentUploadError(
            int statusCode,
            string message,
            Exception? exception = null)
        {
            if (_environment.IsDevelopment() && exception != null)
            {
                return StatusCode(statusCode, new
                {
                    message,
                    detail = exception.Message,
                    innerDetail = exception.InnerException?.Message,
                    traceId = HttpContext.TraceIdentifier
                });
            }

            return StatusCode(statusCode, new
            {
                message,
                traceId = HttpContext.TraceIdentifier
            });
        }

        // =========================
        // 🔹 GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetSuppliers(
    int page = 1,
    int pageSize = 500,
    string? search = null,
    string sortBy = "createdAt",
    string sortOrder = "desc",
    string? status = null,
    bool includeDeleted = false,
    bool includeArchived = false)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 500);

            var query = _context.Suppliers
                .AsNoTracking()
                .AsQueryable();

            var isArchivedStatus = string.Equals(status?.Trim(), "archived", StringComparison.OrdinalIgnoreCase);

            if (isArchivedStatus)
            {
                query = query.Where(x => x.IsDeleted);
            }
            else if (!includeDeleted && !includeArchived)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            // ================= SEARCH =================
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
    (x.Name != null && x.Name.ToLower().Contains(search)) ||
    (x.SupplierCode != null && x.SupplierCode.ToLower().Contains(search)) ||
    (x.Category != null && x.Category.ToLower().Contains(search)) ||
    (x.GstNumber != null && x.GstNumber.ToLower().Contains(search)) ||
    (x.PanNumber != null && x.PanNumber.ToLower().Contains(search)) ||
    (x.Email != null && x.Email.ToLower().Contains(search)) ||
    (x.Phone != null && x.Phone.ToLower().Contains(search)));
            }

            // ================= STATUS FILTER =================
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!isArchivedStatus)
                {
                    var normalizedStatus = SupplierStatusNormalizer.Normalize(status);
                    if (normalizedStatus == null)
                    {
                        return BadRequest(new
                        {
                            message = "Invalid supplier status."
                        });
                    }

                    query = query.Where(x => (x.Status ?? "active").ToLower() == normalizedStatus);
                }
            }


// ================= TOTAL COUNT =================
var totalRecords = await query.CountAsync();

            // ================= SORTING =================
            query = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("name", "asc") => query.OrderBy(x => x.Name),
                ("name", "desc") => query.OrderByDescending(x => x.Name),

                ("email", "asc") => query.OrderBy(x => x.Email),
                ("email", "desc") => query.OrderByDescending(x => x.Email),

                ("category", "asc") => query.OrderBy(x => x.Category),
                ("category", "desc") => query.OrderByDescending(x => x.Category),

                ("createdat", "asc") => query.OrderBy(x => x.CreatedAt),
                ("createdat", "desc") => query.OrderByDescending(x => x.CreatedAt),

                _ => query.OrderByDescending(x => x.CreatedAt)
            };



            // ================= PAGINATION =================
            var supplierData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.SupplierId,
                    x.SupplierCode,
                    x.Name,
                    x.Category,
                    x.GstNumber,
                    x.PanNumber,
                    x.Phone,
                    x.Email,
                    x.Website,
                    x.Status,
                    x.CreatedAt,
                    x.IsDeleted,
                    x.DeletedAt,

                    Purchases = _context.PurchaseOrders
                        .Where(po =>
                            po.SupplierId == x.SupplierId &&
                            !po.IsCancelled)
                        .Sum(po => (decimal?)po.TotalAmount) ?? 0m,

                    PaidAmount = _context.SupplierPayments
                        .Where(payment =>
                            payment.SupplierId == x.SupplierId &&
                            !payment.IsCancelled)
                        .Sum(payment => (decimal?)payment.Amount) ?? 0m
                })
                .ToListAsync();

            var suppliers = supplierData
                .Select(x => new
                {
                    x.SupplierId,
                    x.SupplierCode,
                    x.Name,
                    x.Category,
                    x.GstNumber,
                    x.PanNumber,
                    x.Phone,
                    x.Email,
                    x.Website,
                    x.Status,
                    x.CreatedAt,
                    x.IsDeleted,
                    x.DeletedAt,

                    Purchases = x.Purchases,
                    Outstanding = x.Purchases - x.PaidAmount
                })
                .ToList();

            // ================= RESPONSE =================
            return Ok(new
            {
                page,
                pageSize,
                totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                data = suppliers
            });
        }

        
        // =========================
        // 🔹 GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _context.Suppliers
                .Where(x => x.SupplierId == id && !x.IsDeleted)
                .Select(x => new
                {
                    x.SupplierId,
                    x.SupplierCode,
                    x.Name,
                    x.Category,
                    x.GstNumber,
                    x.PanNumber,
                    x.Phone,
                    x.Email,
                    x.Website,
                    x.Status,
                    x.CreatedAt,
                    x.UpdatedAt,

                    Contacts = _context.SupplierContacts
                        .Where(c => c.SupplierId == x.SupplierId)
                        .Select(c => new
                        {
                            c.ContactId,
                            c.Name,
                            c.Designation,
                            c.Department,
                            c.Phone,
                            c.Email,
                            c.IsPrimary
                        })
                        .ToList(),

                    Addresses = _context.SupplierAddresses
                        .Where(a => a.SupplierId == x.SupplierId)
                        .Select(a => new
                        {
                            a.AddressId,
                            a.AddressType,
                            a.AddressLine,
                            a.City,
                            a.State,
                            a.Country,
                            a.Pincode
                        })
                        .ToList(),

                    PaymentTerm = _context.SupplierPaymentTerms
                        .Where(p => p.SupplierId == x.SupplierId)
                        .Select(p => new
                        {
                            p.TermId,
                            p.CreditDays,
                            p.CreditLimit,
                            p.PaymentMethod,
                            p.Notes
                        })
                        .FirstOrDefault(),

                    BankAccounts = _context.SupplierBankAccounts
                        .Where(b => b.SupplierId == x.SupplierId)
                        .Select(b => new
                        {
                            b.BankId,
                            b.AccountName,
                            b.AccountNumber,
                            b.BankName,
                            b.IfscCode,
                            b.Branch,
                            b.BankState,
                            b.BankCity
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound(new
                {
                    message = "Supplier not found."
                });
            }

            return Ok(supplier);
        }

        // =========================
        // 🔹 CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ===============================
                // VALIDATIONS
                // ===============================

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new
                    {
                        message = "Supplier name is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Phone))
                {
                    return BadRequest(new
                    {
                        message = "Phone number is required."
                    });
                }

                var normalizedStatus = SupplierStatusNormalizer.Normalize(dto.Status, "active");
                if (normalizedStatus == null)
                {
                    return BadRequest(new
                    {
                        message = "Invalid supplier status."
                    });
                }

                _logger.LogInformation(
                    "Creating supplier. SupplierCode={SupplierCode}, RequestedStatus={RequestedStatus}, NormalizedStatus={NormalizedStatus}, TraceId={TraceId}",
                    dto.SupplierCode,
                    dto.Status,
                    normalizedStatus,
                    HttpContext.TraceIdentifier);

                var existingSupplier = await _context.Suppliers
                    .FirstOrDefaultAsync(x =>
                        !x.IsDeleted && x.SupplierCode == dto.SupplierCode);

                if (existingSupplier != null)
                {
                    return Conflict(new
                    {
                        message = "Supplier code already exists."
                    });
                }

                // ===============================
                // CREATE SUPPLIER
                // ===============================

                var supplier = new Supplier
                {
                    SupplierCode = dto.SupplierCode,
                    Name = dto.Name,
                    Category = dto.Category,
                    GstNumber = dto.GstNumber,
                    PanNumber = dto.PanNumber,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Website = dto.Website,
                    Status = normalizedStatus,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Suppliers.Add(supplier);

                await _context.SaveChangesAsync();

                // ===============================
                // CONTACTS
                // ===============================

                if (dto.Contacts.Any())
                {
                    var contacts = dto.Contacts.Select(contact => new SupplierContact
                    {
                        SupplierId = supplier.SupplierId,
                        Name = contact.Name,
                        Designation = contact.Designation,
                        Department = contact.Department,
                        Phone = contact.Phone,
                        Email = contact.Email,
                        IsPrimary = contact.IsPrimary
                    });

                    await _context.SupplierContacts.AddRangeAsync(contacts);
                }

                // ===============================
                // ADDRESSES
                // ===============================

                if (dto.Addresses.Any())
                {
                    var addresses = dto.Addresses.Select(address => new SupplierAddress
                    {
                        SupplierId = supplier.SupplierId,
                        AddressType = address.AddressType,
                        AddressLine = address.AddressLine,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        Pincode = address.Pincode
                    });

                    await _context.SupplierAddresses.AddRangeAsync(addresses);
                }

                // ===============================
                // PAYMENT TERMS
                // ===============================

                if (dto.PaymentTerm != null)
                {
                    var paymentTerm = new SupplierPaymentTerm
                    {
                        SupplierId = supplier.SupplierId,
                        CreditDays = dto.PaymentTerm.CreditDays,
                        CreditLimit = dto.PaymentTerm.CreditLimit,
                        PaymentMethod = dto.PaymentTerm.PaymentMethod,
                        Notes = dto.PaymentTerm.Notes
                    };

                    await _context.SupplierPaymentTerms.AddAsync(paymentTerm);
                }

                // ===============================
                // BANK ACCOUNTS
                // ===============================

                if (dto.BankAccounts.Any())
                {
                    var bankAccounts = dto.BankAccounts.Select(bank => new SupplierBankAccount
                    {
                        SupplierId = supplier.SupplierId,
                        AccountName = bank.AccountName,
                        AccountNumber = bank.AccountNumber,
                        BankName = bank.BankName,
                        IfscCode = bank.IfscCode,
                        Branch = bank.Branch,
                        BankState = bank.BankState,
                        BankCity = bank.BankCity
                    });

                    await _context.SupplierBankAccounts.AddRangeAsync(bankAccounts);
                }

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "CREATE_SUPPLIER",
                    "Suppliers",
                    supplier.SupplierId,
                    $"Supplier created: {supplier.Name}",
                    "suppliers");

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Supplier created successfully.",
                    supplierId = supplier.SupplierId
                });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(
                    ex,
                    "Supplier creation database failure. SupplierCode={SupplierCode}, RequestedStatus={RequestedStatus}, TraceId={TraceId}",
                    dto.SupplierCode,
                    dto.Status,
                    HttpContext.TraceIdentifier);

                return StatusCode(500, new
                {
                    message = GetSupplierPersistenceError(ex)
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(
                    ex,
                    "Supplier creation failed. SupplierCode={SupplierCode}, RequestedStatus={RequestedStatus}, TraceId={TraceId}",
                    dto.SupplierCode,
                    dto.Status,
                    HttpContext.TraceIdentifier);

                return StatusCode(500, new
                {
                    message = "Supplier creation failed. Please retry or contact support if the problem continues."
                });
            }
        }

        
        // =========================
        // 🔹 UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSupplierDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var documentPromotion = new SupplierDocumentPromotionResult();

            try
            {
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(x => x.SupplierId == id && !x.IsDeleted);

                if (supplier == null)
                {
                    return NotFound(new
                    {
                        message = "Supplier not found."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new
                    {
                        message = "Supplier name is required."
                    });
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Name.Trim(), @"^[A-Za-z\s]+$"))
                {
                    return BadRequest(new
                    {
                        message = "Name can contain only letters and spaces."
                    });
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Name.Trim(), @"^[A-Za-z\s]+$"))
                {
                    return BadRequest(new
                    {
                        message = "Name can contain only letters and spaces."
                    });
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Name.Trim(), @"^[A-Za-z\s]+$"))
                {
                    return BadRequest(new
                    {
                        message = "Name can contain only letters and spaces."
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Phone))
                {
                    return BadRequest(new
                    {
                        message = "Phone number is required."
                    });
                }

                var normalizedStatus = SupplierStatusNormalizer.Normalize(dto.Status, supplier.Status ?? "active");
                if (normalizedStatus == null)
                {
                    return BadRequest(new
                    {
                        message = "Invalid supplier status."
                    });
                }

                _logger.LogInformation(
                    "Updating supplier. SupplierId={SupplierId}, SupplierCode={SupplierCode}, PreviousStatus={PreviousStatus}, RequestedStatus={RequestedStatus}, NormalizedStatus={NormalizedStatus}, TraceId={TraceId}",
                    id,
                    dto.SupplierCode,
                    supplier.Status,
                    dto.Status,
                    normalizedStatus,
                    HttpContext.TraceIdentifier);

                // ===============================
                // UPDATE SUPPLIER
                // ===============================

                var previousStatus = SupplierStatusNormalizer.Normalize(supplier.Status, "active") ?? "active";

                supplier.SupplierCode = dto.SupplierCode;
                supplier.Name = dto.Name;
                supplier.Category = dto.Category;
                supplier.GstNumber = dto.GstNumber;
                supplier.PanNumber = dto.PanNumber;
                supplier.Phone = dto.Phone;
                supplier.Email = dto.Email;
                supplier.Website = dto.Website;
                supplier.Status = normalizedStatus;
                supplier.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // ===============================
                // REMOVE OLD CHILD RECORDS
                // ===============================

                var oldContacts = _context.SupplierContacts
                    .Where(x => x.SupplierId == id);

                _context.SupplierContacts.RemoveRange(oldContacts);

                var oldAddresses = _context.SupplierAddresses
                    .Where(x => x.SupplierId == id);

                _context.SupplierAddresses.RemoveRange(oldAddresses);

                var oldPaymentTerms = _context.SupplierPaymentTerms
                    .Where(x => x.SupplierId == id);

                _context.SupplierPaymentTerms.RemoveRange(oldPaymentTerms);

                var oldBankAccounts = _context.SupplierBankAccounts
                    .Where(x => x.SupplierId == id);

                _context.SupplierBankAccounts.RemoveRange(oldBankAccounts);

                await _context.SaveChangesAsync();

                // ===============================
                // INSERT CONTACTS
                // ===============================

                if (dto.Contacts.Any())
                {
                    var contacts = dto.Contacts.Select(contact => new SupplierContact
                    {
                        SupplierId = supplier.SupplierId,
                        Name = contact.Name,
                        Designation = contact.Designation,
                        Department = contact.Department,
                        Phone = contact.Phone,
                        Email = contact.Email,
                        IsPrimary = contact.IsPrimary
                    });

                    await _context.SupplierContacts.AddRangeAsync(contacts);
                }

                // ===============================
                // INSERT ADDRESSES
                // ===============================

                if (dto.Addresses.Any())
                {
                    var addresses = dto.Addresses.Select(address => new SupplierAddress
                    {
                        SupplierId = supplier.SupplierId,
                        AddressType = address.AddressType,
                        AddressLine = address.AddressLine,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        Pincode = address.Pincode
                    });

                    await _context.SupplierAddresses.AddRangeAsync(addresses);
                }

                // ===============================
                // INSERT PAYMENT TERM
                // ===============================

                if (dto.PaymentTerm != null)
                {
                    var paymentTerm = new SupplierPaymentTerm
                    {
                        SupplierId = supplier.SupplierId,
                        CreditDays = dto.PaymentTerm.CreditDays,
                        CreditLimit = dto.PaymentTerm.CreditLimit,
                        PaymentMethod = dto.PaymentTerm.PaymentMethod,
                        Notes = dto.PaymentTerm.Notes
                    };

                    await _context.SupplierPaymentTerms.AddAsync(paymentTerm);
                }

                // ===============================
                // INSERT BANK ACCOUNTS
                // ===============================

                if (dto.BankAccounts.Any())
                {
                    var bankAccounts = dto.BankAccounts.Select(bank => new SupplierBankAccount
                    {
                        SupplierId = supplier.SupplierId,
                        AccountName = bank.AccountName,
                        AccountNumber = bank.AccountNumber,
                        BankName = bank.BankName,
                        IfscCode = bank.IfscCode,
                        Branch = bank.Branch,
                        BankState = bank.BankState,
                        BankCity = bank.BankCity
                    });

                    await _context.SupplierBankAccounts.AddRangeAsync(bankAccounts);
                }

                documentPromotion = await PromotePendingSupplierDocumentsAsync(id);

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "UPDATE_SUPPLIER",
                    "Suppliers",
                    supplier.SupplierId,
                    $"Supplier updated: {supplier.Name}",
                    "suppliers");

                if (!previousStatus.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase))
                {
                    await _auditLogService.LogAsync(
                        "SUPPLIER_STATUS_CHANGED",
                        "Suppliers",
                        supplier.SupplierId,
                        $"Supplier status changed from {previousStatus} to {normalizedStatus}",
                        "suppliers");
                }

                await transaction.CommitAsync();
                DeletePromotedSupplierDocumentReplacements(documentPromotion);

                return Ok(new
                {
                    message = "Supplier updated successfully."
                });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                TryRestorePromotedSupplierDocumentFiles(documentPromotion);
                _logger.LogError(
                    ex,
                    "Supplier update database failure. SupplierId={SupplierId}, SupplierCode={SupplierCode}, RequestedStatus={RequestedStatus}, TraceId={TraceId}",
                    id,
                    dto.SupplierCode,
                    dto.Status,
                    HttpContext.TraceIdentifier);

                return StatusCode(500, new
                {
                    message = GetSupplierPersistenceError(ex)
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TryRestorePromotedSupplierDocumentFiles(documentPromotion);
                _logger.LogError(
                    ex,
                    "Supplier update failed. SupplierId={SupplierId}, SupplierCode={SupplierCode}, RequestedStatus={RequestedStatus}, TraceId={TraceId}",
                    id,
                    dto.SupplierCode,
                    dto.Status,
                    HttpContext.TraceIdentifier);

                return StatusCode(500, new
                {
                    message = "Supplier update failed. Please retry or contact support if the problem continues."
                });
            }
        }

        private static string GetSupplierPersistenceError(DbUpdateException exception)
        {
            var databaseMessage = exception.InnerException?.Message ?? exception.Message;

            if (databaseMessage.Contains("status", StringComparison.OrdinalIgnoreCase))
            {
                return "Supplier status could not be saved because the database status column is not aligned. Run the supplier status workflow migration.";
            }

            return "Supplier data could not be saved. Please retry or contact support if the problem continues.";
        }

        // =========================
        // 🔹 DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.SupplierId == id && !x.IsDeleted);

            if (supplier == null)
            {
                return NotFound(new
                {
                    message = "Supplier not found."
                });
            }

            supplier.IsDeleted = true;
            supplier.DeletedAt = DateTime.UtcNow;
            supplier.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "DELETE_SUPPLIER",
                    "Suppliers",
                    supplier.SupplierId,
                    $"Supplier deleted: {supplier.Name}",
                    "suppliers");

                _logger.LogInformation(
                    "Supplier archived. SupplierId={SupplierId}, SupplierCode={SupplierCode}, TraceId={TraceId}",
                    id,
                    supplier.SupplierCode,
                    HttpContext.TraceIdentifier);

                return Ok(new
                {
                    message = "Supplier archived successfully."
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Supplier archive database failure. SupplierId={SupplierId}, TraceId={TraceId}",
                    id,
                    HttpContext.TraceIdentifier);

                return StatusCode(500, new
                {
                    message = "Supplier could not be archived. Please retry or contact support if the problem continues."
                });
            }
        }







        [HttpGet("ifsc/{ifscCode}")]
        public async Task<IActionResult> GetBankDetails(string ifscCode)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(
                $"https://ifsc.razorpay.com/{ifscCode}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new
                {
                    message = "Invalid IFSC code"
                });
            }

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }






        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.SupplierId == id && x.IsDeleted);

            if (supplier == null)
            {
                return NotFound(new
                {
                    message = "Archived supplier not found."
                });
            }

            supplier.IsDeleted = false;
            supplier.DeletedAt = null;
            supplier.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Supplier restored. SupplierId={SupplierId}, SupplierCode={SupplierCode}, TraceId={TraceId}",
                    id,
                    supplier.SupplierCode,
                    HttpContext.TraceIdentifier);

                return Ok(new
                {
                    message = "Supplier restored successfully."
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Supplier restore database failure. SupplierId={SupplierId}, TraceId={TraceId}",
                    id,
                    HttpContext.TraceIdentifier);

                return StatusCode(500, new
                {
                    message = "Supplier could not be restored. Please retry or contact support if the problem continues."
                });
            }
        }





        [HttpPost("{supplierId}/documents/upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(SupplierDocumentMultipartBodyLimit)]
        [RequestFormLimits(MultipartBodyLengthLimit = SupplierDocumentMultipartBodyLimit)]
        public async Task<IActionResult> UploadDocument(
    int supplierId,
    [FromForm] UploadSupplierDocumentDto dto)
        {
            string? normalizedDocumentType = null;
            string? originalFileName = dto.File?.FileName;
            string? fullFilePath = null;
            var supersededTempDocuments = new List<SupplierDocument>();

            try
            {
                _logger.LogInformation(
                    "Supplier document upload received. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, ContentType={ContentType}, FileSize={FileSize}, TraceId={TraceId}",
                    supplierId,
                    dto.DocumentType,
                    originalFileName,
                    dto.File?.ContentType,
                    dto.File?.Length,
                    HttpContext.TraceIdentifier);

                // ===============================
                // SUPPLIER VALIDATION
                // ===============================

                bool supplierExists;

                try
                {
                    supplierExists = await _context.Suppliers
                        .AnyAsync(x => x.SupplierId == supplierId && !x.IsDeleted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Supplier document upload supplier lookup failed. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, TraceId={TraceId}",
                        supplierId,
                        dto.DocumentType,
                        originalFileName,
                        HttpContext.TraceIdentifier);

                    return SupplierDocumentUploadError(
                        500,
                        "Supplier lookup failed. Please retry.",
                        ex);
                }

                if (!supplierExists)
                {
                    _logger.LogWarning(
                        "Supplier document upload rejected. Supplier not found. SupplierId={SupplierId}, TraceId={TraceId}",
                        supplierId,
                        HttpContext.TraceIdentifier);

                    return NotFound(new
                    {
                        message = "Supplier not found."
                    });
                }

                // ===============================
                // FILE VALIDATION
                // ===============================

                if (dto.File == null || dto.File.Length == 0)
                {
                    _logger.LogWarning(
                        "Supplier document upload rejected. Empty file. SupplierId={SupplierId}, DocumentType={DocumentType}, TraceId={TraceId}",
                        supplierId,
                        dto.DocumentType,
                        HttpContext.TraceIdentifier);

                    return BadRequest(new
                    {
                        message = "Document file is required."
                    });
                }

                originalFileName = Path.GetFileName(dto.File.FileName);
                var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

                if (!SupplierDocumentAllowedExtensions.Contains(extension))
                {
                    _logger.LogWarning(
                        "Supplier document upload rejected. Invalid extension. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, Extension={Extension}, TraceId={TraceId}",
                        supplierId,
                        dto.DocumentType,
                        originalFileName,
                        extension,
                        HttpContext.TraceIdentifier);

                    return BadRequest(new
                    {
                        message = "Only PDF, JPG, JPEG, and PNG files are allowed."
                    });
                }

                // ===============================
                // FILE SIZE VALIDATION
                // ===============================

                if (dto.File.Length > SupplierDocumentMaxFileSize)
                {
                    _logger.LogWarning(
                        "Supplier document upload rejected. File too large. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, FileSize={FileSize}, MaxFileSize={MaxFileSize}, TraceId={TraceId}",
                        supplierId,
                        dto.DocumentType,
                        originalFileName,
                        dto.File.Length,
                        SupplierDocumentMaxFileSize,
                        HttpContext.TraceIdentifier);

                    return BadRequest(new
                    {
                        message = "File size cannot exceed 10 MB."
                    });
                }

                // ===============================
                // DOCUMENT TYPE VALIDATION
                // ===============================

                if (string.IsNullOrWhiteSpace(dto.DocumentType))
                {
                    _logger.LogWarning(
                        "Supplier document upload rejected. Missing document type. SupplierId={SupplierId}, FileName={FileName}, TraceId={TraceId}",
                        supplierId,
                        dto.File.FileName,
                        HttpContext.TraceIdentifier);

                    return BadRequest(new
                    {
                        message = "Invalid supplier document category."
                    });
                }

                normalizedDocumentType = SupplierDocumentTypes.Normalize(dto.DocumentType);

                if (normalizedDocumentType == null)
                {
                    _logger.LogWarning(
                        "Supplier document upload rejected. Unsupported document type. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, TraceId={TraceId}",
                        supplierId,
                        dto.DocumentType,
                        dto.File.FileName,
                        HttpContext.TraceIdentifier);

                    return BadRequest(new
                    {
                        message = "Unsupported compliance document type."
                    });
                }

                if (normalizedDocumentType == SupplierDocumentTypes.Gst ||
                    normalizedDocumentType == SupplierDocumentTypes.Pan ||
                    normalizedDocumentType == SupplierDocumentTypes.Agreement)
                {
                    supersededTempDocuments = await _context.SupplierDocuments
                        .Where(x =>
                            x.SupplierId == supplierId &&
                            x.DocumentType == normalizedDocumentType &&
                            x.IsTemporary &&
                            !x.IsDeleted)
                        .ToListAsync();

                    _context.SupplierDocuments.RemoveRange(supersededTempDocuments);
                }

                // ===============================
                // GENERATE UNIQUE FILE NAME
                // ===============================

                var storedFileName = $"{Guid.NewGuid():N}{extension}";

                // ===============================
                // PHYSICAL STORAGE PATH
                // ===============================

                var uploadFolder = GetSupplierDocumentUploadFolder(true);

                Directory.CreateDirectory(uploadFolder);
                VerifySupplierDocumentUploadFolder(uploadFolder);

                fullFilePath = Path.Combine(uploadFolder, storedFileName);

                _logger.LogInformation(
                    "Supplier document storage path resolved. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, SavePath={SavePath}, TraceId={TraceId}",
                    supplierId,
                    normalizedDocumentType,
                    originalFileName,
                    fullFilePath,
                    HttpContext.TraceIdentifier);

                // ===============================
                // SAVE FILE
                // ===============================

                try
                {
                    await using var stream = new FileStream(
                        fullFilePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None);

                    await dto.File.CopyToAsync(stream);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    _logger.LogError(
                        ex,
                        "Supplier document upload file save failed. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, SavePath={SavePath}, TraceId={TraceId}",
                        supplierId,
                        normalizedDocumentType,
                        originalFileName,
                        fullFilePath,
                        HttpContext.TraceIdentifier);

                    return SupplierDocumentUploadError(
                        500,
                        ex is UnauthorizedAccessException
                            ? "Document storage path is not writable."
                            : "Document file could not be saved. Please retry.",
                        ex);
                }

                // ===============================
                // SAVE DATABASE RECORD
                // ===============================

                var document = new SupplierDocument
                {
                    SupplierId = supplierId,
                    DocumentType = normalizedDocumentType,

                    DisplayName = TrimToMax(originalFileName, 150),

                    OriginalFileName = TrimToMax(originalFileName, 255),

                    StoredFileName = storedFileName,

                    FilePath =
                        GetSupplierDocumentRelativePath(storedFileName, true),

                    ContentType = TrimToMax(
                        string.IsNullOrWhiteSpace(dto.File.ContentType)
                            ? "application/octet-stream"
                            : dto.File.ContentType,
                        100),

                    FileSizeInBytes = dto.File.Length,

                    Status = "pending",

                    IsTemporary = true,

                    UploadedAt = DateTime.UtcNow
                };

                try
                {
                    _context.SupplierDocuments.Add(document);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(
                        ex,
                        "Supplier document upload database insert failed. SupplierId={SupplierId}, DocumentType={DocumentType}, OriginalFileName={OriginalFileName}, StoredFileName={StoredFileName}, SavePath={SavePath}, DatabaseError={DatabaseError}, TraceId={TraceId}",
                        supplierId,
                        document.DocumentType,
                        document.OriginalFileName,
                        document.StoredFileName,
                        fullFilePath,
                        ex.InnerException?.Message ?? ex.Message,
                        HttpContext.TraceIdentifier);

                    TryDeleteSupplierDocumentFile(fullFilePath);

                    return SupplierDocumentUploadError(
                        500,
                        "Document metadata could not be saved. Please retry.",
                        ex);
                }

                _logger.LogInformation(
                    "Supplier document staged. SupplierId={SupplierId}, DocumentId={DocumentId}, DocumentType={DocumentType}, OriginalFileName={OriginalFileName}, StoredFileName={StoredFileName}, FileSize={FileSize}, TempPath={TempPath}, TraceId={TraceId}",
                    supplierId,
                    document.DocumentId,
                    document.DocumentType,
                    document.OriginalFileName,
                    document.StoredFileName,
                    document.FileSizeInBytes,
                    fullFilePath,
                    HttpContext.TraceIdentifier);

                foreach (var supersededDocument in supersededTempDocuments)
                {
                    TryDeleteSupplierDocumentFile(GetSupplierDocumentFullPath(supersededDocument.FilePath));
                }

                // ===============================
                // SUCCESS RESPONSE
                // ===============================

                return Ok(new
                {
                    message = "Document staged successfully. Save supplier to finalize.",

                    data = new
                    {
                        document.DocumentId,
                        document.SupplierId,
                        document.DocumentType,
                        document.DisplayName,
                        document.OriginalFileName,
                        document.FilePath,
                        document.ContentType,
                        document.FileSizeInBytes,
                        document.Status,
                        document.IsTemporary,
                        document.UploadedAt
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Supplier document upload storage permission failure. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, SavePath={SavePath}, TraceId={TraceId}",
                    supplierId,
                    normalizedDocumentType ?? dto.DocumentType,
                    originalFileName,
                    fullFilePath,
                    HttpContext.TraceIdentifier);

                return SupplierDocumentUploadError(
                    500,
                    "Document storage path is not writable.",
                    ex);
            }
            catch (IOException ex)
            {
                _logger.LogError(
                    ex,
                    "Supplier document upload file save failure. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, SavePath={SavePath}, TraceId={TraceId}",
                    supplierId,
                    normalizedDocumentType ?? dto.DocumentType,
                    originalFileName,
                    fullFilePath,
                    HttpContext.TraceIdentifier);

                return SupplierDocumentUploadError(
                    500,
                    "Document file could not be saved. Please retry.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Supplier document upload database failure. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, SavePath={SavePath}, TraceId={TraceId}",
                    supplierId,
                    normalizedDocumentType ?? dto.DocumentType,
                    originalFileName,
                    fullFilePath,
                    HttpContext.TraceIdentifier);

                TryDeleteSupplierDocumentFile(fullFilePath);

                return SupplierDocumentUploadError(
                    500,
                    "Document metadata could not be saved. Please retry.",
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Supplier document upload failed. SupplierId={SupplierId}, DocumentType={DocumentType}, FileName={FileName}, SavePath={SavePath}, TraceId={TraceId}",
                    supplierId,
                    normalizedDocumentType ?? dto.DocumentType,
                    originalFileName,
                    fullFilePath,
                    HttpContext.TraceIdentifier);

                TryDeleteSupplierDocumentFile(fullFilePath);

                return SupplierDocumentUploadError(
                    500,
                    "Document upload failed. Please retry later.",
                    ex);
            }
        }




        [HttpGet("{supplierId}/documents")]
        public async Task<IActionResult> GetSupplierDocuments(int supplierId)
        {
            var supplierExists = await _context.Suppliers
                .AnyAsync(x => x.SupplierId == supplierId && !x.IsDeleted);

            if (!supplierExists)
            {
                return NotFound(new
                {
                    message = "Supplier not found."
                });
            }

            var documents = await _context.SupplierDocuments
                .Where(x =>
                    x.SupplierId == supplierId &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new SupplierDocumentResponseDto
                {
                    DocumentId = x.DocumentId,
                    SupplierId = x.SupplierId,
                    DocumentType = x.DocumentType,
                    DisplayName = x.DisplayName,
                    OriginalFileName = x.OriginalFileName,
                    FilePath = x.FilePath,
                    ContentType = x.ContentType,
                    FileSizeInBytes = x.FileSizeInBytes,
                    Status = x.Status,
                    IsTemporary = x.IsTemporary,
                    UploadedAt = x.UploadedAt
                })
                .ToListAsync();

            return Ok(documents);
        }





        [HttpDelete("documents/{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var document = await _context.SupplierDocuments
                .FirstOrDefaultAsync(x =>
                    x.DocumentId == documentId &&
                    !x.IsDeleted);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Document not found."
                });
            }

            if (document.IsTemporary)
            {
                var tempPath = GetSupplierDocumentFullPath(document.FilePath);
                _context.SupplierDocuments.Remove(document);
                await _context.SaveChangesAsync();
                TryDeleteSupplierDocumentFile(tempPath);
            }
            else
            {
                document.IsDeleted = true;
                document.DeletedAt = DateTime.UtcNow;
                document.Status = "deleted";

                await _context.SaveChangesAsync();
                TryDeleteSupplierDocumentFile(GetSupplierDocumentFullPath(document.FilePath));
            }

            return Ok(new
            {
                message = "Document deleted successfully."
            });
        }


        [HttpDelete("{supplierId}/documents/temp")]
        public async Task<IActionResult> CleanupTemporaryDocuments(int supplierId)
        {
            var supplierExists = await _context.Suppliers
                .AnyAsync(x => x.SupplierId == supplierId && !x.IsDeleted);

            if (!supplierExists)
            {
                return NotFound(new
                {
                    message = "Supplier not found."
                });
            }

            await CleanupSupplierTempDocumentsAsync(supplierId);

            return Ok(new
            {
                message = "Temporary supplier documents cleared."
            });
        }




        [HttpGet("documents/{documentId}/download")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var document = await _context.SupplierDocuments
                .FirstOrDefaultAsync(x =>
                    x.DocumentId == documentId &&
                    !x.IsDeleted);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Document not found."
                });
            }

            var fullPath = GetSupplierDocumentFullPath(document.FilePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(new
                {
                    message = "Physical file not found."
                });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            return File(
                fileBytes,
                document.ContentType,
                document.OriginalFileName);
        }


    }
}
