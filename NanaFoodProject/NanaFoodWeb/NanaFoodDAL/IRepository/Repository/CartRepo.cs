﻿using AutoMapper;
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
            if (cartdetailDto.UserId == null)
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
                var cart = from a in _context.CartDetails
                                join b in _context.Products on a.ProductId equals b.ProductId
                                where a.UserId == user.Id
                                select new
                                {
                                    UserId = a.UserId,
                                    ProductId = a.ProductId,
                                    Quantity = a.Quantity,
                                    Total = a.Total,
                                    ProductName = b.ProductName,
                                    Price = b.Price,
                                    Image = b.ImageUrl,
                                };
                var cartList = await cart.ToListAsync();
                if (cartList.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Result = cartList;
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

        public async Task<ResponseDto> UpdateCart(int ProductId,string UserId, string message)
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null)
            {
                response.IsSuccess = false;
                response.Message = "Sản phẩm không tồn tại";
                return response;
            }
            var cartItem = await _context.CartDetails
            .FirstOrDefaultAsync(c => c.UserId == UserId && c.ProductId == ProductId);

            if (cartItem != null)
            {
                if(message.ToLower() == "decrease")
                {
                    if(cartItem.Quantity == 1)
                    {
                        _context.CartDetails.Remove(cartItem);
                    } else
                    {
                        cartItem.Quantity--;
                        cartItem.Total = cartItem.Quantity * product.Price;
                    }
                }
                else
                {
                    cartItem.Quantity++;
                    cartItem.Total = cartItem.Quantity * product.Price;
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
            response.IsSuccess = true;
            response.Message = "Cập nhật số lượng thành công";
            response.Result = new {Total = cartItem.Total, Quantity = cartItem.Quantity};
            return response;
        }
    }
}
