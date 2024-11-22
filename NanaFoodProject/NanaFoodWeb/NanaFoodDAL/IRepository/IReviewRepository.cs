using NanaFoodDAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IReviewRepository
    {
        Task<ResponseDto> PostReviewAsync(ReviewDto reviewDto);
        Task<ResponseDto> GetAllReview();
        Task<ResponseDto> GetReviewByIdAsync(string id);
        Task<ResponseDto> GetReviewByUserId(string userId);
        Task<ResponseDto> GetReviewByProductId(int productId, int page, int pageSize);
        Task<ResponseDto> GetOrderDetailsByOrderId(int orderId);
        Task<ResponseDto> GetOrderDetailsFromOrder(int orderId);
        Task<double> CalculateAvgRating(int productId);
        Task<ResponseDto> UpdateOrderDetailsReviewState(int orderId, int productId, bool IsReviewState);
        Task<ResponseDto> GetReviewWithUser();
        Task<ResponseDto> ConfirmReview(string reviewId);
        Task<ResponseDto> GetReviewById(string reviewId);
        Task<ResponseDto> GetProductById(int productId);
    }
}
