using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DbInitializer
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;
    private readonly IConfiguration _configuration;

    public DbInitializer(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        ILogger<DbInitializer> logger,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        var adminUsername = _configuration["AdminUser:UserId"];
        var adminPassword = _configuration["AdminUser:Password"];
        var adminName = _configuration["AdminUser:Name"];

        try
        {
            // Ensure the database is created and up-to-date
            await _context.Database.MigrateAsync();

            // Seed Permissions
            var permissions = new[]
            {
                "RegisterUser",
                "View User",
                "Assign Role",
                "Revoke Role",
                "Modify User",
                "Create Department",
                "View Department",
                "Edit Department",
                "Delete Department",
                "Create Role",
                "View Role",
                "Assign Permission",
                "View Product",
                "Create Product",
                "Edit Product",
                "View Equipment",
                "Create Equipment",
                "Edit Equipment",
                "View Batch",
                "Create Batch",
                "Monitor Batch Status",
                "View User Activity Report",
                "Export User Activity Report",
                "View Product Report",
                "Export Product Report",
                "View Batch Status Report",
                "Export Batch Status Report",
                "View Backup Report",
                "Export Backup Report",
            };

            foreach (var permissionName in permissions)
            {
                if (!await _context.Permissions.AnyAsync(p => p.PermissionName == permissionName))
                {
                    _context.Permissions.Add(new Permission { PermissionName = permissionName });
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"{permissionName} permission created");
                }
            }

            // Seed Roles
            var roles = new[]
            {
                "SuperUser",
                "Admin",
                "Maintenance",
                "Operator",
            };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole(roleName));
                    _logger.LogInformation($"{roleName} role created");
                }
            }

            // Seed Admin User
            var adminUser = await _userManager.FindByNameAsync(adminUsername);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminUsername,
                    Name = adminName,
                    CreatedDate = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SuperUser");
                    _logger.LogInformation("Super user created and added to SuperUser role");

                    // Assign all permissions to SuperUser role
                    var superUserRole = await _roleManager.FindByNameAsync("SuperUser");
                    var permissionsList = await _context.Permissions.ToListAsync();

                    foreach (var permission in permissionsList)
                    {
                        if (!await _context.RolePermissions.AnyAsync(rp =>
                            rp.RoleId == superUserRole.Id && rp.PermissionId == permission.PermissionId))
                        {
                            _context.RolePermissions.Add(new RolePermission
                            {
                                RoleId = superUserRole.Id,
                                PermissionId = permission.PermissionId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"Error creating admin user: {error.Description}");
                    }
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while seeding the database: {ex.Message}");
            throw;
        }
    }
}
