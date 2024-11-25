using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace BatchMonitoringSystem.ViewModels
{
    public class AssignPermissionViewModel
    {
        public IEnumerable<ApplicationRole> Roles { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }
        public int SelectedRoleId { get; set; }
        public IEnumerable<int> SelectedPermissionIds { get; set; }
    }
}
