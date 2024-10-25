using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class CouponRepo(ApplicationDbContext _context, IMapper _mapper) :ICouponRepo
    {
        readonly ApplicationDbContext context = _context;
        readonly IMapper mapper = _mapper; 
        ResponseDto response = new ResponseDto();

        public async Task<ResponseDto> Create(Coupon coupon)
        {
            try
            {
                coupon.CouponCode = coupon.CouponCode.ToLower();

                var couponType = await context.CouponTypes.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponTypeId == coupon.CouponTypeId);
                if (couponType == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại mã giảm giá không tồn tại.";
                    return response;
                }

                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không được lớn hơn 7 ngày so với ngày bắt đầu.";
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
                    CouponTypeId = coupon.CouponTypeId,
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

        public async Task<ResponseDto> GetAll(int page, int pageSize)
        {
            try
            {
                var Cp = await _context.Coupons.ToListAsync();
                var totalCount = Cp.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var CouponPerPage = Cp
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CouponType = _mapper.Map<List<CouponDto>>(CouponPerPage)

                };
                response.IsSuccess = true;
                response.Message = "Lấy sản phẩm thành công.";


            }
            catch (Exception e) { response.IsSuccess = false; response.Message = e.Message; }
            return response;
        }

        public async Task<ResponseDto> GetById(string id, int page , int pageSize)
        {
            try
            {
                var Cp = _context.Coupons.Where(p => p.CouponCode == id).ToList();
                var totalCount = Cp.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var CouponTypePerPage = Cp
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Products = _mapper.Map<List<CouponTypeDto>>(CouponTypePerPage)
                };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> Update(Coupon coupon)
        {
            try
            {
                var existingCoupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == coupon.CouponCode);
                if (existingCoupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không tồn tại.";
                    return response;
                }
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
                existingCoupon.CouponTypeId = coupon.CouponTypeId;

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
