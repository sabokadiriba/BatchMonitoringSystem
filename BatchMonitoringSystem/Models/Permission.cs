﻿using Microsoft.CodeAnalysis.CodeActions;

namespace BatchMonitoringSystem.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}
