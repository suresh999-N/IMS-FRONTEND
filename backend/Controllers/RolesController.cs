using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMSBackend.DTOs;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL ROLES
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .OrderBy(x => x.RoleName)
                .ToListAsync();

            return Ok(roles);
        }

        // =========================
        // GET ROLE BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (role == null)
                return NotFound(new
                {
                    message = "Role not found"
                });

            return Ok(role);
        }

        // =========================
        // CREATE ROLE
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            var exists = await _context.Roles
                .AnyAsync(x =>
                    x.RoleName.ToLower() ==
                    role.RoleName.ToLower());

            if (exists)
            {
                return BadRequest(new
                {
                    message = "Role already exists"
                });
            }

            role.CreatedAt = DateTime.UtcNow;

            _context.Roles.Add(role);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role created successfully"
            });
        }

        // =========================
        // UPDATE ROLE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(
            int id,
            Role updated)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (role == null)
                return NotFound(new
                {
                    message = "Role not found"
                });

            role.RoleName = updated.RoleName;
            role.Description = updated.Description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role updated successfully"
            });
        }




        



        // =========================
        // UPDATE ROLE STATUS
        // =========================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRoleStatus(
            int id,
            [FromBody] UpdateRoleStatusDto dto)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (role == null)
            {
                return NotFound(new
                {
                    message = "Role not found"
                });
            }

            role.IsActive = dto.IsActive;

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = null,
                Action = "ROLE_STATUS_CHANGE",
                Module = "Roles",
                TableName = "roles",
                RecordId = role.RoleId,
                Description =
                    $"Role {role.RoleName} status changed to {(dto.IsActive ? "Active" : "Inactive")}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role status updated successfully"
            });
        }




        // =========================
        // DELETE ROLE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (role == null)
                return NotFound(new
                {
                    message = "Role not found"
                });

            _context.Roles.Remove(role);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role deleted successfully"
            });
        }
    }
}