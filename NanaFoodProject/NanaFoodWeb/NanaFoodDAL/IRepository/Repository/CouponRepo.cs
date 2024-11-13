using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System.Text.RegularExpressions;

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
                coupon.CouponCode = coupon.CouponCode.ToLower();
                var existingCoupon = await context.Coupons.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponCode == coupon.CouponCode);
                if (existingCoupon != null)
                {
                    if (existingCoupon.Status == CouponStatus.Delete)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã giảm giá này đang bị hiệu hoá. Vui lòng tạo lại!";
                        return response;
                    }
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá này đã được tạo.";
                    return response;
                }

                if (coupon.EndStart <= coupon.CouponStartDate)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không thể trước ngày bắt đầu.";
                    return response;
                }

                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không được cách ngày bắt đầu quá 7 ngày.";
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
                context.Coupons.Add(coupon);
                await context.SaveChangesAsync();

                response.Result = mapper.Map<CouponDto>(coupon);
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
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Không có mã giảm giá.";
                    return response;
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

        public async Task<ResponseDto> ModifyStatus(string id)
        {
            var eCoupon = await context.Coupons.FirstOrDefaultAsync(e => e.CouponCode == id);
            if (eCoupon == null)
            {
                response.IsSuccess = false;
                response.Message = "Mã giảm giá không tồn tại";
                return response;
            }
            eCoupon.Status = CouponStatus.Delete;
            context.Coupons.Update(eCoupon);
            await context.SaveChangesAsync();

            response.IsSuccess = true;
            response.Message = "Cập nhật trạng thái thành công";
            return response;
        }

        public async Task<ResponseDto> Update(Coupon coupon)
        {
            try
            {
                var existingCoupon = await context.Coupons.AsNoTracking()
                  .FirstOrDefaultAsync(e => e.CouponCode == coupon.CouponCode);

                if (existingCoupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá này không tồn tại.";
                    return response;
                }

                // Thiết lập trạng thái mã giảm giá
                var currentDate = DateTime.Now;

                if (currentDate < coupon.CouponStartDate)
                {
                    coupon.Status = CouponStatus.Inactive; // Chưa đến ngày bắt đầu
                }
                else if (currentDate >= coupon.CouponStartDate && currentDate <= coupon.EndStart)
                {
                    coupon.Status = CouponStatus.Active; // Đang có hiệu lực
                }
                else
                {
                    coupon.Status = CouponStatus.Expired; // Hết hạn
                }

                _context.Entry(coupon).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                response.IsSuccess = true;
                response.Result = _mapper.Map<CouponDto>(coupon);
                response.Message = "Cập nhật mã giảm giá thành công.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return response;
            /*try
            {
                coupon.CouponCode = coupon.CouponCode.ToLower();
                var existingCoupon = await context.Coupons.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.CouponCode == coupon.CouponCode);
                if (existingCoupon != null)
                {
                    if (existingCoupon.Status == CouponStatus.Delete)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã giảm giá này đang bị hiệu hoá. Vui lòng tạo lại!";
                        return response;
                    }
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá này đã được tạo.";
                    return response;
                }

                if (coupon.EndStart <= coupon.CouponStartDate)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không thể trước ngày bắt đầu.";
                    return response;
                }

                if ((coupon.EndStart - coupon.CouponStartDate).TotalDays > 7)
                {
                    response.IsSuccess = false;
                    response.Message = "Ngày kết thúc không được cách ngày bắt đầu quá 7 ngày.";
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

                await context.SaveChangesAsync();
                response.Result = mapper.Map<CouponDto>(coupon);
                response.IsSuccess = true;
                response.Message = "Cập nhật mã giảm giá thành công.";
                response.Result = existingCoupon;


            }
            catch (Exception e) { response.IsSuccess = false; response.Message = e.Message; }
            return response;*/
        }
    }
}
