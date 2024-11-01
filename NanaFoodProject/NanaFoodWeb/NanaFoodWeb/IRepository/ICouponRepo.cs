using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICouponRepo
    {
        Task<ResponseDto> GetAll();
        Task<ResponseDto> Create(Coupon coupon);
        Task<ResponseDto> GetById(string id);
        Task<ResponseDto> Update(Coupon coupon);
        Task<ResponseDto> DeleteById(string id);
        Task<ResponseDto> CheckUserCoupon(string codeCoupon);
    }
}
