using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMSBackend.DTOs;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SystemSettingsController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================
        // GET SETTINGS
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (settings == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Settings not found"
                });
            }

            var dto = new SystemSettingsDto
            {
                CompanyName = settings.CompanyName,
                CompanyEmail = settings.CompanyEmail,
                CompanyPhone = settings.CompanyPhone,
                CompanyAddress = settings.CompanyAddress,

                CompanyLogo = settings.CompanyLogo,

                GstNumber = settings.GstNumber,
                Currency = settings.Currency,
                Timezone = settings.Timezone,

                AllowNegativeStock = settings.AllowNegativeStock,
                DefaultReorderLevel = settings.DefaultReorderLevel,
                StockValuationMethod = settings.StockValuationMethod,

                InvoicePrefix = settings.InvoicePrefix,
                InvoiceStartNumber = settings.InvoiceStartNumber,

                EnableAuditLogs = settings.EnableAuditLogs,
                AuditRetentionDays = settings.AuditRetentionDays,

                LowStockAlert = settings.LowStockAlert,

                DefaultUnitType = settings.DefaultUnitType,
                EnableBarcode = settings.EnableBarcode,
                AutoStockUpdate = settings.AutoStockUpdate,
                EmailNotifications = settings.EmailNotifications,
                LowStockNotifications = settings.LowStockNotifications,
                PurchaseNotifications = settings.PurchaseNotifications,
                SalesNotifications = settings.SalesNotifications,
                SystemAlerts = settings.SystemAlerts,

                EnableTwoFactorAuth = settings.EnableTwoFactorAuth,

                ThemeMode = settings.ThemeMode,
                Language = settings.Language,
                CollapseSidebar = settings.CollapseSidebar


            };

            return Ok(dto);
        }

        // =====================================
        // UPDATE SETTINGS
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSettings(
    int id,
    [FromBody] SystemSettingsDto updated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var settings = await _context.SystemSettings
                .FirstOrDefaultAsync(x => x.SettingId == id);

            if (settings == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Settings not found"
                });
            }


            // Company
            settings.CompanyName = updated.CompanyName;
            settings.CompanyEmail = updated.CompanyEmail;
            settings.CompanyPhone = updated.CompanyPhone;
            settings.CompanyAddress = updated.CompanyAddress;

            settings.CompanyLogo = updated.CompanyLogo;

            settings.GstNumber = updated.GstNumber;
            settings.Currency = updated.Currency;
            settings.Timezone = updated.Timezone;

            // Sales
            settings.InvoicePrefix = updated.InvoicePrefix;
            settings.InvoiceStartNumber = updated.InvoiceStartNumber;

            // Inventory
            settings.AllowNegativeStock = updated.AllowNegativeStock;
            settings.DefaultReorderLevel = updated.DefaultReorderLevel;
            settings.StockValuationMethod = updated.StockValuationMethod;
            settings.LowStockAlert = updated.LowStockAlert;


            settings.DefaultUnitType = updated.DefaultUnitType;
            settings.EnableBarcode = updated.EnableBarcode;
            settings.AutoStockUpdate = updated.AutoStockUpdate;


            // Notifications
            settings.EmailNotifications = updated.EmailNotifications;
            settings.LowStockNotifications = updated.LowStockNotifications;
            settings.PurchaseNotifications = updated.PurchaseNotifications;
            settings.SalesNotifications = updated.SalesNotifications;
            settings.SystemAlerts = updated.SystemAlerts;

            // Security
            Console.WriteLine($"Received TwoFactor: {updated.EnableTwoFactorAuth}");
            settings.EnableTwoFactorAuth = updated.EnableTwoFactorAuth;

            // Theme
            settings.ThemeMode = updated.ThemeMode;
            settings.Language = updated.Language;
            settings.CollapseSidebar = updated.CollapseSidebar;

            // Audit
            settings.EnableAuditLogs = updated.EnableAuditLogs;
            settings.AuditRetentionDays = updated.AuditRetentionDays;

            // Create Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = null,
                Action = "UPDATE",
                Module = "System Settings",
                TableName = "system_settings",
                RecordId = settings.SettingId,
                Description = "System settings updated by administrator",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "System settings updated successfully",
                data = settings
            });
        }


        [HttpPost("upload-logo/{id}")]
        public async Task<IActionResult> UploadLogo(int id, IFormFile file)
        {
            var settings = await _context.SystemSettings
                .FirstOrDefaultAsync(x => x.SettingId == id);

            if (settings == null)
                return NotFound("Settings not found");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/companylogos");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() +
                           Path.GetExtension(file.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            settings.CompanyLogo = "/companylogos/" + fileName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Company logo uploaded successfully",
                logoUrl = settings.CompanyLogo
            });
        }




        [HttpDelete("remove-logo/{id}")]
        public async Task<IActionResult> RemoveLogo(int id)
        {
            var settings = await _context.SystemSettings
                .FirstOrDefaultAsync(x => x.SettingId == id);

            if (settings == null)
                return NotFound("Settings not found");

            settings.CompanyLogo = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Company logo removed successfully"
            });
        }
    }
}