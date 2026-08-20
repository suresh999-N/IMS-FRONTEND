using IMSBackend.Data;
using IMSBackend.Models;
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
        public async Task<IActionResult> GetBarcodes()
        {
            var data = await _context.Barcodes
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(data);
        }



        [HttpPost("generate")]
        public async Task<IActionResult> GenerateBarcode(
    int productId)
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


            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,

                Options = new EncodingOptions
                {
                    Height = 120,
                    Width = 350,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(product.Barcode);

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

            var folderPath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "barcodes");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName =
                Guid.NewGuid() + ".png";

            var filePath =
                Path.Combine(folderPath, fileName);

            bitmap.Save(filePath, ImageFormat.Png);

            var imageUrl =
                $"/uploads/barcodes/{fileName}";

            var barcode = new Barcode
            {
                ProductId = product.ProductId,
                CodeValue = product.Barcode,
                CodeType = "Barcode",
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Barcodes.Add(barcode);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Barcode generated successfully",
                data = barcode
            });
        }
    }
}
