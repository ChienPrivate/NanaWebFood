using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICartRepo
    {
        public Task<ResponseDto> AddToCart(CartDetailsDto cartdetailsDto);
        public Task<ResponseDto> GetCart();
        public Task<ResponseDto> ModifyCartQuantity(int producId, string message);
        public Task<ResponseDto> DeleteCartItem(int productId);
        public Task<ResponseDto> GetProductQuantity(int productId);

    }
}
