namespace NanaFoodWeb.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int View { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
    }
}
