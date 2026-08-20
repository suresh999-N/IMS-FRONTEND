using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.Models;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/variant-attributes")]
    public class VariantAttributeValueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VariantAttributeValueController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.VariantAttributeValues.ToList());
        }

        // ✅ GET BY VARIANT
        [HttpGet("variant/{variantId}")]
        public IActionResult GetByVariant(int variantId)
        {
            var data = _context.VariantAttributeValues
                .Where(x => x.VariantId == variantId)
                .ToList();

            return Ok(data);
        }

        // ✅ CREATE
        [HttpPost]
        public IActionResult Create(VariantAttributeValueDto dto, int variantId)
        {
            var entity = new VariantAttributeValue
            {
                VariantId = variantId,   // ✅ from request
                AttributeId = dto.AttributeId,
                ValueId = dto.ValueId
            };

            _context.VariantAttributeValues.Add(entity);
            _context.SaveChanges();

            return Ok(entity);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _context.VariantAttributeValues.Find(id);
            if (entity == null) return NotFound();

            _context.VariantAttributeValues.Remove(entity);
            _context.SaveChanges();

            return Ok("Deleted");
        }
    }
}