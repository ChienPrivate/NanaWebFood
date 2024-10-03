using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class CartDetails:BaseModel
    {
        [Key]
        public int CartDtlId { get; set; }
        [ForeignKey("Items")]

        public int CartId { get; set; }
        [ForeignKey("Carts")]
        public int ItemId { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string CartCd { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public virtual Carts Cart { get; set; }
        public virtual Items Item { get; set; }
    }
}
