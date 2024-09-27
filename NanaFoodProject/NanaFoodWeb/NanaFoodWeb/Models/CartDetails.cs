
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanaFoodWeb.Models
{
    [Table("CartDetails")]
    [PrimaryKey(nameof(CartId), nameof(ProductId))]
    public class CartDetails
    {
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public double Total { get; set; }
        public int Quantity { get; set; }
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
