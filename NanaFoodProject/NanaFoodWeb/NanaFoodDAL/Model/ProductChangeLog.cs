using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodDAL.Model
{
    [Table("ProductChangeLog")]
    public class ProductChangeLog
    {
        [Key]
        public Guid LogId { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public int View { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public string UpdatedBy { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
