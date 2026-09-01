using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/productvariants")]
    public class ProductVariantController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductVariantController> _logger;

        public ProductVariantController(
            AppDbContext context,
            ILogger<ProductVariantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> Create(
            int productId,
            ProductVariantCreateDto dto,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.ProductId == productId && !item.IsDeleted,
                    cancellationToken);

            if (product == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid ProductId.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var validationError = ValidateVariantDraft(dto);
            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var variantSku = NormalizeSku(
                string.IsNullOrWhiteSpace(dto.SKU)
                    ? $"{product.SKU}-VAR-{DateTime.UtcNow:HHmmssfff}"
                    : dto.SKU);

            var duplicateSku = await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        item.ProductId == productId &&
                        item.SKU == variantSku,
                    cancellationToken);

            if (duplicateSku)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Variant SKU already exists for this product.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var variant = new ProductVariant
            {
                ProductId = productId,
                VariantName = string.IsNullOrWhiteSpace(dto.VariantName)
                    ? "Default"
                    : dto.VariantName.Trim(),
                SKU = variantSku,
                Price = (product.Price ?? 0) + (dto.PriceDelta ?? 0),
                CostPrice = product.CostPrice
            };

            try
            {
                _context.ProductVariants.Add(variant);

                await _context.SaveChangesAsync(cancellationToken);

                await UpsertVariantAttributes(
                    variant.VariantId,
                    dto.Attributes,
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(
                        exception.Message,
                        traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(
                    exception,
                    "Variant create failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<ProductVariant>.Ok(
                variant,
                "Variant created.",
                HttpContext.TraceIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var variants = await _context.ProductVariants
                .AsNoTracking()
                .Select(pv => new
                {
                    pv.VariantId,
                    pv.ProductId,
                    pv.VariantName,
                    pv.SKU,
                    pv.Price,
                    pv.CostPrice,

                    Attributes = _context.VariantAttributeValues
                        .Where(vav => vav.VariantId == pv.VariantId)
                        .Join(
                            _context.Attributes,
                            vav => vav.AttributeId,
                            a => a.AttributeId,
                            (vav, a) => a.Name)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                variants,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.VariantId == id,
                    cancellationToken);

            if (variant == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Variant was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<ProductVariant>.Ok(
                variant,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            ProductVariantCreateDto dto,
            CancellationToken cancellationToken)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(
                    item => item.VariantId == id,
                    cancellationToken);

            if (variant == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Variant was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item =>
                        item.ProductId == variant.ProductId &&
                        !item.IsDeleted,
                    cancellationToken);

            if (product == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Variant product was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var validationError = ValidateVariantDraft(dto);

            if (validationError != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationError,
                    traceId: HttpContext.TraceIdentifier));
            }

            var variantSku = NormalizeSku(
                string.IsNullOrWhiteSpace(dto.SKU)
                    ? variant.SKU
                    : dto.SKU);

            var duplicateSku = await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        item.ProductId == variant.ProductId &&
                        item.SKU == variantSku &&
                        item.VariantId != id,
                    cancellationToken);

            if (duplicateSku)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Variant SKU already exists for this product.",
                    traceId: HttpContext.TraceIdentifier));
            }

            variant.VariantName =
                string.IsNullOrWhiteSpace(dto.VariantName)
                    ? variant.VariantName
                    : dto.VariantName.Trim();

            variant.SKU = variantSku;

            variant.Price =
                (product.Price ?? 0) +
                (dto.PriceDelta ?? 0);

            variant.CostPrice = product.CostPrice;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                await UpsertVariantAttributes(
                    variant.VariantId,
                    dto.Attributes,
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(
                        exception.Message,
                        traceId: HttpContext.TraceIdentifier));
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(
                    exception,
                    "Variant update failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<ProductVariant>.Ok(
                variant,
                "Variant updated.",
                HttpContext.TraceIdentifier));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(
                    item => item.VariantId == id,
                    cancellationToken);

            if (variant == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Variant was not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            try
            {
                var attributes = await _context.VariantAttributeValues
                    .Where(item => item.VariantId == id)
                    .ToListAsync(cancellationToken);

                _context.VariantAttributeValues.RemoveRange(attributes);

                _context.ProductVariants.Remove(variant);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
            {
                LogDbUpdateException(
                    exception,
                    "Variant delete failed.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail(
                        GetInnermostMessage(exception),
                        traceId: HttpContext.TraceIdentifier));
            }

            return Ok(ApiResponse<object>.Ok(
                null,
                "Variant deleted.",
                HttpContext.TraceIdentifier));
        }

        private static string? ValidateVariantDraft(
            ProductVariantCreateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.VariantName) && !System.Text.RegularExpressions.Regex.IsMatch(dto.VariantName.Trim(), @"^[A-Za-z\s]+$"))
            {
                return "Name can contain only letters and spaces.";
            }

            if ((dto.PriceDelta ?? 0) < 0)
            {
                return "Variant price adjustment cannot be negative.";
            }

            if ((dto.StockDelta ?? 0) < 0)
            {
                return "Variant stock quantity cannot be negative.";
            }

            return null;
        }

        private static string NormalizeSku(string? sku)
            => sku?.Trim().ToUpperInvariant() ?? string.Empty;


        // ============================================================
        // Variant Attribute Handling
        // ============================================================

        private async Task UpsertVariantAttributes(
            int variantId,
            IReadOnlyCollection<VariantAttributeValueDto>? attributes,
            CancellationToken cancellationToken)
        {
            var desiredAttributes = (attributes ?? [])
                .Where(attribute => attribute.AttributeId > 0)
                .GroupBy(attribute => attribute.AttributeId)
                .Select(group => group.First())
                .ToList();

            var existingAttributes =
                await _context.VariantAttributeValues
                    .Where(item => item.VariantId == variantId)
                    .ToListAsync(cancellationToken);

            // No attributes requested.
            // Preserve the existing business behavior:
            // remove existing attributes.
            if (desiredAttributes.Count == 0)
            {
                if (existingAttributes.Count > 0)
                {
                    _context.VariantAttributeValues
                        .RemoveRange(existingAttributes);
                }

                return;
            }

            // Store the resolved ValueId separately.
            // This avoids modifying the DTO itself.
            var resolvedValueIds = new Dictionary<int, int>();

            foreach (var attribute in desiredAttributes)
            {
                int? valueId = attribute.ValueId;

                // Frontend currently sends AttributeId but may not
                // send ValueId during Edit.
                //
                // If ValueId is missing, use the existing ValueId
                // belonging to this Variant + Attribute.
                if (!valueId.HasValue)
                {
                    var existingAttribute = existingAttributes
                        .FirstOrDefault(
                            item =>
                                item.AttributeId ==
                                attribute.AttributeId);

                    if (existingAttribute != null)
                    {
                        valueId = existingAttribute.ValueId;
                    }
                }

                // If there is still no ValueId, we cannot create
                // a valid VariantAttributeValue.
                if (!valueId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"ValueId is required for attribute {attribute.AttributeId}.");
                }

                // Keep the existing business validation.
                var valueMatchesAttribute =
                    await _context.AttributeValues
                        .AsNoTracking()
                        .AnyAsync(
                            item =>
                                item.AttributeId ==
                                    attribute.AttributeId &&
                                item.ValueId == valueId.Value,
                            cancellationToken);

                if (!valueMatchesAttribute)
                {
                    throw new InvalidOperationException(
                        $"Invalid value {valueId.Value} for attribute {attribute.AttributeId}.");
                }

                resolvedValueIds[attribute.AttributeId] =
                    valueId.Value;
            }

            // Remove attributes that are no longer requested.
            foreach (var existingAttribute in existingAttributes)
            {
                var stillRequested = desiredAttributes.Any(
                    attribute =>
                        attribute.AttributeId ==
                        existingAttribute.AttributeId);

                if (!stillRequested)
                {
                    _context.VariantAttributeValues
                        .Remove(existingAttribute);
                }
            }

            // Add attributes that don't already exist.
            //
            // Existing attributes are intentionally not changed here,
            // preserving your original business behavior.
            foreach (var attribute in desiredAttributes)
            {
                var alreadyExists = existingAttributes.Any(
                    item =>
                        item.AttributeId ==
                        attribute.AttributeId);

                if (!alreadyExists)
                {
                    _context.VariantAttributeValues.Add(
                        new VariantAttributeValue
                        {
                            VariantId = variantId,
                            AttributeId = attribute.AttributeId,
                            ValueId =
                                resolvedValueIds[
                                    attribute.AttributeId]
                        });
                }
            }
        }


        private static string GetInnermostMessage(
            Exception exception)
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

        private void LogDbUpdateException(
            DbUpdateException exception,
            string message)
        {
            _logger.LogError(
                exception,
                "{Message} InnerException: {InnerException}. TraceId: {TraceId}",
                message,
                exception.InnerException?.ToString()
                    ?? "No inner exception",
                HttpContext.TraceIdentifier);
        }
    }
}