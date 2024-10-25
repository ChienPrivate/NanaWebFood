namespace NanaFoodWeb.Models.Dto.ViewModels
{
    public class OrderVM
    {
        public List<CartResponseDto> CartResponse { get; set; }
        public Order order { get; set; }
    }
}
