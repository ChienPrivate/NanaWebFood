using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository.Repository
{
    public class CouponTypeRepo(ApplicationDbContext context, IMapper mapper) : ICouponTypeRepo
    {
        ApplicationDbContext _context = context;
        IMapper _mapper = mapper;
        ResponseDto response = new ResponseDto();
        public async Task<ResponseDto> Create(CouponType couponType)
        {
            try
            {
                var eCpType = await _context.CouponTypes.FirstOrDefaultAsync(e => e.CouponTypeId == couponType.CouponTypeId);
                if (eCpType != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại giảm giá này đã tồn tại.";
                    return response;
                }
                if (couponType == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Đầu vào không hợp lệ";
                    return response;
                }
                if (_context.CouponTypes.Any(e => e.TypeName == couponType.TypeName))
                {
                    response.IsSuccess = false;
                    response.Message = "Loại mã giảm giá này đã tồn tại";
                    return response;
                }
                _context.CouponTypes.Add(couponType);
                await _context.SaveChangesAsync();
                response.Result = _mapper.Map<CouponTypeDto>(couponType);
                response.IsSuccess = true;
                response.Message = "Tạo mã giảm giá thành công.";


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var eCpType = _context.CouponTypes.FirstOrDefault(e => e.CouponTypeId == id);
                if (eCpType != null)
                {
                    _context.CouponTypes.Remove(eCpType);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.Message = "Xoá thành công.";


                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Mã giảm giá không tồn tại.";
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> GetAll(int page, int pageSize, bool isSelectAll = true)
        {
            try
            {
                var CpType = await _context.CouponTypes.ToListAsync();
                var totalCount = CpType.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var CouponTypePerPage = CpType
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

                response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CouponType = _mapper.Map<List<CouponTypeDto>>(CouponTypePerPage)

                };
                response.IsSuccess = true;
                response.Message = "Lấy sản phẩm thành công.";


            }
            catch (Exception e) { response.IsSuccess = false; response.Message = e.Message; }
            return response;
        }

        public async Task<ResponseDto> GetByCpTypeId(int id, int page, int pageSize)
        {
            try
            {
                var CpType = _context.CouponTypes.Where(p => p.CouponTypeId == id).ToList();
                var totalCount = CpType.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var CouponTypePerPage = CpType
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
        public async Task<ResponseDto> Update(CouponType couponType)
        {
            try
            {
                var eCpType = await _context.CouponTypes.FirstOrDefaultAsync(e => e.CouponTypeId == couponType.CouponTypeId);
                if (eCpType == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Loại sản phẩm không tồn tại.";
                    return response;
                }
                eCpType.CouponTypeId = couponType.CouponTypeId;
                eCpType.TypeName = couponType.TypeName;
                eCpType.Description = couponType.Description;
                await _context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message = "Cập nhật thành công.";


            }
            catch (Exception e) { response.IsSuccess = false; response.Message = e.Message; }
            return response;

        }
    }



}
