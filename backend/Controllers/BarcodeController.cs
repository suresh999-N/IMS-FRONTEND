using IMSBackend.Data;
using IMSBackend.Models;
using IMSBackend.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BarcodeController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL BARCODES
        // =========================
        [HttpGet]
        [Permission("barcode", "view")]
        public async Task<IActionResult> GetBarcodes()
        {
            var data = await (from b in _context.Barcodes
                              join p in _context.Products on b.ProductId equals p.ProductId into productGroup
                              from prod in productGroup.DefaultIfEmpty()
                              orderby b.CreatedAt descending
                              select new
                              {
                                  b.BarcodeId,
                                  b.ProductId,
                                  ProductName = prod != null ? prod.Name : string.Empty,
                                  Code = !string.IsNullOrWhiteSpace(b.CodeValue)
                                      ? b.CodeValue
                                      : (prod != null
                                          ? (!string.IsNullOrWhiteSpace(prod.Barcode)
                                              ? prod.Barcode
                                              : (!string.IsNullOrWhiteSpace(prod.SKU)
                                                  ? prod.SKU
                                                  : $"PRD-{prod.ProductId}"))
                                          : string.Empty),
                                  CodeValue = !string.IsNullOrWhiteSpace(b.CodeValue)
                                      ? b.CodeValue
                                      : (prod != null
                                          ? (!string.IsNullOrWhiteSpace(prod.Barcode)
                                              ? prod.Barcode
                                              : (!string.IsNullOrWhiteSpace(prod.SKU)
                                                  ? prod.SKU
                                                  : $"PRD-{prod.ProductId}"))
                                          : string.Empty),
                                  CodeType = !string.IsNullOrWhiteSpace(b.CodeType) ? b.CodeType : "Barcode",
                                  b.ImageUrl,
                                  b.CreatedAt
                              }).ToListAsync();

            return Ok(data);
        }

        // =========================
        // GENERATE BARCODE / QR
        // =========================
        [HttpPost("generate")]
        [Permission("barcode", "add")]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task<IActionResult> GenerateBarcode(int productId, [FromQuery] string? codeType = "Barcode")
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.ProductId == productId &&
                    !x.IsDeleted);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            var normalizedType = string.Equals(codeType?.Trim(), "QR Code", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(codeType?.Trim(), "QR", StringComparison.OrdinalIgnoreCase)
                                 ? "QR Code"
                                 : "Barcode";

            var isQr = normalizedType == "QR Code";

            var writer = new BarcodeWriterPixelData
            {
                Format = isQr ? BarcodeFormat.QR_CODE : BarcodeFormat.CODE_128,

                Options = new EncodingOptions
                {
                    Height = isQr ? 250 : 120,
                    Width = isQr ? 250 : 350,
                    Margin = isQr ? 1 : 2
                }
            };

            var resolvedCode = !string.IsNullOrWhiteSpace(product.Barcode)
                ? product.Barcode
                : (!string.IsNullOrWhiteSpace(product.SKU)
                    ? product.SKU
                    : $"PRD-{product.ProductId}");

            var pixelData = writer.Write(resolvedCode);

            using var bitmap = new Bitmap(
                pixelData.Width,
                pixelData.Height,
                PixelFormat.Format32bppRgb);

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            try
            {
                Marshal.Copy(
                    pixelData.Pixels,
                    0,
                    bitmapData.Scan0,
                    pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "barcodes");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid() + ".png";

            var filePath = Path.Combine(folderPath, fileName);

            bitmap.Save(filePath, ImageFormat.Png);

            var imageUrl = $"/uploads/barcodes/{fileName}";

            // Remove previous code of the same type for this product
            var existingCodes = await _context.Barcodes
                .Where(b => b.ProductId == product.ProductId &&
                            (b.CodeType == normalizedType || (normalizedType == "Barcode" && (b.CodeType == null || b.CodeType == ""))))
                .ToListAsync();

            if (existingCodes.Any())
            {
                _context.Barcodes.RemoveRange(existingCodes);
            }

            var barcode = new Barcode
            {
                ProductId = product.ProductId,
                CodeValue = resolvedCode,
                CodeType = normalizedType,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Barcodes.Add(barcode);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"{normalizedType} generated successfully",
                data = new
                {
                    barcode.BarcodeId,
                    barcode.ProductId,
                    ProductName = product.Name,
                    Code = barcode.CodeValue,
                    CodeValue = barcode.CodeValue,
                    barcode.CodeType,
                    barcode.ImageUrl,
                    barcode.CreatedAt
                }
            });
        }
    }
}
