using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    [Table("Product")]
    internal class Product
    {
        [Key]
        public int ProductId { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public int View { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductChangeLog> ProductChangeLogs { get; set; } 
        public List<Review> Reviews { get; set; }
        public List<CartDetails> CartDetails { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
        public List<WishList> WishLists { get; set; }
    }
}
