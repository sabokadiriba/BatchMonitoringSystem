using System;
using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string EquipmentName { get; set; }

        [Required]
        [StringLength(15)]
        public string IP { get; set; }

        [Required]
        [StringLength(5)]
        public string Port { get; set; }

        [Required]
        [StringLength(50)]
        public string SqlUserName { get; set; }

        [Required]
        [StringLength(50)]
        public string SqlPassword { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
