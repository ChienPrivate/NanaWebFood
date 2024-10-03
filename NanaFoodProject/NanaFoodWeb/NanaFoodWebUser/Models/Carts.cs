using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class Carts:BaseModel
    {
        [Key]
        public int CartId { get; set; }
        [ForeignKey("Guest")]
        public int GuestId { get; set; }
        [ForeignKey("Customers")]
        public int CustomerId { get; set; }
        public string CustomerCd { get; set; }
        public string CustomerName { get; set; }
        public string CartCd { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal TotalMoney { get; set; }
        public virtual Customers Customer { get; set; }
        public virtual Guest Guest{ get; set; }
        public List<CartDetails> CartDetails { get; set; } = new List<CartDetails>();
    }
}
