using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NanaFoodWeb.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Không được để trống tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Không được để trống số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Không được để trống email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Không được để trống số nhà và tên đường")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Hãy Chọn phương thức thanh Toán")]
        public string PaymentType { get; set; }
        [Required(ErrorMessage = "Trạng thái thanh toán không được để trống")]
        public string PaymentStatus { get; set; } = "Chưa thanh toán";
        [Required(ErrorMessage = "Trạng thái giao hàng không được để trống")]
        public string OrderStatus { get; set; } = "Chờ xác nhận";
        public string? Note { get; set; }
        public string? CancelReason { get; set; }
        [Required(ErrorMessage = "Phí ship không được để trống")]
        public int ShipmentFee { get; set; }
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        public string UserId { get; set; } = "string";
        public double Total { get; set; } = 0;
        public string ExpectedDeliveryDate { get; set; }
        [Required(ErrorMessage = "Ngày đặt hàng không được trống")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ReceiveDate { get; set; }
        public string? CouponCode { get; set; }
        public double? Discount { get; set; }// Giá trị giảm giá
        public double? MinAmount { get; set; } // giá trị điều kiện cần để giảm
        [JsonIgnore]
        [Required(ErrorMessage = "Hãy chọn quận/huyện")]
        public string District { get; set; }
        [JsonIgnore]
        [Required(ErrorMessage = "Hãy chọn phường xã")]
        public string Ward { get; set; }
    }
}
