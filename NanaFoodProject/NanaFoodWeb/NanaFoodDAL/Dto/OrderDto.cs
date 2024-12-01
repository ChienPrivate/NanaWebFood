using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Không được để trống tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Không được để trống số điện thoại")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Không được để trống địa chỉ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Hãy Chọn phương thức thanh Toán")]
        public string PaymentType { get; set; }
        [Required(ErrorMessage = "Trạng thái thanh toán không được để trống")]
        public string PaymentStatus { get; set; }
        [Required(ErrorMessage = "Trạng thái giao hàng không được để trống")]
        public string OrderStatus { get; set; }
        [Required(ErrorMessage = "Phí ship không được để trống")]
        public int ShipmentFee { get; set; }
        public string? Note { get; set; }
        public string? CancelUserId { get; set; }
        public string? CancelUserName { get; set; }
        public string? CancelUserFullName { get; set; }
        public string? CancelUserRoles { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        [Required(ErrorMessage = "Mã người dùng không được để trống")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "KHông được để trống hóa đơn")]
        public double Total { get; set; }
        public string? CouponCode { get; set; }
        public double? Discount { get; set; }// Giá trị giảm giá
        public double? MinAmount { get; set; } // giá trị điều kiện cần để giảm
        [Required(ErrorMessage = "Ngày đặt hàng không được trống")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string ExpectedDeliveryDate { get; set; }
        public DateTime ReceiveDate { get; set; }
    }
}
