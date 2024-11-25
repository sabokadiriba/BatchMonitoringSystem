using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.Models
{
    public class ProductParameter
    {
        [Key]
        public int ProductParameterId { get; set; }
        [Required]
        [StringLength(100)]
        public string ParameterName { get; set; }

        public double MinValue { get; set; }


        public double MaxValue { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

}
