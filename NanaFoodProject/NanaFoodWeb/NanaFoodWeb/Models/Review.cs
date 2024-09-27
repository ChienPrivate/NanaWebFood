using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; } = Guid.NewGuid();
        public string Comment { get; set; }
        public double Rating { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
