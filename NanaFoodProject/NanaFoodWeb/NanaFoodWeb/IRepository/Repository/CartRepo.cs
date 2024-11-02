using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;

namespace NanaFoodWeb.IRepository.Repository
{
    public class CartRepo : ICartRepo
    {
        private readonly IBaseService _baseService;
        public CartRepo(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> AddToCart(CartDetailsDto cartdetailsDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartdetailsDto,
                Url = StaticDetails.APIBase + $"/api/Cart/AddtoCart"
            });
        }

        public async Task<ResponseDto> GetCart()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Cart/GetCart"
            });
        }

        public async Task<ResponseDto> ModifyCartQuantity(int productId, string message)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Cart/UpdateCart/{productId}&{message}"
            });
        }

        public async Task<ResponseDto> DeleteCartItem(int productId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.APIBase + $"/api/Cart/deletecart/{productId}"
            });
        }
    }
}
