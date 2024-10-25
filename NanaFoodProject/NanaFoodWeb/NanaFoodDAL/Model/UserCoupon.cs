using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace NanaFoodDAL.Model
{
    public class UserCoupon
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Coupon")]
        public string CouponCode { get; set; }
        public Coupon Coupon { get; set; }

        public DateTime AppliedAt { get; set; }
    }
}
