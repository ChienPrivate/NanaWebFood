using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICouponRepo
    {
        Task<ResponseDto> GetAll();
        Task<ResponseDto> Create(Coupon coupon);
        Task<ResponseDto> GetById(string id);
        Task<ResponseDto> Update(CouponDto coupon);
        Task<ResponseDto> ModifyStatus(string id);
        Task<ResponseDto> DeleteById(string id);
        Task<ResponseDto> CheckUserCoupon(string codeCoupon);
    }
}
