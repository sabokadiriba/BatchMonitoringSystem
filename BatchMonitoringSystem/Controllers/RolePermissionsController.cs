using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.Data;
using Microsoft.CodeAnalysis.CodeActions;
using BatchMonitoringSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
namespace BatchMonitoringSystem.Controllers
{

    [Authorize(Policy = "AssignPermissionPolicy")]
    public class RolePermissionsController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RolePermissionsController(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        // GET: RoleActions/Assign
      public async Task<IActionResult> Assign()
{
    var roles = await _roleManager.Roles.ToListAsync();
    var permissions = await _context.Permissions.ToListAsync();

    if (roles == null || permissions == null)
    {
        // Handle the situation where roles or permissions could not be retrieved.
        // For example, you could return a NotFound or an appropriate error view.
        return NotFound();
    }

    var model = new AssignPermissionViewModel
    {
        Roles = roles,
        Permissions = permissions,
        SelectedPermissionIds = new List<int>() // Initialize to avoid null reference
    };

    return View(model);
}
        // POST: RoleActions/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignPermissionViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                if (model.SelectedRoleId == null || !model.SelectedPermissionIds.Any())
                {
                    ModelState.AddModelError(string.Empty, "Please select a role and at least one permission.");
                    model.Roles = await _roleManager.Roles.ToListAsync();
                    model.Permissions = await _context.Permissions.ToListAsync();
                    return View(model);
                }

                // Process assignment
                _context.RolePermissions.RemoveRange(
                    _context.RolePermissions
                        .Where(ra => ra.RoleId == model.SelectedRoleId));

                foreach (var permissionId in model.SelectedPermissionIds)
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = model.SelectedRoleId,
                        PermissionId = permissionId
                    };
                    _context.RolePermissions.Add(rolePermission);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Assign));
            }

            // Re-populate the view model if validation fails
            model.Roles = await _roleManager.Roles.ToListAsync();
            model.Permissions = await _context.Permissions.ToListAsync();
            return View(model);
        }

    }

}
