using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models
{
    public class CartDetails
    {
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

    }

}
