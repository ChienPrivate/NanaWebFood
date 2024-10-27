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
        [Required(ErrorMessage = "Không được để trống địa chỉ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Hãy Chọn phương thức thanh Toán")]
        public string PaymentType { get; set; }
        [Required(ErrorMessage = "Trạng thái thanh toán không được để trống")]
        public string PaymentStatus { get; set; } = "Đã thanh toán";
        [Required(ErrorMessage = "Trạng thái giao hàng không được để trống")]
        public string OrderStatus { get; set; } = "Đang chuẩn bị";
        public string? Note { get; set; }
        [Required(ErrorMessage = "Phí ship không được để trống")]
        public int ShipmentFee { get; set; }
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        public string UserId { get; set; } = "string";
        public double Total { get; set; } = 0;
        [Required(ErrorMessage = "Ngày đặt hàng không được trống")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ReceiveDate { get; set; }
    }
}
