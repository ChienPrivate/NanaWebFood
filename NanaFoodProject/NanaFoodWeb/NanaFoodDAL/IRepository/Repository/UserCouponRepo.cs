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
        public async Task<ResponseDto> ApplyCoupon(UserCoupon userCoupon)
        {
            try
            {
                var euser = await _context.Users.FirstOrDefaultAsync(e => e.Id == userCoupon.UserId);
                if (euser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Người dùng không tồn tại.";
                    return response;
                }
                var ecoupon = await _context.Coupons.FirstOrDefaultAsync(e => e.CouponCode == userCoupon.CouponCode);
                if (ecoupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không tồn tại.";
                    return response;
                }
                var existingUserCoupon = await _context.UserCoupons.FirstOrDefaultAsync(uc => uc.UserId == userCoupon.UserId && uc.CouponCode == userCoupon.CouponCode);

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
                if (DateTime.Now < ecoupon.CouponStartDate || DateTime.Now > ecoupon.EndStart)
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

                userCoupon.AppliedAt = DateTime.Now;
                await _context.UserCoupons.AddAsync(userCoupon);
                await _context.SaveChangesAsync();
                response.Result = _mapper.Map<UserCouponDto>(userCoupon);
                response.IsSuccess = true;
                response.Message = "Mã giảm giá đã được áp dụng thành công.";
            }
            catch (Exception ex) { }
            return response;
        }
    }
}
