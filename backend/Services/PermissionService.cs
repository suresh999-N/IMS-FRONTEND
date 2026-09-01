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

        /// <summary>
        /// Check whether a role has permission for a module and action.
        /// </summary>
        public async Task<bool> HasPermission(
    string roleName,
    string moduleKey,
    string action)
        {
            Console.WriteLine("========== Permission Check ==========");
            Console.WriteLine($"Role      : {roleName}");
            Console.WriteLine($"Module    : {moduleKey}");
            Console.WriteLine($"Action    : {action}");


            Console.WriteLine($"Role : {roleName}");
            Console.WriteLine($"Module : {moduleKey}");
            Console.WriteLine($"Action : {action}");


            var permission = await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.Role.RoleName == roleName &&
                    x.Module.ModuleKey == moduleKey);

            if (permission == null)
            {
                Console.WriteLine("Permission NOT FOUND");
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

        /// <summary>
        /// Returns all permissions for a role.
        /// </summary>
        public async Task<List<RolePermission>> GetPermissionsAsync(string roleName)
        {
            return await _context.RolePermissions
                .Include(x => x.Role)
                .Include(x => x.Module)
                .Where(x => x.Role.RoleName == roleName)
                .OrderBy(x => x.Module.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Returns permission for a single module.
        /// </summary>
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

        /// <summary>
        /// Returns all permissions using RoleId.
        /// (Future use)
        /// </summary>
        public async Task<List<RolePermission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                .Include(x => x.Module)
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.Module.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Checks permission using RoleId.
        /// (Future use)
        /// </summary>
        public async Task<bool> HasPermissionAsync(
            int roleId,
            string moduleKey,
             string action)
             {
            var permission = await _context.RolePermissions
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    x.Module.ModuleKey == moduleKey);

            if (permission == null)
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

        /// <summary>
        /// Update permissions for a role.
        /// </summary>
        public async Task UpdatePermissionsAsync(
            int roleId,
            List<RolePermission> permissions)
        {
            var existingPermissions = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            await _context.RolePermissions.AddRangeAsync(permissions);

            await _context.SaveChangesAsync();
        }
    }
}