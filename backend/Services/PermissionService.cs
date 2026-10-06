using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Services
{
    public class PermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // CHECK PERMISSION BY ROLE NAME
        // =========================================================
        public async Task<bool> HasPermission(
            string roleName,
            string moduleKey,
            string action)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            if (string.Equals(roleName.Trim(), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var normalizedRole = roleName.Trim().ToLower();
            var normalizedModule = (moduleKey ?? "").Trim().ToLower();
            var altModule = normalizedModule.EndsWith("s")
                ? normalizedModule[..^1]
                : normalizedModule + "s";

            var permission = await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.Role.RoleName.ToLower() == normalizedRole &&
                    (x.Module.ModuleKey.ToLower() == normalizedModule || x.Module.ModuleKey.ToLower() == altModule));

            if (permission == null || !permission.Role.IsActive)
            {
                return false;
            }

            var result = action.Trim().ToLower() switch
            {
                "view" => permission.CanView,
                "add" or "create" => permission.CanAdd,
                "edit" or "update" => permission.CanEdit,
                "delete" => permission.CanDelete,
                _ => false
            };

            return result;
        }

        // =========================================================
        // GET ALL PERMISSIONS BY ROLE NAME
        // =========================================================
        public async Task<List<RolePermission>> GetPermissionsAsync(
            string roleName)
        {
            var list = await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .Where(x => x.Role.RoleName.ToLower() == (roleName ?? "").ToLower())
                .OrderBy(x => x.Module.DisplayOrder)
                .ToListAsync();

            if (string.Equals(roleName?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var p in list)
                {
                    p.CanView = true;
                    p.CanAdd = true;
                    p.CanEdit = true;
                    p.CanDelete = true;
                }
            }

            return list;
        }

        // =========================================================
        // GET SINGLE MODULE PERMISSION
        // =========================================================
        public async Task<RolePermission?> GetModulePermissionAsync(
            string roleName,
            string moduleKey)
        {
            return await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.Role.RoleName == roleName &&
                    x.Module.ModuleKey == moduleKey);
        }

        // =========================================================
        // GET PERMISSIONS BY ROLE ID
        // =========================================================
        public async Task<List<RolePermission>> GetPermissionsByRoleIdAsync(
            int roleId)
        {
            return await _context.RolePermissions
                .Include(x => x.Module)
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.Module.DisplayOrder)
                .ToListAsync();
        }

        // =========================================================
        // CHECK PERMISSION BY ROLE ID
        // =========================================================
        public async Task<bool> HasPermissionAsync(
            int roleId,
            string moduleKey,
            string action)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role != null && string.Equals(role.RoleName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var normalizedModule = (moduleKey ?? "").Trim().ToLower();
            var altModule = normalizedModule.EndsWith("s")
                ? normalizedModule[..^1]
                : normalizedModule + "s";

            var permission = await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    (x.Module.ModuleKey.ToLower() == normalizedModule || x.Module.ModuleKey.ToLower() == altModule));

            if (permission == null || !permission.Role.IsActive)
                return false;

            return action.Trim().ToLower() switch
            {
                "view" => permission.CanView,
                "add" or "create" => permission.CanAdd,
                "edit" or "update" => permission.CanEdit,
                "delete" => permission.CanDelete,
                _ => false
            };
        }

        // =========================================================
        // ENSURE PERMISSIONS FOR ONE ROLE
        // =========================================================
        public async Task EnsurePermissionsForRoleAsync(int roleId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId);
            var isAdmin = role != null && string.Equals(role.RoleName, "Admin", StringComparison.OrdinalIgnoreCase);

            // Get all active modules
            var modules = await _context.Modules
                .Where(x => x.IsActive)
                .ToListAsync();

            // Get modules that already have a permission
            // record for this role
            var existingModuleIds = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .Select(x => x.ModuleId)
                .ToListAsync();

            // Find modules without a permission record
            var missingModules = modules
                .Where(x => !existingModuleIds.Contains(x.ModuleId))
                .ToList();

            if (missingModules.Any())
            {
                var newPermissions = missingModules
                    .Select(module => new RolePermission
                    {
                        RoleId = roleId,
                        ModuleId = module.ModuleId,
                        CanView = isAdmin,
                        CanAdd = isAdmin,
                        CanEdit = isAdmin,
                        CanDelete = isAdmin
                    })
                    .ToList();

                await _context.RolePermissions
                    .AddRangeAsync(newPermissions);

                await _context.SaveChangesAsync();
            }

            if (isAdmin)
            {
                var adminPermissions = await _context.RolePermissions
                    .Where(x => x.RoleId == roleId)
                    .ToListAsync();

                bool changed = false;
                foreach (var p in adminPermissions)
                {
                    if (!p.CanView || !p.CanAdd || !p.CanEdit || !p.CanDelete)
                    {
                        p.CanView = true;
                        p.CanAdd = true;
                        p.CanEdit = true;
                        p.CanDelete = true;
                        changed = true;
                    }
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }

        // =========================================================
        // ENSURE PERMISSIONS FOR ALL EXISTING ROLES
        // =========================================================
        public async Task EnsurePermissionsForAllRolesAsync()
        {
            // Get all active roles
            var roles = await _context.Roles
                .Where(x => x.IsActive)
                .Select(x => x.RoleId)
                .ToListAsync();

            foreach (var roleId in roles)
            {
                await EnsurePermissionsForRoleAsync(roleId);
            }
        }

        // =========================================================
        // UPDATE PERMISSIONS FOR A ROLE
        // =========================================================
        public async Task UpdatePermissionsAsync(
            int roleId,
            List<RolePermission> permissions)
        {
            var existingPermissions = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            await _context.RolePermissions
                .AddRangeAsync(permissions);

            await _context.SaveChangesAsync();
        }
    }
}