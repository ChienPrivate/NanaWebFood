using System.ComponentModel.DataAnnotations;

namespace NanaFoodWeb.Models.Dto
{
    public class CartDetailsDto
    {
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        public double? Total { get; set; }
    }
}
