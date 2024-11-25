using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BatchMonitoringSystem.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _manager;
        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _manager = roleManager;
        }
        [Authorize(Policy = "ViewRolePolicy")]
        public IActionResult Index()
        {
            var roles = _manager.Roles;
            return View(roles);
        }

        [Authorize(Policy = "CreateRolePolicy")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IdentityRole role)
        {
            if (!_manager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _manager.CreateAsync(new ApplicationRole(role.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
    }
}
