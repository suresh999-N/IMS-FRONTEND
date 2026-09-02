using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/units")]
    public class UnitController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitController> _logger;

        public UnitController(AppDbContext context, ILogger<UnitController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var units = await _context.Units
                .AsNoTracking()
                .Where(unit => !unit.IsDeleted)
                .OrderBy(unit => unit.Name)
                .ToListAsync();

            return Ok(ApiResponse<List<Unit>>.Ok(
                units,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var unit = await _context.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.UnitId == id && !item.IsDeleted, cancellationToken);

            if (unit == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Unit was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<Unit>.Ok(
                unit,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Unit unit, CancellationToken cancellationToken)
        {
            var validationError = await ValidateUnit(unit, null, cancellationToken);
            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                    traceId: HttpContext.TraceIdentifier));
            }

            unit.Name = NormalizeText(unit.Name);
            unit.ShortName = NormalizeText(unit.ShortName);

            try
            {
                _context.Units.Add(unit);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Unit create failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = unit.UnitId },
                ApiResponse<Unit>.Ok(
                    unit,
                    "Unit created successfully.",
                    HttpContext.TraceIdentifier));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Unit updated, CancellationToken cancellationToken)
        {
            var unit = await _context.Units.FindAsync(new object[] { id }, cancellationToken);
            if (unit == null || unit.IsDeleted)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Unit was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var validationError = await ValidateUnit(updated, id, cancellationToken);
            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                    traceId: HttpContext.TraceIdentifier));
            }

            unit.Name = NormalizeText(updated.Name);
            unit.ShortName = NormalizeText(updated.ShortName);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Unit update failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<Unit>.Ok(
                unit,
                "Unit updated successfully.",
                HttpContext.TraceIdentifier));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var unit = await _context.Units.FindAsync(new object[] { id }, cancellationToken);
            if (unit == null || unit.IsDeleted)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Unit was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var isInUse = await _context.Products
                .AsNoTracking()
                .AnyAsync(
                    product => product.UnitId == id && !product.IsDeleted,
                    cancellationToken);

            if (isInUse)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "This unit cannot be deleted because products are linked to it.",
                    traceId: HttpContext.TraceIdentifier));
            }

            try
            {
                unit.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(exception, "Unit delete failed.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetDbUpdateUserMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(
                null,
                "Unit deleted successfully.",
                HttpContext.TraceIdentifier));
        }

        private async Task<string?> ValidateUnit(
            Unit unit,
            int? unitId,
            CancellationToken cancellationToken)
        {
            var name = NormalizeText(unit.Name);
            var shortName = NormalizeText(unit.ShortName);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "Unit name is required.";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[A-Za-z\s]+$"))
            {
                return "Name can contain only letters and spaces.";
            }

            if (string.IsNullOrWhiteSpace(shortName))
            {
                return "Unit abbreviation is required.";
            }

            var normalizedName = name.ToLowerInvariant();
            var normalizedShortName = shortName.ToLowerInvariant();

            var duplicateNameExists = await _context.Units
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        !item.IsDeleted &&
                        item.UnitId != (unitId ?? 0) &&
                        item.Name.ToLower() == normalizedName,
                    cancellationToken);

            if (duplicateNameExists)
            {
                return "Unit name already exists.";
            }

            var duplicateShortNameExists = await _context.Units
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        !item.IsDeleted &&
                        item.UnitId != (unitId ?? 0) &&
                        item.ShortName.ToLower() == normalizedShortName,
                    cancellationToken);

            return duplicateShortNameExists
                ? "Unit abbreviation already exists."
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
                return "Unit name or abbreviation already exists.";
            }

            if (detail.Contains("foreign key", StringComparison.OrdinalIgnoreCase) ||
                detail.Contains("constraint", StringComparison.OrdinalIgnoreCase))
            {
                return $"Unit could not be saved because linked catalog data is invalid. {detail}";
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
