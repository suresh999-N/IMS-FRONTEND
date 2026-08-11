using IMSBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL LOGS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetLogs(
            int page = 1,
            int pageSize = 50,
            string? module = null,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.AuditLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(module) &&
                !module.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var normalizedModule = module.Trim().ToLower();
                query = normalizedModule switch
                {
                    "inventory" => query.Where(x =>
                        (x.Module ?? string.Empty).ToLower() == "inventory" ||
                        (x.Module ?? string.Empty).ToLower() == "stock" ||
                        (x.Action ?? string.Empty).ToLower().Contains("stock") ||
                        (x.Action ?? string.Empty).ToLower().Contains("goods_receipt")),
                    "payments" => query.Where(x =>
                        (x.Module ?? string.Empty).ToLower() == "payments" ||
                        (x.Action ?? string.Empty).ToLower().Contains("payment")),
                    "invoices" => query.Where(x =>
                        (x.Module ?? string.Empty).ToLower() == "sales" ||
                        (x.Action ?? string.Empty).ToLower().Contains("invoice")),
                    _ => query.Where(x =>
                        (x.Module ?? string.Empty).ToLower() == normalizedModule ||
                        (x.Action ?? string.Empty).ToLower().Contains(normalizedModule))
                };
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                query = query.Where(x =>
                    (x.Action ?? string.Empty).ToLower().Contains(normalizedSearch) ||
                    (x.Module ?? string.Empty).ToLower().Contains(normalizedSearch) ||
                    (x.Description ?? string.Empty).ToLower().Contains(normalizedSearch) ||
                    (x.TableName ?? string.Empty).ToLower().Contains(normalizedSearch));
            }

            var totalRecords = await query.CountAsync(cancellationToken);
            var auditLogs = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            var logs = auditLogs.Select(x => new
            {
                x.LogId,
                id = x.LogId,
                x.UserId,
                x.Action,
                x.Module,
                x.TableName,
                x.RecordId,
                x.Description,
               
                createdAt = DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc)
            });

            return Ok(new
            {
                page,
                pageSize,
                totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                data = logs
            });
        }

        // =========================
        // GET LOGS BY MODULE
        // =========================
        [HttpGet("module/{module}")]
        public async Task<IActionResult> GetLogsByModule(
            string module)
        {
            var logs = await _context.AuditLogs
                .Where(x =>
                    x.Module != null &&
                    x.Module.ToLower() ==
                    module.ToLower())
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }

        // =========================
        // GET LOGS BY USER
        // =========================
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLogsByUser(
            int userId)
        {
            var logs = await _context.AuditLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }
    }
}
