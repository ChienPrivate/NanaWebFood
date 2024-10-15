using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public double Price { get; set; }
        public int View { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public List<ProductChangeLog> ProductChangeLogs { get; set; }
        public List<Review> Reviews { get; set; }
        public List<CartDetails> CartDetails { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
        public List<WishList> WishLists { get; set; }
    }
}
