﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class CouponRepo(ApplicationDbContext _context, IMapper _mapper) :ICouponRepo
    {
        readonly ApplicationDbContext context = _context;
        readonly IMapper mapper = _mapper; 
        ResponseDto response = new ResponseDto();

        public async Task<ResponseDto> CheckUserCoupon(string userId, string codeCoupon)
        {
            var eCart = context.CartDetails.Where(e => e.UserId == userId);
            var totalPay = eCart.Sum(x => x.Total);
            var eCoupon = context.Coupons.Find(codeCoupon);
            var eUsed = context.UserCoupons.FirstOrDefault(e=>e.UserId == userId && e.CouponCode == codeCoupon);
            if(eCoupon == null)
            {
                response.IsSuccess = false;
                response.Message = "Mã giảm giá không đúng.";
                return response;
            }
            if(eCoupon.MinAmount > totalPay)
            {
                response.IsSuccess = false;
                response.Message = "Bạn chưa đủ điều kiện để sử dụng mã giảm giá.";
                return response;
            }
            if(eCoupon.MaxUsage == 0)
            {
                response.IsSuccess = false;
                response.Message = "Mã giảm giá đã hết lượt sử dụng.";
                return response;
            }
            if ( eUsed is not null)
            {
                response.IsSuccess = false;
                response.Message = "Bạn đã sử dụng mã giảm giá này rồi.";
                return response;
            }
            response.IsSuccess = true;
            response.Message = "OK";
            return response;

        }

        public async Task<ResponseDto> Create(Coupon coupon)
        {
            try
            {
               /* coupon.CouponCode = coupon.CouponCode.ToLower();

                var couponType = await context.CouponTypes.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponTypeId == coupon.CouponTypeId);
                if (couponType == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại mã giảm giá không tồn tại.";
                    return response;
                }*/

                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không thể trước ngày bắt đầu.";
                    return response;
                }
                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không được lớn hơn 7 ngày so với ngày bắt đầu.";
                    return response;
                }

                if (coupon.MinAmount <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Giá trị đơn hàng tối thiểu không hợp lệ. Phải lớn hơn 0.";
                    return response;
                }
                if (coupon.MaxUsage <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Số lần sử dụng tối đa phải lớn hơn 0.";
                    return response;
                }
                var existingCoupon = await context.Coupons.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponCode == coupon.CouponCode);
                if (existingCoupon != null)
                {
                    if (existingCoupon.Status ==  CouponStatus.Block)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã giảm giá này đang bị khoá. Vui lòng tạo lại!";
                        return response;
                    }
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá này đã sử dụng.";
                    return response;
                }

                // Thiết lập trạng thái mã giảm giá
                CouponStatus couponStatus;
                var currentDate = DateTime.Now;

                // Kiểm tra trạng thái của mã giảm giá
                if (currentDate < coupon.CouponStartDate)
                {
                    couponStatus = CouponStatus.Inactive; // Chưa đến ngày bắt đầu
                }
                else if (currentDate >= coupon.CouponStartDate && currentDate <= coupon.EndStart)
                {
                    couponStatus = CouponStatus.Active; // Đang có hiệu lực
                }
                else
                {
                    couponStatus = CouponStatus.Expired; // Hết hạn
                }
                var newCoupon = new Coupon
                {
                    CouponCode = coupon.CouponCode,
                    Discount = coupon.Discount,
                    MinAmount = coupon.MinAmount,
                    CouponStartDate = coupon.CouponStartDate,
                    EndStart = coupon.EndStart,
                    TimesUsed = coupon.TimesUsed, //Số lượt đã dùng
                    MaxUsage = coupon.MaxUsage, // Số lần sử dụng tối đa
                    Description = coupon.Description, // Mô tả mã giảm giá
                    Status = couponStatus,
                    /*CouponTypeId = coupon.CouponTypeId,*/
                };

                await context.Coupons.AddAsync(newCoupon);
                await context.SaveChangesAsync();

                response.Result = mapper.Map<CouponDto>(newCoupon);
                response.IsSuccess = true;
                response.Message = "Tạo mã giảm giá thành công";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    

        public async Task<ResponseDto> DeleteById(string id)
        {
            try
            {
                var eCoupon = await context.Coupons.FirstOrDefaultAsync(e => e.CouponCode == id); 
                if(eCoupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không tồn tại"; 
                    return response;
                }
                context.Coupons.Remove(eCoupon);
                await context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message = "Xoá thành công";
            }
            catch(Exception ex) { response.IsSuccess = false; response.Message = ex.Message;}
            return response;
            
        }

        public async Task<ResponseDto> GetAll()
        {
            try 
            {
                var Cp = await context.Coupons.ToListAsync();
                if(Cp != null)
                {
                    response.IsSuccess = true;
                    response.Message = "Lấy sản phẩm thành công.";
                    response.Result = Cp; 
                }
            }
            catch (Exception e) { response.IsSuccess = false; response.Message = e.Message; }
            return response;
        }

        public async Task<ResponseDto> GetById(string code)
        {
            try
            {
                var coupon = await context.Coupons.FirstOrDefaultAsync(e => e.CouponCode == code); 

                if (coupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không tồn tại.";
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "Lấy mã giảm giá thành công.";
                response.Result = coupon;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<ResponseDto> Update(Coupon coupon)
        {
            try
            {
                coupon.CouponCode = coupon.CouponCode.ToLower();
/*
                var couponType = await context.CouponTypes.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponTypeId == coupon.CouponTypeId);
                if (couponType == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại mã giảm giá không tồn tại.";
                    return response;
                }*/

                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không thể trước ngày bắt đầu.";
                    return response;
                }
                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không được lớn hơn 7 ngày so với ngày bắt đầu.";
                    return response;
                }

                if (coupon.MinAmount <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Giá trị đơn hàng tối thiểu không hợp lệ. Phải lớn hơn 0.";
                    return response;
                }
                if (coupon.MaxUsage <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Số lần sử dụng tối đa phải lớn hơn 0.";
                    return response;
                }
                var existingCoupon = await context.Coupons.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponCode == coupon.CouponCode);
                if (existingCoupon != null)
                {
                    if (existingCoupon.Status == CouponStatus.Block)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã giảm giá này đang bị khoá. Vui lòng tạo lại!";
                        return response;
                    }
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá này đã sử dụng.";
                    return response;
                }

                // Thiết lập trạng thái mã giảm giá
                CouponStatus couponStatus;
                var currentDate = DateTime.Now;

                // Kiểm tra trạng thái của mã giảm giá
                if (currentDate < coupon.CouponStartDate)
                {
                    couponStatus = CouponStatus.Inactive; // Chưa đến ngày bắt đầu
                }
                else if (currentDate >= coupon.CouponStartDate && currentDate <= coupon.EndStart)
                {
                    couponStatus = CouponStatus.Active; // Đang có hiệu lực
                }
                else
                {
                    couponStatus = CouponStatus.Expired; // Hết hạn
                }
                // Cập nhật tất cả các thuộc tính của Coupon
                existingCoupon.CouponCode = coupon.CouponCode;
                existingCoupon.Discount = coupon.Discount;
                existingCoupon.Description = coupon.Description;
                existingCoupon.MinAmount = coupon.MinAmount;
                existingCoupon.CouponStartDate = coupon.CouponStartDate;
                existingCoupon.EndStart = coupon.EndStart;
                existingCoupon.MaxUsage = coupon.MaxUsage;
                existingCoupon.TimesUsed = coupon.TimesUsed;
                existingCoupon.Status = couponStatus; 
              /*  existingCoupon.CouponTypeId = coupon.CouponTypeId;*/

                await _context.SaveChangesAsync();
                response.Result = mapper.Map<CouponDto>(coupon);
                response.IsSuccess = true;
                response.Message = "Cập nhật mã giảm giá thành công.";
                response.Result = existingCoupon;


            }
            catch (Exception e) { response.IsSuccess = false; response.Message = e.Message; }
            return response;
        }
    }
}
