namespace NanaFoodWeb.Models.Dto
{
    public class ReviewProductDto
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string? ProductImage { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public string? Comment { get; set; }
        public double? Rating { get; set; }
        public string UserImageUrl { get; set; }
        public string UserFullName { get; set; }
        public bool IsReviewed { get; set; }
    }
}
