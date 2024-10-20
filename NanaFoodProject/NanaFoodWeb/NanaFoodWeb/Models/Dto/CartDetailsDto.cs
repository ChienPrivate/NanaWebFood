using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class CartDetailsDto
    {
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        public decimal? Price { get; set; } = 0;
        public double? Total { get; set; }
    }
}
