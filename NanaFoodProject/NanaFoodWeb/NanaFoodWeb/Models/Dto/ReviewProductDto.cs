namespace NanaFoodWeb.Models.Dto
{
    public class ReviewProductDto
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string? ProductImage { get; set; }
    }
}
