using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodWeb.Models
{
    [Table("OrderDetails")]
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
