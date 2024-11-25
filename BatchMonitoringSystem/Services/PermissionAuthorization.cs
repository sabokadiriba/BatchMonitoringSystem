using BatchMonitoringSystem.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Services
{

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _context;

        public PermissionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                context.Fail();
                return;
            }


            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var rolePermissions = await _context.RolePermissions
                .Where(rp => userRoles.Contains(rp.RoleId) && rp.Permission.PermissionName == requirement.Permission)
                .AnyAsync();

            if (rolePermissions)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }


}
