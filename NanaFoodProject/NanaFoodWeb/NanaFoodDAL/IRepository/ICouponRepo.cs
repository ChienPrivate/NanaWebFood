using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository
{
    public interface ICouponRepo
    {
        Task<ResponseDto>GetAll();
        Task<ResponseDto> GetAvailableCoupon();
        Task<ResponseDto>Create(Coupon  coupon);
        Task<ResponseDto> GetById(string id);
        Task<ResponseDto> ModifyStatus(string id);
        Task<ResponseDto> Update(Coupon coupon); 
        Task<ResponseDto> DeleteById(string id);
        Task<ResponseDto> CheckUserCoupon(string userId, string codeCoupon);
    }
}
