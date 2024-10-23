using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;
using static NanaFoodWeb.Utility.StaticDetails;

namespace NanaFoodWeb.IRepository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IBaseService _baseService;
        public OrderRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> CalculateShippingFees(CalculateShippingFeeRequestDto requestDto)
        {

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = ShipppingFeeCaculateEndPoint,
                Data = requestDto
            }, false, "token");
        }

        public async Task<ResponseDto> GetAvailableServiceAsync(int from_district, int todistrict)
        {
            var requestBody = new
            {
                shop_id = 5406185,
                from_district = from_district,
                to_district = todistrict
            };

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = AvailableServiceEndPoint,
                Data = requestBody
            }, false, "token");

        }

        public async Task<ResponseDto> GetDistrictAsync(int provinceId)
        {
            var requestBody = new
            {
                province_id = provinceId
            };

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = DistrictEndPoint,
                Data = requestBody
            }, false, "token");
        }

        public async Task<ResponseDto> GetProvinceAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = ProvinceEndPoint,
            }, false, "token");
        }

        public async Task<ResponseDto> GetWardAsync(int districtId)
        {
            var requestBody = new
            {
                district_id = districtId
            };

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = WardEndPoint,
                Data = requestBody
            }, false, "token");
        }
    }
}
