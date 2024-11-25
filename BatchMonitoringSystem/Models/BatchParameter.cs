using BatchMonitoringSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.Models
{
    public class BatchParameter
    {
        [Key]
        public int BatchParameterId { get; set; }

        [Required]
        public int BatchId { get; set; }
        public Batch Batch { get; set; }

        [Required]
        public string ParameterName { get; set; }

        // Store multiple actual values as a JSON string or serialized list
        public string ActualValuesJson { get; set; } = string.Empty;

     
        [Required]
        public bool IsWithinRange { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string? Comment { get; set; }
    }
}

