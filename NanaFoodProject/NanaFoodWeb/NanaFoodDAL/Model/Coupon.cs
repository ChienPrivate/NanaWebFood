using System.ComponentModel.DataAnnotations;

namespace NanaFoodDAL.Model
{
    public class Coupon
    {
        [Key]
        public string CouponCode { get; set; }
        public double Discount { get; set; }
        public double MinAmount { get; set; }
    }
}
