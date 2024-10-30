using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        /// <summary>
        /// Tạo đánh giá cho sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng đăng đánh giá cho sản phẩm đã mua.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "comment": "Rất ngon!",
        ///     "rating": 5,
        ///     "userId": "user-id-example",
        ///     "productId": 1,
        ///     "orderId": 123
        /// }
        /// ```
        /// </remarks>
        /// <param name="reviewDto">Thông tin đánh giá của người dùng.</param>
        /// <returns>
        /// - 200 OK nếu đăng đánh giá thành công.
        /// - 400 BadRequest nếu có lỗi trong dữ liệu đầu vào.
        /// </returns>
        /// <response code="200">Đánh giá được đăng thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi đăng đánh giá.</response>
        [HttpPost("review")]
        public async Task<IActionResult> PostReviewAsync(ReviewDto reviewDto)
        {
            var response = await _reviewRepository.PostReviewAsync(reviewDto);
            
            if (response.IsSuccess)
            {
                var modifyState = await _reviewRepository.UpdateOrderDetailsReviewState(reviewDto.OrderId,reviewDto.ProductId,true);
                if (modifyState.IsSuccess)
                {
                    return Ok(modifyState);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Lấy tất cả đánh giá
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả các đánh giá.
        /// </remarks>
        /// <returns>
        /// - 200 OK nếu lấy danh sách đánh giá thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách tất cả đánh giá.</response>
        [HttpGet("reviews")]
        public async Task<IActionResult> GetAllReview()
        {
            var response = await _reviewRepository.GetAllReview();

            return Ok(response);
        }

        /// <summary>
        /// Lấy đánh giá theo ID
        /// </summary>
        /// <remarks>
        /// API này trả về thông tin chi tiết của một đánh giá dựa trên ID của đánh giá.
        /// </remarks>
        /// <param name="id">ID của đánh giá.</param>
        /// <returns>
        /// - 200 OK nếu lấy thông tin đánh giá thành công.
        /// - 404 NotFound nếu không tìm thấy đánh giá với ID này.
        /// </returns>
        /// <response code="200">Trả về thông tin đánh giá.</response>
        /// <response code="404">Không tìm thấy đánh giá.</response>
        [HttpGet("review/{id}")]
        public async Task<IActionResult> GetReviewByIdAsync([FromRoute] string id)
        {
            var response = await _reviewRepository.GetReviewByIdAsync(id);

            return Ok(response);
        }

        /// <summary>
        /// Lấy đánh giá theo ID người dùng
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các đánh giá của một người dùng cụ thể dựa trên UserID.
        /// </remarks>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách đánh giá thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách đánh giá của người dùng.</response>
        [HttpGet("userreview/{userId}")]
        public async Task<IActionResult> GetReviewByUserId([FromRoute] string userId)
        {
            var response = await _reviewRepository.GetReviewByUserId(userId);

            return Ok(response);
        }

        /// <summary>
        /// Lấy đánh giá theo ID sản phẩm
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các đánh giá cho một sản phẩm dựa trên ProductID.
        /// </remarks>
        /// <param name="productId">ID của sản phẩm.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách đánh giá thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách đánh giá cho sản phẩm.</response>
        [HttpGet("productreviews/{productId}")]
        public async Task<IActionResult> GetReviewByProductId(int productId)
        {
            var response = await _reviewRepository.GetReviewByProductId(productId);

            return Ok(response);
        }

        /// <summary>
        /// Lấy chi tiết đánh giá theo ID đơn hàng
        /// </summary>
        /// <remarks>
        /// API này trả về chi tiết các đánh giá trong một đơn hàng dựa trên OrderID.
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng.</param>
        /// <returns>
        /// - 200 OK nếu lấy chi tiết đánh giá trong đơn hàng thành công.
        /// </returns>
        /// <response code="200">Trả về chi tiết đánh giá trong đơn hàng.</response>
        [HttpGet("orderdetailsreview/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
        {
            var response = await _reviewRepository.GetOrderDetailsByOrderId(orderId);

            return Ok(response);
        }

        [HttpGet("orderdetailsInOrder/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsFromOrder(int orderId)
        {
            var response = await _reviewRepository.GetOrderDetailsFromOrder(orderId);

            return Ok(response);
        }
    }
}
