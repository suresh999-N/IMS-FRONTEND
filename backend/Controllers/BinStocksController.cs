using IMSBackend.Data;
using IMSBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/bin-stocks")]
    public class BinStocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BinStocksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/bin-stocks
        [HttpGet]
        public IActionResult GetAll()
        {
            var result =
                from bs in _context.BinStocks
                join p in _context.Products
                    on bs.ProductId equals p.ProductId
                join w in _context.Warehouses
                    on bs.WarehouseId equals w.WarehouseId
                
                join b in _context.Bins
                    on bs.BinId equals b.BinId
                select new BinStockDetailsDto
                {
                    BinStockId = bs.BinStockId,

                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    VariantId = bs.VariantId,

                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.Name,

                    RackId = b.RackId,
                    RackCode = _context.Racks
    .Where(x => x.RackId == b.RackId)
    .Select(x => x.RackCode)
    .FirstOrDefault() ?? "",

                    BinId = b.BinId,
                    BinCode = b.BinCode ?? "",

                    Quantity = bs.Quantity
                };

            return Ok(result.ToList());
        }
    }
}
