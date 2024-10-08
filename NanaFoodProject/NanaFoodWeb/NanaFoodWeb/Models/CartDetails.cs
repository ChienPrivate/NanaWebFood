namespace NanaFoodWeb.Models
{
    public class CartDetails
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public double Total { get; set; }
        public int Quantity { get; set; }
    }
}
