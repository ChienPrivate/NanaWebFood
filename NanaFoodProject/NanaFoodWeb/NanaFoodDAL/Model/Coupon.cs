using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
