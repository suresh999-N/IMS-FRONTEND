
using IMSBackend.Data;
using System.Text.RegularExpressions;
using IMSBackend.DTOs;
using IMSBackend.Models;
using IMSBackend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<CustomersController> _logger;
        private static readonly SemaphoreSlim CustomerMasterSchemaLock = new(1, 1);
        private static bool _customerMasterSchemaEnsured;

        public CustomersController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<CustomersController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        private static string Clean(string? value)
            => CustomerDto.CollapseSpaces(value);

        private static string NormalizeEmail(string? value)
            => Regex.Replace(Clean(value).ToLowerInvariant(), @"\s+", string.Empty);

        private static void NormalizeSinglePrimary<T>(
            List<T> items,
            Func<T, bool> isPrimary,
            Action<T, bool> setPrimary)
        {
            if (items.Count == 0)
            {
                return;
            }

            var primarySeen = false;
            foreach (var item in items)
            {
                if (!isPrimary(item))
                {
                    continue;
                }

                if (!primarySeen)
                {
                    primarySeen = true;
                    continue;
                }

                setPrimary(item, false);
            }

            if (!primarySeen)
            {
                setPrimary(items[0], true);
            }
        }

        private static readonly HashSet<string> AllowedCustomerStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Active",
            "Inactive",
            "Blocked"
        };

        public sealed class CustomerStatusUpdateDto
        {
            public string? Status { get; set; }
            public string? Reason { get; set; }
        }

        private static string NormalizeStatus(string? value)
        {
            var status = Clean(value);

            if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
            {
                return "Active";
            }

            if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
            {
                return "Inactive";
            }

            if (status.Equals("blocked", StringComparison.OrdinalIgnoreCase))
            {
                return "Blocked";
            }

            return status;
        }

        private static object CustomerResponse(Customer customer, string? notes = null) => new
        {
            CustomerId = customer.CustomerId,
            CustomerCode = customer.CustomerCode,
            Name = customer.Name,
            Company = customer.Company,
            GstNumber = customer.GstNumber,
            PanNumber = customer.PanNumber,
            Phone = customer.Phone,
            Email = customer.Email,
            City = customer.City,
            CreditLimit = customer.CreditLimit,
            OutstandingBalance = customer.OutstandingBalance,
            Status = customer.Status,
            CreatedAt = customer.CreatedAt,
            created_at = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            Notes = notes
        };

        private async Task<object> CustomerResponseAsync(Customer customer, string? notes = null)
        {
            await EnsureCustomerMasterSchemaAsync();

            var contacts = await _context.CustomerContacts
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.CustomerId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.ContactId)
                .Select(x => new
                {
                    x.ContactId,
                    x.ContactName,
                    x.Role,
                    x.Designation,
                    x.Phone,
                    x.Email,
                    x.IsPrimary
                })
                .ToListAsync();

            var addresses = await _context.CustomerAddresses
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.CustomerId)
                .OrderBy(x => x.AddressId)
                .Select(x => new
                {
                    x.AddressId,
                    x.AddressType,
                    x.AddressLine,
                    x.AddressLine2,
                    x.City,
                    x.State,
                    x.Country,
                    x.Pincode,
                    x.IsPrimary
                })
                .ToListAsync();

            var paymentTerms = await _context.CustomerPaymentTerms
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.CustomerId)
                .OrderByDescending(x => x.PaymentTermId)
                .Select(x => new
                {
                    x.PaymentTermId,
                    x.CreditDays,
                    x.CreditLimit,
                    x.PaymentMode,
                    x.Notes
                })
                .FirstOrDefaultAsync();

            var bankDetails = await _context.CustomerBankDetails
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.CustomerId)
                .OrderBy(x => x.BankDetailId)
                .Select(x => new
                {
                    x.BankDetailId,
                    x.AccountName,
                    x.AccountNumber,
                    x.BankName,
                    x.IfscCode,
                    x.Branch,
                    x.IsPrimary
                })
                .ToListAsync();

            return new
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customer.CustomerCode,
                Name = customer.Name,
                Company = customer.Company,
                GstNumber = customer.GstNumber,
                PanNumber = customer.PanNumber,
                Phone = customer.Phone,
                Email = customer.Email,
                City = customer.City,
                CreditLimit = customer.CreditLimit,
                OutstandingBalance = customer.OutstandingBalance,
                Status = customer.Status,
                CreatedAt = customer.CreatedAt,
                created_at = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                Notes = notes,
                Contacts = contacts,
                Addresses = addresses,
                PaymentTerms = paymentTerms,
                BankDetails = bankDetails
            };
        }

        private async Task EnsureCustomerMasterSchemaAsync()
        {
            if (_customerMasterSchemaEnsured)
            {
                return;
            }

            await CustomerMasterSchemaLock.WaitAsync();
            try
            {
                if (_customerMasterSchemaEnsured)
                {
                    return;
                }

                // Some deployed IMS databases predate the Customer master child tabs.
                // Ensure these Supplier-parity columns exist before EF tries to read/write them.
                await EnsureColumnAsync(
                    "customer_contacts",
                    "role",
                    "ALTER TABLE customer_contacts ADD COLUMN role VARCHAR(100) NULL;");
                await EnsureColumnAsync(
                    "customer_addresses",
                    "address_line2",
                    "ALTER TABLE customer_addresses ADD COLUMN address_line2 VARCHAR(255) NULL;");
                await EnsureColumnAsync(
                    "customer_addresses",
                    "is_primary",
                    "ALTER TABLE customer_addresses ADD COLUMN is_primary TINYINT(1) NOT NULL DEFAULT 0;");
                await EnsureColumnAsync(
                    "customer_bank_details",
                    "is_primary",
                    "ALTER TABLE customer_bank_details ADD COLUMN is_primary TINYINT(1) NOT NULL DEFAULT 0;");
                await EnsureColumnAsync(
                    "customer_payment_terms",
                    "payment_method",
                    "ALTER TABLE customer_payment_terms ADD COLUMN payment_method VARCHAR(100) NULL;");

                _customerMasterSchemaEnsured = true;
            }
            finally
            {
                CustomerMasterSchemaLock.Release();
            }
        }

        private async Task EnsureColumnAsync(string tableName, string columnName, string addColumnSql)
        {
            var existingColumnCount = await _context.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) AS `Value` FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = {0} AND COLUMN_NAME = {1}",
                tableName,
                columnName).SingleAsync();

            if (existingColumnCount == 0)
            {
                await _context.Database.ExecuteSqlRawAsync(addColumnSql);
            }
        }

        private async Task SaveCustomerMasterChildrenAsync(int customerId, CustomerDto dto, bool replaceExisting)
        {
            await EnsureCustomerMasterSchemaAsync();

            if (replaceExisting)
            {
                await _context.CustomerContacts.Where(x => x.CustomerId == customerId).ExecuteDeleteAsync();
                await _context.CustomerAddresses.Where(x => x.CustomerId == customerId).ExecuteDeleteAsync();
                await _context.CustomerBankDetails.Where(x => x.CustomerId == customerId).ExecuteDeleteAsync();
                await _context.CustomerPaymentTerms.Where(x => x.CustomerId == customerId).ExecuteDeleteAsync();
            }

            var contacts = dto.Contacts
                .Where(x => !string.IsNullOrWhiteSpace(x.ContactName) || !string.IsNullOrWhiteSpace(x.Phone) || !string.IsNullOrWhiteSpace(x.Email))
                .Select((x, index) => new CustomerContact
                {
                    CustomerId = customerId,
                    ContactName = Clean(x.ContactName),
                    Role = Clean(x.Role),
                    Designation = Clean(x.Designation),
                    Phone = CustomerDto.NormalizePhone(x.Phone),
                    Email = NormalizeEmail(x.Email),
                    IsPrimary = x.IsPrimary || index == 0
                })
                .ToList();

            if (contacts.Count > 1 && contacts.Count(x => x.IsPrimary) > 1)
            {
                var primarySeen = false;
                foreach (var contact in contacts)
                {
                    if (!contact.IsPrimary)
                    {
                        continue;
                    }

                    if (!primarySeen)
                    {
                        primarySeen = true;
                        continue;
                    }

                    contact.IsPrimary = false;
                }
            }

            var addresses = dto.Addresses
                .Where(x => !string.IsNullOrWhiteSpace(x.AddressLine) || !string.IsNullOrWhiteSpace(x.City))
                .Select((x, index) => new CustomerAddress
                {
                    CustomerId = customerId,
                    AddressType = Clean(x.AddressType),
                    AddressLine = Clean(x.AddressLine),
                    AddressLine2 = Clean(x.AddressLine2),
                    City = Clean(x.City),
                    State = Clean(x.State),
                    Country = Clean(x.Country),
                    Pincode = Clean(x.Pincode),
                    IsPrimary = x.IsPrimary || index == 0
                })
                .ToList();

            var submittedBankDetails = dto.BankDetails.Count > 0 ? dto.BankDetails : dto.BankAccounts;
            var bankDetails = submittedBankDetails
                .Where(x => !string.IsNullOrWhiteSpace(x.AccountName) || !string.IsNullOrWhiteSpace(x.AccountNumber) || !string.IsNullOrWhiteSpace(x.BankName))
                .Select((x, index) => new CustomerBankDetail
                {
                    CustomerId = customerId,
                    AccountName = Clean(x.AccountName),
                    AccountNumber = Clean(x.AccountNumber),
                    BankName = Clean(x.BankName),
                    IfscCode = Clean(x.IfscCode).ToUpperInvariant(),
                    Branch = Clean(x.Branch),
                    IsPrimary = x.IsPrimary || index == 0
                })
                .ToList();

            NormalizeSinglePrimary(addresses, address => address.IsPrimary, (address, isPrimary) => address.IsPrimary = isPrimary);
            NormalizeSinglePrimary(bankDetails, bank => bank.IsPrimary, (bank, isPrimary) => bank.IsPrimary = isPrimary);

            if (contacts.Count > 0)
            {
                await _context.CustomerContacts.AddRangeAsync(contacts);
            }

            if (addresses.Count > 0)
            {
                await _context.CustomerAddresses.AddRangeAsync(addresses);
            }

            if (bankDetails.Count > 0)
            {
                await _context.CustomerBankDetails.AddRangeAsync(bankDetails);
            }

            if (dto.PaymentTerms != null)
            {
                await _context.CustomerPaymentTerms.AddAsync(new CustomerPaymentTerm
                {
                    CustomerId = customerId,
                    CreditDays = Math.Max(0, dto.PaymentTerms.CreditDays),
                    CreditLimit = Math.Max(0, dto.PaymentTerms.CreditLimit),
                    PaymentMode = Clean(dto.PaymentTerms.PaymentMode),
                    Notes = Clean(dto.PaymentTerms.Notes)
                });
            }

            var activities = dto.Activities
                .Where(activity => !string.IsNullOrWhiteSpace(activity.ActivityType) || !string.IsNullOrWhiteSpace(activity.Description))
                .Select(activity => new CustomerActivity
                {
                    CustomerId = customerId,
                    ActivityType = Clean(activity.ActivityType),
                    Description = Clean(activity.Description),
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            if (activities.Count > 0)
            {
                await _context.CustomerActivities.AddRangeAsync(activities);
            }
        }

        private static BadRequestObjectResult ValidationError(Dictionary<string, string[]> errors)
            => new(new
            {
                success = false,
                message = "Validation failed.",
                errors
            });

        private async Task<Dictionary<string, string[]>> GetDuplicateErrorsAsync(
            string email,
            string phone,
            string customerCode,
            int? excludeCustomerId = null)
        {
            var errors = new Dictionary<string, string[]>();

            if (!string.IsNullOrWhiteSpace(customerCode))
            {
                var codeExists = await _context.Customers
                    .AsNoTracking()
                    .AnyAsync(customer =>
                        (!excludeCustomerId.HasValue || customer.CustomerId != excludeCustomerId.Value) &&
                        customer.CustomerCode != null &&
                        customer.CustomerCode.ToLower() == customerCode.ToLower());

                if (codeExists)
                {
                    errors[nameof(CustomerDto.CustomerCode)] = ["This customer code is already used by another customer."];
                }
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailExists = await _context.Customers
                    .AsNoTracking()
                    .AnyAsync(customer =>
                        (!excludeCustomerId.HasValue || customer.CustomerId != excludeCustomerId.Value) &&
                        customer.Email != null &&
                        customer.Email.ToLower() == email.ToLower());

                if (emailExists)
                {
                    errors[nameof(CustomerDto.Email)] = ["This email is already used by another customer."];
                }
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var phoneRows = await _context.Customers
                    .AsNoTracking()
                    .Where(customer => !excludeCustomerId.HasValue || customer.CustomerId != excludeCustomerId.Value)
                    .Select(customer => new { customer.CustomerId, customer.Phone })
                    .ToListAsync();

                if (phoneRows.Any(customer => CustomerDto.NormalizePhone(customer.Phone) == phone))
                {
                    errors[nameof(CustomerDto.Phone)] = ["This phone number is already used by another customer."];
                }
            }

            return errors;
        }

        private async Task<string> GenerateNextCustomerCodeAsync()
        {
            const string customerCodePrefix = "CUST-";
            const int customerCodeDigits = 6;

            // Customer codes are global business identifiers, not name-derived labels.
            // A serializable transaction plus a unique database index protects this
            // read-next/write-next sequence from concurrent create requests.
            var existingCodes = await _context.Customers
                .AsNoTracking()
                .Where(customer => customer.CustomerCode != null && customer.CustomerCode.StartsWith(customerCodePrefix))
                .Select(customer => customer.CustomerCode!)
                .ToListAsync();

            var maxSequence = existingCodes
                .Select(code => Regex.Match(code, @"^CUST-(\d{6})$"))
                .Where(match => match.Success && int.TryParse(match.Groups[1].Value, out _))
                .Select(match => int.Parse(match.Groups[1].Value))
                .DefaultIfEmpty(0)
                .Max();

            for (var nextSequence = maxSequence + 1; nextSequence < maxSequence + 10000; nextSequence++)
            {
                var candidate = $"{customerCodePrefix}{nextSequence.ToString().PadLeft(customerCodeDigits, '0')}";
                var exists = await _context.Customers
                    .AsNoTracking()
                    .AnyAsync(customer => customer.CustomerCode == candidate);

                if (!exists)
                {
                    return candidate;
                }
            }

            return $"{customerCodePrefix}{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        private async Task<string> AllocateCustomerCodeFromIdAsync(int customerId)
        {
            // Name-derived codes caused collisions, and scanning existing codes could reuse
            // identifiers after deletes. The database identity is the durable sequence source,
            // so this stays unique under concurrent creates and never depends on customer name.
            var candidate = $"CUST-{customerId:D6}";
            var exists = await _context.Customers
                .AsNoTracking()
                .AnyAsync(customer => customer.CustomerId != customerId && customer.CustomerCode == candidate);

            return exists
                ? $"CUST-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
                : candidate;
        }

        private async Task RollbackSafelyAsync(IDbContextTransaction transaction)
        {
            try
            {
                await transaction.RollbackAsync();
            }
            catch (Exception rollbackException)
            {
                _logger.LogError(
                    rollbackException,
                    "Customer transaction rollback failed. TraceId={TraceId}",
                    HttpContext.TraceIdentifier);
            }
        }

        private static bool IsCustomerCodeUniqueViolation(DbUpdateException exception)
        {
            var detail = $"{exception.Message} {exception.InnerException?.Message}".ToLowerInvariant();
            return detail.Contains("duplicate") &&
                (detail.Contains("customer_code") || detail.Contains("ux_customers_customer_code"));
        }

        private static string GetInnermostMessage(Exception exception)
        {
            var current = exception;
            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return current.Message;
        }

        // =====================================
        // GET ALL CUSTOMERS
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetCustomers(
    int page = 1,
    int pageSize = 500,
    string? search = null,
    string sortBy = "customerId",
    string sortOrder = "desc")
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 500);

            var query = _context.Customers.AsQueryable();

            // ================= SEARCH =================
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    (x.Email != null && x.Email.ToLower().Contains(search)) ||
                    (x.Phone != null && x.Phone.ToLower().Contains(search)) ||
                    (x.Company != null && x.Company.ToLower().Contains(search)) ||
                    (x.CustomerCode != null && x.CustomerCode.ToLower().Contains(search)) ||
                    (x.GstNumber != null && x.GstNumber.ToLower().Contains(search)));
            }

            // ================= TOTAL COUNT =================
            var totalRecords = await query.CountAsync();

            // ================= SORTING =================
            query = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("name", "asc") => query.OrderBy(x => x.Name),
                ("name", "desc") => query.OrderByDescending(x => x.Name),

                ("city", "asc") => query.OrderBy(x => x.City),
                ("city", "desc") => query.OrderByDescending(x => x.City),

                ("createdat", "asc") => query.OrderBy(x => x.CreatedAt).ThenBy(x => x.CustomerId),
                ("createdat", "desc") => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.CustomerId),

                _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.CustomerId)
            };

            // ================= PAGINATION =================
            var customers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode,
                    Name = x.Name,
                    Company = x.Company,
                    Phone = x.Phone,
                    Email = x.Email,
                    City = x.City,
                    CreditLimit = x.CreditLimit,
                    OutstandingBalance = x.OutstandingBalance,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    created_at = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            // ================= RESPONSE =================
            return Ok(new
            {
                page,
                pageSize,
                totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                data = customers
            });
        }

        // =====================================
        // GET CUSTOMER BY ID
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
                return NotFound(new
                {
                    success = false,
                    message = "Customer not found"
                });

            var latestNote = await _context.CustomerActivities
                .Where(x => x.CustomerId == id && x.ActivityType == "NOTE")
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Description)
                .FirstOrDefaultAsync();

            return Ok(await CustomerResponseAsync(customer, latestNote));
        }

        // =====================================
        // CREATE CUSTOMER
        // =====================================
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDto dto)
        {
            var city = string.IsNullOrWhiteSpace(dto.City) ? Clean(dto.Address) : Clean(dto.City);
            var taxNumber = CustomerDto.NormalizeGstNumber(
                string.IsNullOrWhiteSpace(dto.GstNumber) ? dto.TaxNumber : dto.GstNumber);
            var notes = Clean(dto.Notes);
            var requestedStatus = NormalizeStatus(dto.Status);
            var normalizedEmail = NormalizeEmail(dto.Email);
            var normalizedPhone = CustomerDto.NormalizePhone(dto.Phone);
            var duplicateErrors = await GetDuplicateErrorsAsync(normalizedEmail, normalizedPhone, string.Empty);

            if (duplicateErrors.Count > 0)
            {
                return ValidationError(duplicateErrors);
            }

            await EnsureCustomerMasterSchemaAsync();

            const int maxCustomerCodeAttempts = 3;
            for (var attempt = 1; attempt <= maxCustomerCodeAttempts; attempt++)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                try
                {
                    var temporaryCustomerCode = $"TMP-{Guid.NewGuid():N}";

                    var customer = new Customer
                    {
                        CustomerCode = temporaryCustomerCode,
                        Name = Clean(dto.Name),
                        Company = Clean(dto.Company),
                        City = city,
                        GstNumber = taxNumber,
                        PanNumber = Clean(dto.PanNumber),
                        Phone = normalizedPhone,
                        Email = normalizedEmail,
                        CreditLimit = dto.CreditLimit,
                        OutstandingBalance = dto.OutstandingBalance,
                        Status = AllowedCustomerStatuses.Contains(requestedStatus) ? requestedStatus : "Active",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                    customer.CustomerCode = await AllocateCustomerCodeFromIdAsync(customer.CustomerId);
                    await SaveCustomerMasterChildrenAsync(customer.CustomerId, dto, replaceExisting: false);

                    _context.CustomerActivities.Add(new CustomerActivity
                    {
                        CustomerId = customer.CustomerId,
                        ActivityType = "CREATE",
                        Description = $"Customer {customer.Name} created",
                        CreatedAt = DateTime.UtcNow
                    });

                    if (!string.IsNullOrWhiteSpace(notes))
                    {
                        _context.CustomerActivities.Add(new CustomerActivity
                        {
                            CustomerId = customer.CustomerId,
                            ActivityType = "NOTE",
                            Description = notes,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    await _context.SaveChangesAsync();

                    var responseData = await CustomerResponseAsync(customer, notes);
                    await transaction.CommitAsync();

                    try
                    {
                        await _auditLogService.LogAsync(
                            "CREATE_CUSTOMER",
                            "Customers",
                            customer.CustomerId,
                            $"Customer created: {customer.Name}",
                            "customers");
                    }
                    catch (Exception auditException)
                    {
                        _logger.LogError(
                            auditException,
                            "Customer created but audit logging failed. CustomerId={CustomerId}, CustomerCode={CustomerCode}, TraceId={TraceId}",
                            customer.CustomerId,
                            customer.CustomerCode,
                            HttpContext.TraceIdentifier);
                    }

                    return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, new
                    {
                        success = true,
                        message = "Customer created successfully",
                        data = responseData
                    });
                }
                catch (DbUpdateException exception) when (IsCustomerCodeUniqueViolation(exception) && attempt < maxCustomerCodeAttempts)
                {
                    await RollbackSafelyAsync(transaction);
                    _context.ChangeTracker.Clear();
                    _logger.LogWarning(
                        exception,
                        "Customer code collision during create. Retrying with next code. Attempt={Attempt}, TraceId={TraceId}",
                        attempt,
                        HttpContext.TraceIdentifier);
                    continue;
                }
                catch (DbUpdateException exception) when (IsCustomerCodeUniqueViolation(exception))
                {
                    await RollbackSafelyAsync(transaction);
                    _context.ChangeTracker.Clear();
                    _logger.LogError(
                        exception,
                        "Customer creation failed due to duplicate generated customer code after retries. InnerException={InnerException}, TraceId={TraceId}",
                        GetInnermostMessage(exception),
                        HttpContext.TraceIdentifier);

                    return Conflict(new
                    {
                        success = false,
                        message = "Customer code could not be allocated. Please try again.",
                        traceId = HttpContext.TraceIdentifier
                    });
                }
                catch (DbUpdateException exception)
                {
                    await RollbackSafelyAsync(transaction);
                    _context.ChangeTracker.Clear();
                    _logger.LogError(
                        exception,
                        "Customer creation database failure. InnerException={InnerException}, TraceId={TraceId}",
                        GetInnermostMessage(exception),
                        HttpContext.TraceIdentifier);

                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        success = false,
                        message = "Customer could not be saved because a database step failed.",
                        traceId = HttpContext.TraceIdentifier
                    });
                }
                catch (Exception exception)
                {
                    await RollbackSafelyAsync(transaction);
                    _context.ChangeTracker.Clear();
                    _logger.LogError(
                        exception,
                        "Customer creation failed. InnerException={InnerException}, TraceId={TraceId}",
                        GetInnermostMessage(exception),
                        HttpContext.TraceIdentifier);

                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        success = false,
                        message = "Customer could not be saved. No customer record was created.",
                        traceId = HttpContext.TraceIdentifier
                    });
                }
            }

            return Conflict(new
            {
                success = false,
                message = "Customer code could not be allocated. Please try again.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        // =====================================
        // UPDATE CUSTOMER
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            CustomerDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
                return NotFound(new
                {
                    success = false,
                    message = "Customer not found"
                });

            var city = string.IsNullOrWhiteSpace(dto.City) ? Clean(dto.Address) : Clean(dto.City);
            var taxNumber = CustomerDto.NormalizeGstNumber(
                string.IsNullOrWhiteSpace(dto.GstNumber) ? dto.TaxNumber : dto.GstNumber);
            var notes = Clean(dto.Notes);
            var requestedStatus = NormalizeStatus(dto.Status);
            var normalizedEmail = NormalizeEmail(dto.Email);
            var normalizedPhone = CustomerDto.NormalizePhone(dto.Phone);
            var duplicateErrors = await GetDuplicateErrorsAsync(normalizedEmail, normalizedPhone, string.Empty, id);

            if (duplicateErrors.Count > 0)
            {
                return ValidationError(duplicateErrors);
            }

            await EnsureCustomerMasterSchemaAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                customer.Name = Clean(dto.Name);
                customer.Company = Clean(dto.Company);
                customer.City = city;
                customer.GstNumber = taxNumber;
                customer.PanNumber = Clean(dto.PanNumber);
                customer.Phone = normalizedPhone;
                customer.Email = normalizedEmail;
                customer.CreditLimit = dto.CreditLimit;
                customer.OutstandingBalance = dto.OutstandingBalance;
                if (AllowedCustomerStatuses.Contains(requestedStatus))
                {
                    customer.Status = requestedStatus;
                }
                customer.UpdatedAt = DateTime.UtcNow;

                await SaveCustomerMasterChildrenAsync(customer.CustomerId, dto, replaceExisting: true);
                await _context.SaveChangesAsync();

                _context.CustomerActivities.Add(new CustomerActivity
                {
                    CustomerId = customer.CustomerId,
                    ActivityType = "UPDATE",
                    Description = $"Customer {customer.Name} updated",
                    CreatedAt = DateTime.UtcNow
                });

                if (!string.IsNullOrWhiteSpace(notes))
                {
                    _context.CustomerActivities.Add(new CustomerActivity
                    {
                        CustomerId = customer.CustomerId,
                        ActivityType = "NOTE",
                        Description = notes,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();

                var responseData = await CustomerResponseAsync(customer, notes);
                await transaction.CommitAsync();

                try
                {
                    await _auditLogService.LogAsync(
                        "UPDATE_CUSTOMER",
                        "Customers",
                        customer.CustomerId,
                        $"Customer updated: {customer.Name}",
                        "customers");
                }
                catch (Exception auditException)
                {
                    _logger.LogError(
                        auditException,
                        "Customer updated but audit logging failed. CustomerId={CustomerId}, CustomerCode={CustomerCode}, TraceId={TraceId}",
                        customer.CustomerId,
                        customer.CustomerCode,
                        HttpContext.TraceIdentifier);
                }

                return Ok(new
                {
                    success = true,
                    message = "Customer updated successfully",
                    data = responseData
                });
            }
            catch (DbUpdateException exception)
            {
                await RollbackSafelyAsync(transaction);
                _logger.LogError(
                    exception,
                    "Customer update database failure. CustomerId={CustomerId}, InnerException={InnerException}, TraceId={TraceId}",
                    id,
                    GetInnermostMessage(exception),
                    HttpContext.TraceIdentifier);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Customer could not be updated because a database step failed.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception exception)
            {
                await RollbackSafelyAsync(transaction);
                _logger.LogError(
                    exception,
                    "Customer update failed. CustomerId={CustomerId}, InnerException={InnerException}, TraceId={TraceId}",
                    id,
                    GetInnermostMessage(exception),
                    HttpContext.TraceIdentifier);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Customer could not be updated. No customer changes were saved.",
                    traceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateCustomerStatus(
            int id,
            CustomerStatusUpdateDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
                return NotFound(new
                {
                    success = false,
                    message = "Customer not found"
                });

            var nextStatus = NormalizeStatus(dto.Status);
            var currentStatus = NormalizeStatus(customer.Status ?? "Active");

            if (string.IsNullOrWhiteSpace(nextStatus) || !AllowedCustomerStatuses.Contains(nextStatus))
            {
                return ValidationError(new Dictionary<string, string[]>
                {
                    [nameof(CustomerStatusUpdateDto.Status)] = ["Select a valid customer status."]
                });
            }

            if (nextStatus.Equals(currentStatus, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationError(new Dictionary<string, string[]>
                {
                    [nameof(CustomerStatusUpdateDto.Status)] = ["Please select a different status."]
                });
            }

            var reason = Clean(dto.Reason);
            var description = string.IsNullOrWhiteSpace(reason)
                ? $"Customer status changed from {currentStatus} to {nextStatus}"
                : $"Customer status changed from {currentStatus} to {nextStatus}. Reason: {reason}";

            customer.Status = nextStatus;
            customer.UpdatedAt = DateTime.UtcNow;

            _context.CustomerActivities.Add(new CustomerActivity
            {
                CustomerId = customer.CustomerId,
                ActivityType = "STATUS",
                Description = description,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CUSTOMER_STATUS_CHANGED",
                "Customers",
                customer.CustomerId,
                description,
                "customers");

            return Ok(new
            {
                success = true,
                message = "Customer status updated successfully.",
                data = CustomerResponse(customer)
            });
        }

        // =====================================
        // DELETE CUSTOMER
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
                return NotFound(new
                {
                    success = false,
                    message = "Customer not found"
                });

            var hasTransactions =
                await _context.Invoices.AsNoTracking().AnyAsync(x => x.CustomerId == id) ||
                await _context.CustomerPayments.AsNoTracking().AnyAsync(x => x.CustomerId == id) ||
                await _context.CustomerLedgers.AsNoTracking().AnyAsync(x => x.CustomerId == id) ||
                await _context.SalesOrders.AsNoTracking().AnyAsync(x => x.CustomerId == id);

            if (hasTransactions)
            {
                return Conflict(new
                {
                    success = false,
                    message = "This customer cannot be deleted because invoices, payments, or transactions exist."
                });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                await _context.CustomerActivities.Where(x => x.CustomerId == id).ExecuteDeleteAsync();
                await _context.CustomerContacts.Where(x => x.CustomerId == id).ExecuteDeleteAsync();
                await _context.CustomerAddresses.Where(x => x.CustomerId == id).ExecuteDeleteAsync();
                await _context.CustomerBankDetails.Where(x => x.CustomerId == id).ExecuteDeleteAsync();
                await _context.CustomerPaymentTerms.Where(x => x.CustomerId == id).ExecuteDeleteAsync();

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException exception)
            {
                await RollbackSafelyAsync(transaction);
                _logger.LogError(
                    exception,
                    "Customer delete database failure. CustomerId={CustomerId}, InnerException={InnerException}, TraceId={TraceId}",
                    id,
                    GetInnermostMessage(exception),
                    HttpContext.TraceIdentifier);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Customer could not be deleted because a database step failed.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            try
            {
                await _auditLogService.LogAsync(
                    "DELETE_CUSTOMER",
                    "Customers",
                    id,
                    $"Customer deleted: {customer.Name}",
                    "customers");
            }
            catch (Exception auditException)
            {
                _logger.LogError(
                    auditException,
                    "Customer deleted but audit logging failed. CustomerId={CustomerId}, TraceId={TraceId}",
                    id,
                    HttpContext.TraceIdentifier);
            }

            return Ok(new
            {
                success = true,
                message = "Customer deleted successfully"
            });
        }

        // =====================================
        // CUSTOMER SUMMARY
        // =====================================
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalCustomers =
                await _context.Customers.CountAsync();
            var activeCustomers =
                await _context.Customers.CountAsync(x => x.Status == null || x.Status.ToLower() == "active");
            var newCustomerCutoff = DateTime.UtcNow.AddDays(-30);
            var newCustomers =
                await _context.Customers.CountAsync(x => x.CreatedAt >= newCustomerCutoff);
            var outstandingReceivables =
                await _context.Customers.SumAsync(x => x.OutstandingBalance);
            var totalCreditLimit =
                await _context.Customers.SumAsync(x => x.CreditLimit);
            var creditUtilization = totalCreditLimit > 0
                ? Math.Round((outstandingReceivables / totalCreditLimit) * 100, 2)
                : 0;

            return Ok(new
            {
                totalCustomers,
                activeCustomers,
                repeatCustomers = 0,
                newCustomers,
                outstandingReceivables,
                creditUtilization,
                customerGrowth = 0
            });
        }

        // =====================================
        // CUSTOMER HISTORY
        // =====================================

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetCustomerHistory(int id)
        {
            var history = await _context.CustomerActivities
                .Where(x => x.CustomerId == id)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(history);
        }
    }
}
