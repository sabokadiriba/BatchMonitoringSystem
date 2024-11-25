using System.Data;
using Microsoft.AspNetCore.Identity;
namespace BatchMonitoringSystem.Models
{
    public class ApplicationUser: IdentityUser<int>
    {
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public bool IsFirstLogin { get; set; }
        public string CretedUser { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Department Department { get; set; }


    }
}
