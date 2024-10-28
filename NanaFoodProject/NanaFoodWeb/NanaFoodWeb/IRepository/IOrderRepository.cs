using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Momo;
using NanaFoodWeb.Models.VNPay;

namespace NanaFoodWeb.IRepository
{
    public interface IOrderRepository
    {
        Task<ResponseDto> GetProvinceAsync();
        Task<ResponseDto> GetDistrictAsync(int provinceId);
        Task<ResponseDto> GetWardAsync(int districtId);
        Task<ResponseDto> GetAvailableServiceAsync(int fromDistrict, int toDistrict);
        Task<ResponseDto> CalculateShippingFees(CalculateShippingFeeRequestDto requestDto);
        Task<ResponseDto> AddOrderAsync(Order order);
        Task<ResponseDto> CancelOrderAsync(int orderId);
        Task<ResponseDto> MomoPayment(int total);
        string VNPayPayment(int total);
        Task<ResponseDto> CODPayment(Order order);
        Task<ResponseDto> GetOrderByIdAsync(int id);
        Task<ResponseDto> GetUserOrderIdAsync(string UserId);
        Task<ResponseDto> GetOrderDetailsAsync(int OrderId);
        Task<ResponseDto> CalculateProfitAsync();
        Task<ResponseDto> UpdateOrderStatus(int OrderId, string message);
    }
}
