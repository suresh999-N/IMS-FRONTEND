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
        public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
        {
            await DeduplicateUnitsAsync(cancellationToken);

            var queryable = _context.Units
                .AsNoTracking()
                .Where(unit => !unit.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                queryable = queryable.Where(unit =>
                    unit.Name.ToLower().Contains(term) ||
                    unit.ShortName.ToLower().Contains(term));
            }

            var units = await queryable
                .OrderBy(unit => unit.Name)
                .ToListAsync(cancellationToken);

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

            unit.Name = FormatUnitName(unit.Name);
            unit.ShortName = FormatUnitName(unit.ShortName);

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

            unit.Name = FormatUnitName(updated.Name);
            unit.ShortName = FormatUnitName(updated.ShortName);

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

        private async Task DeduplicateUnitsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var activeUnits = await _context.Units
                    .Where(u => !u.IsDeleted)
                    .ToListAsync(cancellationToken);

                var groups = activeUnits
                    .GroupBy(u => GetCanonicalUnitStem(u.Name))
                    .Where(g => g.Count() > 1)
                    .ToList();

                bool hasChanges = false;
                foreach (var group in groups)
                {
                    var mainUnit = group.OrderByDescending(u => _context.Products.Count(p => p.UnitId == u.UnitId && !p.IsDeleted))
                                        .ThenByDescending(u => u.Name.Length > 0 && char.IsUpper(u.Name[0]))
                                        .ThenBy(u => u.UnitId)
                                        .First();

                    var duplicates = group.Where(u => u.UnitId != mainUnit.UnitId).ToList();
                    foreach (var dup in duplicates)
                    {
                        var linkedProducts = await _context.Products
                            .Where(p => p.UnitId == dup.UnitId && !p.IsDeleted)
                            .ToListAsync(cancellationToken);

                        foreach (var product in linkedProducts)
                        {
                            product.UnitId = mainUnit.UnitId;
                        }

                        dup.IsDeleted = true;
                        hasChanges = true;
                    }

                    mainUnit.Name = FormatUnitName(mainUnit.Name);
                    mainUnit.ShortName = FormatUnitName(mainUnit.ShortName);
                }

                if (hasChanges)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unit deduplication background task encountered an issue.");
            }
        }

        private async Task<string?> ValidateUnit(
            Unit unit,
            int? unitId,
            CancellationToken cancellationToken)
        {
            var name = FormatUnitName(unit.Name);
            var shortName = FormatUnitName(unit.ShortName);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "Unit name is required.";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[A-Za-z\s\-°%()]+$"))
            {
                return "Name can contain only letters and spaces.";
            }

            if (string.IsNullOrWhiteSpace(shortName))
            {
                return "Unit abbreviation is required.";
            }

            var nameStem = GetCanonicalUnitStem(name);
            var shortNameStem = GetCanonicalUnitStem(shortName);

            var activeUnits = await _context.Units
                .AsNoTracking()
                .Where(u => !u.IsDeleted && u.UnitId != (unitId ?? 0))
                .ToListAsync(cancellationToken);

            var duplicateNameExists = activeUnits.Any(item =>
                GetCanonicalUnitStem(item.Name) == nameStem ||
                item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (duplicateNameExists)
            {
                return "Unit name already exists.";
            }

            var duplicateShortNameExists = activeUnits.Any(item =>
                GetCanonicalUnitStem(item.ShortName) == shortNameStem ||
                item.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));

            return duplicateShortNameExists
                ? "Unit abbreviation already exists."
                : null;
        }

        private static string GetCanonicalUnitStem(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var s = text.Trim().ToLowerInvariant();
            if (s.EndsWith("ies") && s.Length > 3)
            {
                s = s.Substring(0, s.Length - 3) + "y";
            }
            else if (s.EndsWith("es") && s.Length > 3 && (s.EndsWith("shes") || s.EndsWith("ches") || s.EndsWith("boxes") || s.EndsWith("xes")))
            {
                s = s.Substring(0, s.Length - 2);
            }
            else if (s.EndsWith("s") && !s.EndsWith("ss") && s.Length > 2)
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

        private static string FormatUnitName(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var str = text.Trim();
            var words = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                var w = words[i];
                if (w.Length > 0)
                {
                    words[i] = char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant();
                }
            }
            return string.Join(" ", words);
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
