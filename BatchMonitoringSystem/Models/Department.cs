using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
    }
}
