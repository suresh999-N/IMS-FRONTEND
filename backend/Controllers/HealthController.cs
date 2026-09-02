using IMSBackend.Data;
using Microsoft.AspNetCore.Mvc;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var dbConnected = _context.Database.CanConnect();

                return Ok(new
                {
                    status = "Healthy",
                    database = dbConnected ? "Connected" : "Disconnected",
                    serverTime = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    error = ex.Message
                });
            }
        }
    }
}