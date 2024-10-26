using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICouponRepo
    {
        #region Coupon
        Task<ResponseDto> GetAll(int page, int pageSize);
        Task<ResponseDto> Create(Coupon coupon);
        Task<ResponseDto> GetById(string id, int page, int pageSize);
        Task<ResponseDto> Update(Coupon coupon);
        Task<ResponseDto> DeleteById(string id);
        Task<ResponseDto> CheckUserCoupon(string codeCoupon);
        #endregion


        #region CouponType

        Task<ResponseDto> GetAll(int page, int pageSize, bool isSelectAll = true);
        Task<ResponseDto> Update(CouponType couponType);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto> Create(CouponType couponType);
        Task<ResponseDto> GetByCpTypeId(int id, int page, int pageSize);

        #endregion
    }
}
