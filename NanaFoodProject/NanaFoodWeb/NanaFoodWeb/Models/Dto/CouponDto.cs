using NanaFoodDAL.Model;

namespace NanaFoodWeb.Models.Dto
{
    public class CouponDto
    {
        public string CouponCode { get; set; }
        public double Discount { get; set; }// Giá trị giảm giá
        public string Description { get; set; }
        public double MinAmount { get; set; }  // Giá trị đơn hàng tối thiểu
        public DateTime CouponStartDate { get; set; }
        public DateTime EndStart { get; set; }
        public int MaxUsage { get; set; } // Số lần sử dụng tối đa của mã giảm giá
        public int TimesUsed { get; set; } = 0;  // Số lần mã giảm giá đã được sử dụng
        public CouponStatus Status { get; set; }
    }
}
