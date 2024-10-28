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

        public Task<ResponseDto> Create(Coupon coupon)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Create(CouponType couponType)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> GetAll(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> GetAll(int page, int pageSize, bool isSelectAll = true)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> GetByCpTypeId(int id, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> GetById(string id, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Update(Coupon coupon)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Update(CouponType couponType)
        {
            throw new NotImplementedException();
        }
    }
}
