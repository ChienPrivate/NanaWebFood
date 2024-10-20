using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository.Repository
{
    public class CartRepo(ApplicationDbContext context, IMapper mapper) : ICartRepo
    {
        ApplicationDbContext _context = context;
        IMapper _mapper = mapper;
        ResponseDto response = new ResponseDto();

        public async Task<ResponseDto> AddToCart(CartDetailsDto cartdetailDto)
        {
            var product = await _context.Products.FindAsync(cartdetailDto.ProductId);
            if (product == null)
            {
                response.IsSuccess = false;
                response.Message = "Sản phẩm không tồn tại";
                return response;
            }
            var user = await _context.Users.FindAsync(cartdetailDto.UserId);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Người dùng không tồn tại";
                return response;
            }

            var cartItem = await _context.CartDetails
            .FirstOrDefaultAsync(c => c.UserId == cartdetailDto.UserId && c.ProductId == cartdetailDto.ProductId);

            if (cartItem != null)
            {
                // Sản phẩm đã có trong giỏ hàng, cập nhật số lượng và tổng tiền
                cartItem.Quantity += cartdetailDto.Quantity;
                cartItem.Total = cartItem.Quantity * product.Price;
            }
            else
            {
                cartItem = _mapper.Map<CartDetails>(cartdetailDto);
                cartItem.Total = cartdetailDto.Quantity * product.Price;
                _context.CartDetails.Add(cartItem);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
            response.IsSuccess = true;
            response.Message = "Đã thêm vào giỏ hàng";
            return response;
        }

        public async Task<ResponseDto> DeleteCart(int ProductId, string UserID)
        {
            try
            {
                var eCartDetails = _context.CartDetails.FirstOrDefault(c => c.ProductId == ProductId && c.UserId == UserID);
                if (eCartDetails is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mục giỏ hàng không tồn tại";
                    return response;
                }
                _context.CartDetails.Remove(eCartDetails);
                await _context.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "Xóa mục giỏ hàng thành công";

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Đã xảy ra lỗi khi xóa mục giỏ hàng: {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> GetCart(User user)
        {
            try
            {
                var cart = await _context.CartDetails.Where(x => x.UserId == user.Id).ToListAsync();
                if (cart.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Result = _mapper.Map<List<CartDetailsDto>>(cart);
                    response.Message = "Lấy thông tin giỏ hàng thành công";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Chưa có sản phẩm nào trong giỏ hàng";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }
    }
}
