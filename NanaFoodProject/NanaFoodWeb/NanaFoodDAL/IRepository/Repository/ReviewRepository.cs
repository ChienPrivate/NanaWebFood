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
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public ReviewRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        public async Task<ResponseDto> GetAllReview()
        {
            try
            {
                var reviews = await _context.Reviews.ToListAsync();

                _response.IsSuccess = true;
                _response.Result = reviews;
                _response.Message = "Lấy danh sách đánh giá thành công";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.Message = ex.Message;
            }

            return _response;
        }

        public async Task<ResponseDto> GetReviewByIdAsync(string id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync();

            if (review != null)
            {
                _response.IsSuccess = true;
                _response.Message = $"Lấy thành công đánh giá có id {review.ReviewId}";
                _response.Result = _mapper.Map<ReviewDto>(review);
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = $"Không có đánh giá này";
                _response.Result = null;
            }

            return _response;
        }

        public async Task<ResponseDto> GetReviewByUserId(string userId)
        {
            var userReview = await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();

            if (userReview != null)
            {
                _response.IsSuccess = true;
                _response.Message = $"Lấy thành công danh sách đánh giá của người dùng";
                _response.Result = _mapper.Map<List<ReviewDto>>(userReview);
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = $"Không có đánh giá này";
                _response.Result = null;
            }

            return _response;
        }

        public async Task<ResponseDto> PostReviewAsync(ReviewDto reviewDto)
        {
            try
            {
                await _context.Reviews.AddAsync(_mapper.Map<Review>(reviewDto));

                await _context.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.Message = "Đánh giá sản phẩm thành công";
                _response.Result = reviewDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                _response.Result = reviewDto;
            }

            return _response;
        }

        public async Task<ResponseDto> GetReviewByProductId(int productId)
        {
            var productReview = await _context.Reviews.Where(r => r.ProductId == productId).ToListAsync();

            if (productReview != null)
            {
                _response.IsSuccess = true;
                _response.Message = $"Lấy thành công danh sách đánh giá của sản phẩm";
                _response.Result = _mapper.Map<List<ReviewDto>>(productReview);
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = $"Không có đánh giá này";
                _response.Result = null;
            }

            return _response;
        }

        public async Task<ResponseDto> GetProductRating()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                var orderDetails = await _context.OrderDetails
                .Where(o => o.OrderId == orderId && o.IsReviewed == false)
                .Include(o => o.Product)
                .ToListAsync();

                var productReview = orderDetails.Select(o => new ReviewProductDto
                {
                    ProductId = o.ProductId,
                    ProductImage = o.Product.ImageUrl,
                    ProductName = o.Product.ProductName,
                    OrderId = orderId,
                });

                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách sản phẩm cần đánh giá thành công";
                _response.Result = productReview;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
            }
            return _response;

        }

        public Task<double> CalculateAvgRating(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> UpdateOrderDetailsReviewState(int orderId, int productId, bool IsReviewState)
        {
            var orderDetails = await _context.OrderDetails.FirstOrDefaultAsync(o => o.OrderId == orderId && o.ProductId == productId);

            if (orderDetails != null)
            {
                orderDetails.IsReviewed = IsReviewState;

                var result =  await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Cập nhật trạng thái thành công";
                    _response.Result = orderDetails;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Xảy ra lỗi trong quá trình cập nhật trạng thái";
                _response.Result = orderDetails;
            }

            return _response;

        }
    }
}
