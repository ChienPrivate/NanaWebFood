using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    public class Coupon
    {
        [Key]
        public string CouponCode { get; set; }
        public double Discount { get; set; }
        public double MinAmount { get; set; }
    }
}
