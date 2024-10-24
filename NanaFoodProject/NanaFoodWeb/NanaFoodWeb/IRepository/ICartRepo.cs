using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICartRepo
    {
        public Task<ResponseDto> AddToCart(CartDetailsDto cartdetailsDto);
        public Task<ResponseDto> GetCart();

    }
}
