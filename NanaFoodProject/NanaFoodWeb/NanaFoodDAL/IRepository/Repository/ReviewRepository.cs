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

        public async Task<ResponseDto> GetReviewByProductId(int productId, int page, int pageSize)
        {
            var productReview = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .Select(ur => new UserReviewDto
                {
                    ReviewId = ur.ReviewId.ToString(),
                    UserId = ur.UserId,
                    UserName = ur.User.UserName ?? "Khách",
                    FullName = ur.User.FullName ?? "Khách",
                    AvatarUrl = ur.User.AvatarUrl ?? "https://placehold.co/300x300",
                    Comment = ur.Comment,
                    Rating = ur.Rating,
                    ReviewedDate = ur.ReviewedDate,
                }).ToListAsync();

            var Reviewpage = productReview
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = productReview.Count;
            var totalPagesReviewPage = (int)Math.Ceiling((decimal)totalCount / pageSize);

            if (productReview.Any())
            {
                _response.IsSuccess = true;
                _response.Message = $"Lấy thành công danh sách đánh giá của sản phẩm";
                _response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPagesReviewPage,
                    Reviews = Reviewpage
                };
            }
            else
            {
                _response.IsSuccess = true;
                _response.Message = $"Không có đánh giá nào";
                _response.Result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPagesReviewPage,
                    Reviews = new List<UserReviewDto>()
                };
            }

            return _response;
        }

        public async Task<ResponseDto> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                var orderDetails = await _context.OrderDetails
                .Where(o => o.OrderId == orderId && o.IsReviewed == false)
                .Include(o => o.Product)
                .Include (o => o.Review)
                .ToListAsync();

                var productReview = orderDetails.Select(o => new ReviewProductDto
                {
                    ProductId = o.ProductId,
                    ProductImage = o.Product.ImageUrl,
                    ProductName = o.Product.ProductName,
                    Price = o.Product.Price,
                    Quantity = o.Quantity,
                    Total = o.Total,
                    Comment = o.Review.Comment,
                    Rating = o.Review.Rating,
                    IsReviewed = o.IsReviewed,
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

        public async Task<double> CalculateAvgRating(int productId)
        {
            var confirmedReviews = _context.Reviews
                .Where(o => o.ProductId == productId && o.IsConfirm);

            var totalRating = confirmedReviews.Sum(o => o.Rating);
            var reviewCount = confirmedReviews.Count();

            return reviewCount > 0 ? (double)totalRating / reviewCount : 0.0;
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

        public async Task<ResponseDto> GetOrderDetailsFromOrder(int orderId)
        {
            try
            {
                var productList = await (from od in _context.OrderDetails
                                         join r in _context.Reviews on new { od.OrderId, od.ProductId } equals new { r.OrderId, r.ProductId } into reviewGroup
                                         from review in reviewGroup.DefaultIfEmpty()
                                         where od.OrderId == orderId // Lọc theo OrderId trong OrderDetails
                                         select new ReviewProductDto
                                         {
                                             ProductId = od.ProductId,
                                             ProductImage = od.ImageUrl,
                                             ProductName = od.ProductName,
                                             Price = od.Price,
                                             Quantity = od.Quantity,
                                             Total = od.Total,
                                             Comment = review.Comment ?? "No comment", // Gán mặc định "No comment" nếu không có đánh giá
                                             Rating = review.Rating ?? 0, // Gán mặc định 0 nếu không có đánh giá
                                             IsReviewed = od.IsReviewed,
                                             OrderId = od.OrderId
                                         }).ToListAsync();

                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách sản phẩm từ đơn hàng thành công";
                _response.Result = productList;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message += ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetReviewWithUser()
        {
            try
            {
                var reviewsWithUsers = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.User != null)
                .Select(r => new UserWithReviewDto
                {
                    ReviewId = r.ReviewId.ToString(),
                    UserId = r.UserId,
                    UserAvartar = r.User.AvatarUrl ?? "https://placehold.co/300x300",
                    FullName = r.User.FullName,
                    UserName = r.User.UserName,
                    Rating = r.Rating ?? 0, // Nếu null thì mặc định là 0
                    Comment = r.Comment,
                    ReviewedDate = r.ReviewedDate,
                    IsConfirm = r.IsConfirm

                }).ToListAsync();

                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách đánh giá thành công";
                _response.Result = reviewsWithUsers;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                _response.Result = new List<UserWithReviewDto>();
            }

            return _response;
        }

        public async Task<ResponseDto> ConfirmReview(string reviewId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId.ToString() == reviewId);

            if (review == null)
            {
                _response.IsSuccess = false;
                _response.Message = $"Không tìm thấy đánh giá {reviewId}";

                return _response;
            }

            var reviewDto = _mapper.Map<ReviewDto>(review);

            review.IsConfirm = !review.IsConfirm;

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            _response.IsSuccess = true;
            _response.Message = $"Duyệt đánh giá {reviewDto.ReviewId} thành công";
            _response.Result = reviewDto;

            return _response;

        }

        public async Task<ResponseDto> GetReviewById(string reviewId)
        {
            var reviewWithUser = await _context.Reviews
                .Include(r => r.User) // Include để lấy thông tin liên quan đến User\
                .Select(r => new UserWithReviewDto
                {
                    ReviewId = r.ReviewId.ToString(),
                    UserId = r.UserId,
                    ProductId = r.ProductId,
                    UserAvartar = r.User.AvatarUrl ?? "https://placehold.co/300x300",
                    FullName = r.User.FullName,
                    UserName = r.User.UserName,
                    Rating = r.Rating ?? 0, // Nếu null thì mặc định là 0
                    Comment = r.Comment,
                    ReviewedDate = r.ReviewedDate,
                    IsConfirm = r.IsConfirm

                }).FirstOrDefaultAsync();

            _response.IsSuccess = reviewWithUser != null;
            _response.Message = reviewWithUser != null ? $"Lấy thông tin đánh giá {reviewId} thành công" : $"Đánh giá {reviewId} không tồn tại";
            _response.Result = reviewWithUser != null ? reviewWithUser : new UserWithReviewDto();

            return _response;
        }

        public async Task<ResponseDto> GetProductById(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            _response.IsSuccess = product != null;
            _response.Message = product != null ? $"Tìm thấy sản phẩm có mã {product.ProductId}" : $"Không tìm thấy sản phẩm có mã {productId}";
            _response.Result = product != null ? _mapper.Map<ProductDto>(product) : new ProductDto();

            return _response;
        }
    }
}
