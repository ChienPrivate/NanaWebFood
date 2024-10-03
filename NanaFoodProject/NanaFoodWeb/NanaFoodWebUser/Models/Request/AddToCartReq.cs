namespace StoreManagement.Models.Request
{
    public class AddToCartReq
    {
        public string CustomerCd { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
