using System.ComponentModel.DataAnnotations;

namespace BatchMonitoringSystem.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProductParameter> Parameters { get; set; } = new List<ProductParameter>();

    }

}
