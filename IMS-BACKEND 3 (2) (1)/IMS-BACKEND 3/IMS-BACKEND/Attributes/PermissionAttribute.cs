using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace IMSBackend.Attributes
{
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _module;
        private readonly string _action;

        public PermissionAttribute(string module, string action)
        {
            _module = module;
            _action = action;
        }

        public async Task OnAuthorizationAsync(
            AuthorizationFilterContext context)
        {
            var permissionService =
                context.HttpContext.RequestServices
                .GetRequiredService<PermissionService>();


            

            // Get role from JWT
            Console.WriteLine("=========== CLAIMS ===========");

            foreach (var c in context.HttpContext.User.Claims)
            {
                Console.WriteLine($"{c.Type} = {c.Value}");
            }

            var role = context.HttpContext.User
                .FindFirst(ClaimTypes.Role)
                ?.Value;

            Console.WriteLine($"Role from JWT = {role}");
            Console.WriteLine($"Module = {_module}");
            Console.WriteLine($"Action = {_action}");

            if (string.IsNullOrEmpty(role))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Console.WriteLine("===== ATTRIBUTE =====");
            Console.WriteLine($"Role   : {role}");
            Console.WriteLine($"Module : {_module}");
            Console.WriteLine($"Action : {_action}");

            var allowed = await permissionService.HasPermission(role, _module, _action);

            Console.WriteLine($"Allowed : {allowed}");

            if (!allowed)
            {
                Console.WriteLine("Returning 403");
                context.Result = new ForbidResult();
            }
        }
    }
}