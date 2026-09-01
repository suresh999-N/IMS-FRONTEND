using Microsoft.AspNetCore.Mvc;
using IMSBackend.Data;
using IMSBackend.Models;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/attributes")]
    public class ProductAttributeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductAttributeController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _context.Attributes.ToList();
            return Ok(data);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var attribute = _context.Attributes.Find(id);

            if (attribute == null)
                return NotFound("Attribute not found");

            return Ok(attribute);
        }

        // ✅ CREATE
        [HttpPost]
        public IActionResult Create(ProductAttributeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            var attribute = new ProductAttribute
            {
                Name = dto.Name
            };

            _context.Attributes.Add(attribute);
            _context.SaveChanges();

            return Ok(attribute);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductAttributeDto dto)
        {
            var attribute = _context.Attributes.Find(id);

            if (attribute == null)
                return NotFound("Attribute not found");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            attribute.Name = dto.Name;

            _context.SaveChanges();

            return Ok(attribute);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var attribute = _context.Attributes.Find(id);

            if (attribute == null)
                return NotFound("Attribute not found");

            _context.Attributes.Remove(attribute);
            _context.SaveChanges();

            return Ok("Attribute deleted successfully");
        }
    }
}