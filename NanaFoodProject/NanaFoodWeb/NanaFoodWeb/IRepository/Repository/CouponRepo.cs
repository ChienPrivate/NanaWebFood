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
        public Task<ResponseDto> DeleteById(string id)
        {
            throw new NotImplementedException();
        }


        public async Task<ResponseDto> Create(Coupon coupon)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + $"/api/Coupon/createCP"
            });
        }
        public Task<ResponseDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Update(Coupon coupon)
        {
            throw new NotImplementedException();
        }

    }
}
