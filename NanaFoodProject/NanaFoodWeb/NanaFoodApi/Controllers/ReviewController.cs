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

        [HttpGet("reviews")]
        public async Task<IActionResult> GetAllReview()
        {
            var response = await _reviewRepository.GetAllReview();

            return Ok(response);
        }

        [HttpGet("review/{id}")]
        public async Task<IActionResult> GetReviewByIdAsync([FromRoute] string id)
        {
            var response = await _reviewRepository.GetReviewByIdAsync(id);

            return Ok(response);
        }

        [HttpGet("rserreview/{userId}")]
        public async Task<IActionResult> GetReviewByUserId([FromRoute] string userId)
        {
            var response = await _reviewRepository.GetReviewByUserId(userId);

            return Ok(response);
        }

        [HttpGet("productreviews/{productId}")]
        public async Task<IActionResult> GetReviewByProductId(int productId)
        {
            var response = await _reviewRepository.GetReviewByProductId(productId);

            return Ok(response);
        }

        [HttpGet("orderdetailsreview/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
        {
            var response = await _reviewRepository.GetOrderDetailsByOrderId(orderId);

            return Ok(response);
        }
    }
}
