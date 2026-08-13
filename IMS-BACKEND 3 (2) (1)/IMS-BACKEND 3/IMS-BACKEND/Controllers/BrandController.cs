using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/brands")]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<BrandController> _logger;

        public BrandController(
            AppDbContext context,
            AuditLogService auditLogService,
            ILogger<BrandController> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var brands = await _context.Brands
                .AsNoTracking()
                .Where(brand => !brand.IsDeleted)
                .OrderByDescending(brand => brand.BrandId)
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<List<Brand>>.Ok(
                brands,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.BrandId == id && !item.IsDeleted, cancellationToken);

            if (brand == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Brand was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<Brand>.Ok(
                brand,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Brand brand, CancellationToken cancellationToken)
        {
            var validationError = await ValidateBrand(brand, null, cancellationToken);
            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                    traceId: HttpContext.TraceIdentifier));
            }

            brand.Name = NormalizeText(brand.Name);
            brand.Description = NormalizeText(brand.Description);

            try
            {
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "CREATE_BRAND",
                    "Brands",
                    brand.BrandId,
                    $"Brand {brand.Name} created",
                    "brands",
                    cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Brand create failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = brand.BrandId },
                ApiResponse<Brand>.Ok(
                    brand,
                    "Brand created successfully.",
                    HttpContext.TraceIdentifier));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Brand updated, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands.FindAsync(new object[] { id }, cancellationToken);
            if (brand == null || brand.IsDeleted)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Brand was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var validationError = await ValidateBrand(updated, id, cancellationToken);
            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                    traceId: HttpContext.TraceIdentifier));
            }

            brand.Name = NormalizeText(updated.Name);
            brand.Description = NormalizeText(updated.Description);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "UPDATE_BRAND",
                    "Brands",
                    brand.BrandId,
                    $"Brand {brand.Name} updated",
                    "brands",
                    cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Brand update failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<Brand>.Ok(
                brand,
                "Brand updated successfully.",
                HttpContext.TraceIdentifier));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands.FindAsync(new object[] { id }, cancellationToken);
            if (brand == null || brand.IsDeleted)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Brand was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var isInUse = await _context.Products
                .AsNoTracking()
                .AnyAsync(
                    product => product.BrandId == id && !product.IsDeleted,
                    cancellationToken);

            if (isInUse)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "This brand cannot be deleted because products are linked to it.",
                    traceId: HttpContext.TraceIdentifier));
            }

            try
            {
                brand.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(
                    "DELETE_BRAND",
                    "Brands",
                    brand.BrandId,
                    $"Brand {brand.Name} deleted",
                    "brands",
                    cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Brand delete failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(
                null,
                "Brand deleted successfully.",
                HttpContext.TraceIdentifier));
        }

        private async Task<string?> ValidateBrand(
            Brand brand,
            int? brandId,
            CancellationToken cancellationToken)
        {
            var name = NormalizeText(brand.Name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "Brand name is required.";
            }

            var normalizedName = name.ToLowerInvariant();
            var duplicateExists = await _context.Brands
                .AsNoTracking()
                .AnyAsync(
                        item =>
                        !item.IsDeleted &&
                        item.BrandId != (brandId ?? 0) &&
                        item.Name.ToLower() == normalizedName,
                    cancellationToken);

            return duplicateExists
                ? "Brand name already exists."
                : null;
        }

        private static string NormalizeText(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }

        private static string GetDbUpdateUserMessage(DbUpdateException exception)
        {
            var detail = GetInnermostMessage(exception);

            if (detail.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("unique", StringComparison.OrdinalIgnoreCase))
            {
                return "Brand name already exists.";
            }

            if (detail.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("constraint", StringComparison.OrdinalIgnoreCase))
            {
                return $"Brand could not be saved because linked catalog data is invalid. {detail}";
            }

            return detail;
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
    }
}
