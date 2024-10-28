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
        Task<ResponseDto> GetReviewByProductId(int productId);
        Task<ResponseDto> GetOrderDetailsByOrderId(int orderId);
        Task<double> CalculateAvgRating(int productId);
        Task<ResponseDto> UpdateOrderDetailsReviewState(int orderId, int productId, bool IsReviewState);
    }
}
