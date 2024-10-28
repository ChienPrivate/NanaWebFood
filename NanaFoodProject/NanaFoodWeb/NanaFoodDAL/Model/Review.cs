using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodDAL.Model
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
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public DateTime ReviewedDate { get; set; } = DateTime.Now;
        public User User { get; set; }
        [ForeignKey("OrderId,ProductId")]
        public OrderDetails OrderDetails { get; set; }
    }
}
