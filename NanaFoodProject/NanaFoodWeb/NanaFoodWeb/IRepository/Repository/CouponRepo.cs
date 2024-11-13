using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;

namespace NanaFoodWeb.IRepository.Repository
{
    public class CouponRepo : ICouponRepo
    {
        private readonly IBaseService _baseService;
        public CouponRepo(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> CheckUserCoupon(string codeCoupon)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Coupon/Check/{codeCoupon}"
            });
        }
        public async Task<ResponseDto> DeleteById(string id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.APIBase + $"/api/Coupon/Delete/{id}"
            });
        }
        public async Task<ResponseDto> ModifyStatus(string id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Coupon/DeleteStatus/{id}"
            });
        }


        public async Task<ResponseDto> Create(Coupon coupon)
        {
            return _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + $"/api/Coupon/create",
                Data = coupon

            }).Result ;
        }
        public async Task<ResponseDto> GetAll()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Coupon/GetAll"
            });
        }

        public async Task<ResponseDto> GetById(string id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Coupon/getbyId/{id}"
            });
        }

        public async Task<ResponseDto> Update(CouponDto coupon)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Coupon/update", 
                Data = coupon
            });
        }

       
    }
}
