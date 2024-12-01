using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.Model
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public int ShipmentFee { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string? Note { get; set; }
        public string? CancelUserId { get; set; }
        public string? CancelUserName { get; set; }
        public string? CancelUserFullName { get; set; }
        public string? CancelUserRoles { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ReceiveDate { get; set; }
        [ForeignKey(nameof(Coupon))]
        public string? CouponCode { get; set; }
        public double? Discount { get; set; }// Giá trị giảm giá
        public double? MinAmount { get; set; } // giá trị điều kiện cần để giảm
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        public Coupon? Coupon { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
