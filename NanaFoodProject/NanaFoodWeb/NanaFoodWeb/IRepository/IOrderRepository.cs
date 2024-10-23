using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IOrderRepository
    {
        Task<ResponseDto> GetProvinceAsync();
        Task<ResponseDto> GetDistrictAsync(int provinceId);
        Task<ResponseDto> GetWardAsync(int districtId);
        Task<ResponseDto> GetAvailableServiceAsync(int fromDistrict, int toDistrict);
        Task<ResponseDto> CalculateShippingFees(CalculateShippingFeeRequestDto requestDto);
    }
}
