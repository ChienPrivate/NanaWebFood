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
    }
}
