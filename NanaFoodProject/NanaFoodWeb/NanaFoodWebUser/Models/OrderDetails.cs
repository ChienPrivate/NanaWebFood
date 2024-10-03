using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models
{
    public class OrderDetails:BaseModel
    {
        public int OrderDtlId { get; set; }
        [ForeignKey("Orders")]
        public int OrderId { get; set; }
        [ForeignKey("ItemMenu")]
        public int ItemMenuId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public virtual Orders Order { get; set; }
        public virtual ItemMenu ItemMenu { get; set; }
    }
}
