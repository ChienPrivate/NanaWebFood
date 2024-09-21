using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
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
