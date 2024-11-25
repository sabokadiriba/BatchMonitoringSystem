using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.Models
{
    public class tblActualData
    {
        [Key]
        public int ActualDataId { get; set; }
        public int BatchId { get; set; }
        public int EquipmentId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ParameterName { get; set; }
        public float? ActualValue { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Comments { get; set; } 
    }
}
