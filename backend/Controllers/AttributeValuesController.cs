using IMSBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/attribute-values")]
    public class AttributeValuesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributeValuesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.AttributeValues
                .OrderBy(item => item.AttributeId)
                .ThenBy(item => item.Value)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("attribute/{attributeId:int}")]
        public async Task<IActionResult> GetByAttribute(int attributeId)
        {
            var data = await _context.AttributeValues
                .Where(item => item.AttributeId == attributeId)
                .OrderBy(item => item.Value)
                .ToListAsync();

            return Ok(data);
        }
    }
}
