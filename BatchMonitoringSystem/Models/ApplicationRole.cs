
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BatchMonitoringSystem.Models
{ 
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        // Define the navigation property for RolePermissions
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}
