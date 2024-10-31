using NanaFoodWeb.Models.Dto;
using static NanaFoodWeb.Utility.StaticDetails;
using System.Runtime.Intrinsics.Arm;
using NanaFoodWeb.Models;

namespace NanaFoodWeb.IRepository.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IBaseService _baseService;
        public ReviewRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> GetOrderDetailsByOrderId(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Review/orderdetailsreview/{orderId}"
            });
        }

        public async Task<ResponseDto> GetOrderDetailsFromOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Review/orderdetailsInOrder/{orderId}"
            });
        }

        public async Task<ResponseDto> GetProducReview(int productId, int page, int pageSize)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Review/productreviews/{productId}/{page}/{pageSize}"
            });
        }

        public async Task<ResponseDto> GetProductRating(int productId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Review/GetRating/{productId}"
            });
        }

        public async Task<ResponseDto> PostReviewAsync(Review review)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = APIBase + $"/api/Review/review",
                Data = review
            });
        }
    }
}
