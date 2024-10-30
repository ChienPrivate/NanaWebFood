using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.Models.Dto;
using static NanaFoodWeb.Utility.StaticDetails;
using System.Runtime.Intrinsics.Arm;
using NanaFoodWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.IRepository
{
    public interface IReviewRepository
    {
        Task<ResponseDto> GetOrderDetailsByOrderId(int orderId);
        Task<ResponseDto> GetOrderDetailsFromOrder(int orderId);
        Task<ResponseDto> PostReviewAsync(Review review);
    }
}
