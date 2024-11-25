using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Controllers
{
    public class UsersController : Controller
    {
       
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context
            )
        {
           
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        [Authorize(Policy = "ViewUserPolicy")]
        public async Task<IActionResult> Index()
        {
            // Fetch users and include related department information
            var users = await _userManager.Users.Include(u => u.Department).ToListAsync();

            // Retrieve all role names and store them in ViewBag
            var roleNames = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.Roles = roleNames;

            // Initialize a list to hold user view models
            var userViewModels = new List<ViewUserViewModel>();

            // Iterate through each user
            foreach (var user in users)
            {
                // Retrieve roles for the current user
                var userRoles = await _userManager.GetRolesAsync(user);

                // Add user information to the view model list
                userViewModels.Add(new ViewUserViewModel
                {
                    Id = user.Id,
                    Department = user.Department?.name ?? "No Department", // Ensure Department is not null
                    UserId = user.UserName,
                    Name = user.Name,
                    Roles = userRoles.ToList() // Convert roles to list
                });
            }

            // Return the view with user view models
            return View(userViewModels);
        }


        [Authorize(Policy = "AssignRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(roleName))
            {
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role does not exist.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return BadRequest("Failed to assign role.");
        }
        [Authorize(Policy = "RevokeRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> RevokeRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(roleName))
            {
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role does not exist.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return BadRequest("Failed to revoke role.");
        }

    }
}
