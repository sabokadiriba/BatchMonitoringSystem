using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchMonitoringSystem.Models
{
    public class UserActivityLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; }

        [StringLength(255)]
        public string ActivityDescription { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }

}
