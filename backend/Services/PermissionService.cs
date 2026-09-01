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
            Console.WriteLine("========== Permission Check ==========");
            Console.WriteLine($"Role   : {roleName}");
            Console.WriteLine($"Module : {moduleKey}");
            Console.WriteLine($"Action : {action}");

            var permission = await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.Role.RoleName == roleName &&
                    x.Module.ModuleKey == moduleKey);

            if (permission == null || !permission.Role.IsActive)
            {
                Console.WriteLine("Permission NOT FOUND or Role INACTIVE");
                return false;
            }

            Console.WriteLine("Permission FOUND");
            Console.WriteLine($"DB Module : {permission.Module.ModuleKey}");
            Console.WriteLine($"CanView   : {permission.CanView}");

            var result = action.ToLower() switch
            {
                "view" => permission.CanView,
                "add" => permission.CanAdd,
                "edit" => permission.CanEdit,
                "delete" => permission.CanDelete,
                _ => false
            };

            Console.WriteLine($"Final Result = {result}");

            return result;
        }

        // =========================================================
        // GET ALL PERMISSIONS BY ROLE NAME
        // =========================================================
        public async Task<List<RolePermission>> GetPermissionsAsync(
            string roleName)
        {
            return await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .Where(x => x.Role.RoleName == roleName)
                .OrderBy(x => x.Module.DisplayOrder)
                .ToListAsync();
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
            var permission = await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    x.Module.ModuleKey == moduleKey);

            if (permission == null || !permission.Role.IsActive)
                return false;

            return action.ToLower() switch
            {
                "view" => permission.CanView,
                "add" => permission.CanAdd,
                "edit" => permission.CanEdit,
                "delete" => permission.CanDelete,
                _ => false
            };
        }

        // =========================================================
        // ENSURE PERMISSIONS FOR ONE ROLE
        // =========================================================
        public async Task EnsurePermissionsForRoleAsync(int roleId)
        {
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

            if (!missingModules.Any())
                return;

            var newPermissions = missingModules
                .Select(module => new RolePermission
                {
                    RoleId = roleId,
                    ModuleId = module.ModuleId,

                    // IMPORTANT:
                    // New permission records start with
                    // NO ACCESS.
                    CanView = false,
                    CanAdd = false,
                    CanEdit = false,
                    CanDelete = false
                })
                .ToList();

            await _context.RolePermissions
                .AddRangeAsync(newPermissions);

            await _context.SaveChangesAsync();
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