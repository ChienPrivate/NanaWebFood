using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NanaFoodDAL.Model
{

    public class Coupon
    {
        [Key]
        public string CouponCode { get; set; }
        public double Discount { get; set; }// Giá trị giảm giá
        public string Description { get; set; }
        public double MinAmount { get; set; }  // Giá trị đơn hàng tối thiểu
        public DateTime CouponStartDate { get; set; } 
        public DateTime EndStart { get; set; }
        public int MaxUsage { get; set; } // Số lần sử dụng tối đa của mã giảm giá
        public int TimesUsed { get; set; } = 0;  // Số lần mã giảm giá đã được sử dụng
        public CouponStatus Status { get; set; }
        public List<UserCoupon> UserCoupons { get; set; }   
      
    }
   
    public enum CouponStatus
    {
        Active, //Mã đang có hiệu lực, sẵn sàng sử dụng.
        Inactive, //Mã đang tạm thời không hoạt động, sẽ được kích hoạt sau
        Delete, //Mã đã bị xóa khỏi hệ thống, không còn hiển thị cho người dùng.
        Expired //Mã đã hết hạn, không còn hiệu lực.
    }


}

