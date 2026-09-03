using IMSBackend.Data;
using IMSBackend.Models;
using IMSBackend.DTOs;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PermissionService _permissionService;

        public RolesController(
            AppDbContext context,
            PermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // =========================================================
        // GET ALL ROLES
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .OrderBy(x => x.RoleName)
                .ToListAsync();

            return Ok(roles);
        }

        // =========================================================
        // GET ROLE BY ID
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
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

            return Ok(role);
        }

        // =========================================================
        // CREATE ROLE
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (role == null)
            {
                return BadRequest(new
                {
                    message = "Role data is required"
                });
            }

            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                return BadRequest(new
                {
                    message = "Role name is required"
                });
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(role.RoleName.Trim(), @"^[A-Za-z\s]+$"))
            {
                return BadRequest(new
                {
                    message = "Name can contain only letters and spaces."
                });
            }

            // Check duplicate role name
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

            // Set creation date
            role.CreatedAt = DateTime.UtcNow;

            // New roles should be active by default
            role.IsActive = true;

            // Save role first so RoleId is generated
            _context.Roles.Add(role);

            await _context.SaveChangesAsync();

            // =====================================================
            // IMPORTANT:
            // Automatically create permission records for this role
            // for every active module.
            //
            // All permissions start as FALSE.
            //
            // This works for ANY role name:
            // Admin, Manager, Salesman, Accountant, Cashier, etc.
            // =====================================================
            await _permissionService
                .EnsurePermissionsForRoleAsync(role.RoleId);

            return Ok(new
            {
                message = "Role created successfully",
                roleId = role.RoleId,
                roleName = role.RoleName
            });
        }

        // =========================================================
        // UPDATE ROLE
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(
            int id,
            Role updated)
        {
            if (updated == null)
            {
                return BadRequest(new
                {
                    message = "Role data is required"
                });
            }

            if (string.IsNullOrWhiteSpace(updated.RoleName))
            {
                return BadRequest(new
                {
                    message = "Role name is required"
                });
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (role == null)
            {
                return NotFound(new
                {
                    message = "Role not found"
                });
            }

            // Check whether another role already has this name
            var duplicate = await _context.Roles
                .AnyAsync(x =>
                    x.RoleId != id &&
                    x.RoleName.ToLower() ==
                    updated.RoleName.ToLower());

            if (duplicate)
            {
                return BadRequest(new
                {
                    message = "Another role with this name already exists"
                });
            }

            role.RoleName = updated.RoleName;
            role.Description = updated.Description;
            role.IsActive = updated.IsActive;

            await _context.SaveChangesAsync();

            // Make sure this role has permission rows
            // for any modules that may have been added later.
            await _permissionService
                .EnsurePermissionsForRoleAsync(role.RoleId);

            return Ok(new
            {
                message = "Role updated successfully"
            });
        }

        // =========================================================
        // UPDATE ROLE STATUS
        // =========================================================
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
                    $"Role {role.RoleName} status changed to " +
                    $"{(dto.IsActive ? "Active" : "Inactive")}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role status updated successfully"
            });
        }

        // =========================================================
        // DELETE ROLE
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
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

            // Delete permissions belonging to this role first.
            // This prevents foreign-key problems if the database
            // does not have cascade delete configured.
            var permissions = await _context.RolePermissions
                .Where(x => x.RoleId == id)
                .ToListAsync();

            if (permissions.Any())
            {
                _context.RolePermissions.RemoveRange(permissions);
            }

            // Delete the role
            _context.Roles.Remove(role);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role deleted successfully"
            });
        }
    }
}