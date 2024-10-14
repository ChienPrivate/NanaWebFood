using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodDAL.Model
{
    [Table("Coupon")]
    public class Coupon
    {
        [Key]
        public string CouponCode { get; set; }
        public double Discount { get; set; }
        public double MinAmount { get; set; }
    }
}
