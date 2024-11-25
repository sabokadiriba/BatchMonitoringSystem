using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.ViewModels
{
    public class ProductReportModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
        public IEnumerable<object> Parameters { get; internal set; }
    }
    public class ProductDto
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }
        public DateTime CreatedAt { get; set; } 
        public List<ProductParameterDto> Parameters { get; set; } = new List<ProductParameterDto>();
    }

    public class ProductParameterDto
    {
        [Required]
        [StringLength(100)]
        public string ParameterName { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }
    }



}
