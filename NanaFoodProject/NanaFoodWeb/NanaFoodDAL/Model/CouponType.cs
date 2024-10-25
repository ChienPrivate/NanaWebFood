using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    [Table("CouponType")]
    public class CouponType
    {

        [Key]
        public int CouponTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<Coupon> Coupons { get; set; }
    }
}
