using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class UserCouponRepo(ApplicationDbContext context, IMapper mapper) : IUserCouponRepo
    {
        ApplicationDbContext _context = context;
        IMapper _mapper = mapper;
        ResponseDto response = new ResponseDto();
        public async Task<ResponseDto> ApplyCoupon(string userId, string codeCoupon)
        {
            try
            {
                var eCart = _context.CartDetails.Where(e => e.UserId == userId);
                var totalPay = eCart.Sum(x => x.Total);
                var euser = await _context.Users.FirstOrDefaultAsync(e => e.Id == userId);
                var ecoupon = await _context.Coupons.FirstOrDefaultAsync(e => e.CouponCode == codeCoupon);
                var existingUserCoupon = await _context.UserCoupons.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponCode == codeCoupon);
                if (euser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Người dùng không tồn tại.";
                    return response;
                }
                if (ecoupon.MinAmount > totalPay)
                {
                    response.IsSuccess = false;
                    response.Message = "Bạn chưa đủ điều kiện để sử dụng mã giảm giá.";
                    return response;
                }
                if (ecoupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không tồn tại.";
                    return response;
                }

                if (existingUserCoupon != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Bạn đã sử dụng mã giảm giá này rồi!";
                    return response;
                }

                if (ecoupon.MaxUsage > 0)
                {
                    ecoupon.TimesUsed++;
                    ecoupon.MaxUsage--;

                    if (ecoupon.MaxUsage < 0)
                    {
                        ecoupon.Status = CouponStatus.Expired;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không còn lượt sử dụng.";
                    return response;
                }
                if(ecoupon.Status == CouponStatus.Expired || ecoupon.Status == CouponStatus.Inactive)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá đã hết hạn hoặc chưa có hiệu lực.";
                    return response;
                }
                if (ecoupon.Status != CouponStatus.Active)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá hiện không khả dụng.";
                    return response;
                }

                var userCoupon = new UserCoupon
                {
                    UserId = userId,
                    CouponCode = codeCoupon,
                    AppliedAt = DateTime.Now
                };
                await _context.UserCoupons.AddAsync(userCoupon);
                await _context.SaveChangesAsync();
                response.Result = _mapper.Map<UserCouponDto>(userCoupon);
                response.IsSuccess = true;
                response.Message = "Mã giảm giá đã được áp dụng thành công.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message; 
            }
            return response;
        }
    }
}
