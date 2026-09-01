
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Permissions/roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(x => new
                {
                    x.RoleId,
                    x.RoleName,
                    x.Description,
                    x.IsActive
                })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/Permissions/role/1
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(int roleId)
        {
            var role = await _context.Roles
                .Where(x => x.RoleId == roleId)
                .Select(x => new
                {
                    x.RoleId,
                    x.RoleName,
                    x.Description,
                    x.IsActive
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound("Role not found");
            }

            var permissions = await _context.RolePermissions
    .Include(x => x.Module)
    .Where(x => x.RoleId == roleId)
    .Select(x => new
    {
        x.PermissionId,
        x.RoleId,
        x.ModuleId,

        moduleKey = x.Module.ModuleKey,
        moduleName = x.Module.ModuleName,
        category = x.Module.Category,
        displayOrder = x.Module.DisplayOrder,

        x.CanView,
        x.CanAdd,
        x.CanEdit,
        x.CanDelete
    })
    .OrderBy(x => x.displayOrder)
    .ToListAsync();

            return Ok(new
            {
                role,
                permissions
            });
        }






        // =====================================
        // CLONE ROLE PERMISSIONS
        // =====================================
        [HttpPost("clone")]
        public async Task<IActionResult> ClonePermissions(
            [FromBody] ClonePermissionsDto dto)
        {
            var sourcePermissions = await _context.RolePermissions
                .Where(x => x.RoleId == dto.SourceRoleId)
                .ToListAsync();

            if (!sourcePermissions.Any())
            {
                return BadRequest(new
                {
                    message = "Source role has no permissions"
                });
            }

            var targetRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == dto.TargetRoleId);

            if (targetRole == null)
            {
                return NotFound(new
                {
                    message = "Target role not found"
                });
            }

            // Remove existing permissions
            var existingPermissions = await _context.RolePermissions
                .Where(x => x.RoleId == dto.TargetRoleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            // Clone permissions
            foreach (var permission in sourcePermissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = dto.TargetRoleId,
                    ModuleId = permission.ModuleId,
                    CanView = permission.CanView,
                    CanAdd = permission.CanAdd,
                    CanEdit = permission.CanEdit,
                    CanDelete = permission.CanDelete
                });
            }

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = null,
                Action = "CLONE_PERMISSIONS",
                Module = "Permissions",
                TableName = "role_permissions",
                RecordId = dto.TargetRoleId,
                Description =
                    $"Permissions copied from Role {dto.SourceRoleId} to Role {dto.TargetRoleId}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Permissions cloned successfully"
            });
        }





        [HttpPost("apply-template")]
        public async Task<IActionResult> ApplyTemplate(
    [FromBody] ApplyPermissionTemplateDto dto)
        {
            var permissions = await _context.RolePermissions
            .Include(x => x.Module)
             .Where(x => x.RoleId == dto.RoleId)
             .ToListAsync();

            if (!permissions.Any())
            {
                return NotFound(new
                {
                    message = "Permissions not found for role"
                });
            }

            foreach (var p in permissions)
            {
                switch (dto.Template.ToLower())
                {
                    case "admin":
                        p.CanView = true;
                        p.CanAdd = true;
                        p.CanEdit = true;
                        p.CanDelete = true;
                        break;

                    case "manager":
                        p.CanView = true;
                        p.CanAdd = true;
                        p.CanEdit = true;
                        p.CanDelete = false;

                        if (p.Module.ModuleKey == "users" ||
                        p.Module.ModuleKey == "systemSettings")
                        {
                            p.CanView = false;
                            p.CanAdd = false;
                            p.CanEdit = false;
                            p.CanDelete = false;
                        }
                        break;

                    case "staff":
                        p.CanView = true;
                        p.CanAdd = false;
                        p.CanEdit = false;
                        p.CanDelete = false;

                        if (p.Module.ModuleKey == "users" ||
                            p.Module.ModuleKey == "systemSettings" ||
                             p.Module.ModuleKey == "reports")
                        {
                            p.CanView = false;
                        }
                        break;

                    default:
                        return BadRequest(new
                        {
                            message = "Invalid template"
                        });
                }
            }

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = null,
                Action = "APPLY_TEMPLATE",
                Module = "Permissions",
                TableName = "role_permissions",
                RecordId = dto.RoleId,
                Description =
                    $"Applied {dto.Template} template to Role {dto.RoleId}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"{dto.Template} template applied successfully"
            });
        }





        // =====================================
        // RESET ROLE PERMISSIONS
        // =====================================
        [HttpPost("reset/{roleId}")]
        public async Task<IActionResult> ResetPermissions(int roleId)
        {
            var permissions = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();

            if (!permissions.Any())
            {
                return NotFound(new
                {
                    message = "No permissions found for role"
                });
            }

            foreach (var permission in permissions)
            {
                permission.CanView = false;
                permission.CanAdd = false;
                permission.CanEdit = false;
                permission.CanDelete = false;
            }

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = null,
                Action = "RESET_PERMISSIONS",
                Module = "Permissions",
                TableName = "role_permissions",
                RecordId = roleId,
                Description = $"All permissions reset for Role {roleId}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Permissions reset successfully"
            });
        }




        // PUT: api/Permissions/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePermissions(
    [FromBody] List<UpdatePermissionDto> permissions)
        {
            foreach (var item in permissions)
            {
                var permission = await _context.RolePermissions
     .Include(x => x.Module)
     .FirstOrDefaultAsync(x => x.PermissionId == item.PermissionId);

                if (permission != null)
                {
                    // Audit Log
                    _context.AuditLogs.Add(new AuditLog
                    {
                        UserId = null,
                        Action = "UPDATE",
                        Module = "Permissions",
                        TableName = "role_permissions",
                        RecordId = permission.PermissionId,
                        Description =
                            $"Permissions updated for {permission.Module.ModuleName}. " +
                            $"View:{permission.CanView}->{item.CanView}, " +
                            $"Add:{permission.CanAdd}->{item.CanAdd}, " +
                            $"Edit:{permission.CanEdit}->{item.CanEdit}, " +
                            $"Delete:{permission.CanDelete}->{item.CanDelete}",
                        CreatedAt = DateTime.UtcNow
                    });

                    permission.CanView = item.CanView;
                    permission.CanAdd = item.CanAdd;
                    permission.CanEdit = item.CanEdit;
                    permission.CanDelete = item.CanDelete;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Permissions updated successfully"
            });
        }

    }

    public class UpdatePermissionDto
    {
        public int PermissionId { get; set; }

        public bool CanView { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }
    }
}