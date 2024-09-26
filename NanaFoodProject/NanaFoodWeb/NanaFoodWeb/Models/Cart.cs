using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    [Table(nameof(Cart))]
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Coupon))]
        public string CouponCode { get; set; }
        public Coupon Coupon { get; set; }
        public User User { get; set; }
        public List<CartDetails> CartDetails { get; set; }
    }
}
