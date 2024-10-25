namespace NanaFoodDAL.Dto
{
    public class CouponDto
    {
        public int CouponTypeId { get; set; }
        public string CouponCode { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }// Giá trị giảm giá
        public double MinAmount { get; set; }  // Giá trị đơn hàng tối thiểu
        public DateTime CouponStartDate { get; set; }
        public DateTime EndStart { get; set; }  // Ngày hết hạn
        public int MaxUsage { get; set; } = 0;
        public int TimesUsed { get; set; } = 0;  // Số lần mã giảm giá đã được sử dụng
    }
}
