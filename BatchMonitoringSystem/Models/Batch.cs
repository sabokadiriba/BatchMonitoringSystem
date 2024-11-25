
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace BatchMonitoringSystem.Models
    {
        public class Batch
        {
            [Key]
            public int BatchId { get; set; }
            [Required]
            public string BatchName { get; set; }
            [Required]
            public int EquipmentId { get; set; }
            public Equipment Equipment { get; set; }

            [Required]
            public int ProductId { get; set; }
            public Product Product { get; set; }

            [Required]
            public DateTime BatchStartTime { get; set; }

            [Required]
            public DateTime BatchEndTime { get; set; }

            public string Comments { get; set; } = string.Empty;
            public BatchStatus BatchStatus { get; set; } = BatchStatus.Pending;
            public DateTime CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
           public List<BatchParameter> BatchParameters { get; set; } = new List<BatchParameter>();
        }

        public enum BatchStatus
        {
            Passed,
            Failed,
            Pending
        }
    }


