using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    [Table("Order")]
    public class Order
    {
        public int OrderId { get; set; }
        [Required(ErrorMessage = "")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "")]
        public string Address { get; set; }
        [Required(ErrorMessage = "")]
        public string PaymentType { get; set; }
        [Required(ErrorMessage = "")]
        public string PaymentStatus { get; set; }
        [Required(ErrorMessage = "")]
        public string OrderStatus { get; set; }
        [Required(ErrorMessage = "")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "")]
        public DateTime ReceiveDate { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
