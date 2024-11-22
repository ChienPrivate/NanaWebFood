using Azure;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Route("Review")]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        public async Task<IActionResult> Index()
        {
            var reviewWithUserResponse = await _reviewRepository.GetReviewWithUsers();

            if (reviewWithUserResponse.IsSuccess)
            {
                var reviewithUser = JsonConvert.DeserializeObject<List<UserWithReviewDto>>(reviewWithUserResponse.Result.ToString());

                var confirmedYet = reviewithUser.Where(c => !c.IsConfirm);
                var confirmed = reviewithUser.Where(c => c.IsConfirm);

                ViewBag.ConfirmedYet = confirmedYet;
                ViewBag.Confirmed = confirmed;
            }

            return View();
        }

        [HttpGet("Details/{reviewId}")]
        public async Task<IActionResult> Details(string reviewId)
        {
            var reviewResponse = await _reviewRepository.GetReviewById(reviewId);
            

            if (reviewResponse.IsSuccess)
            {
                var review = JsonConvert.DeserializeObject<UserWithReviewDto>(reviewResponse.Result.ToString());

                var productResponse = await _reviewRepository.GetProductById(review.ProductId);
                var ratingResponse = await _reviewRepository.GetProductRating(review.ProductId);
                if (productResponse.IsSuccess)
                {
                    var product = JsonConvert.DeserializeObject<ProductDto>(productResponse.Result.ToString());
                    ViewBag.Product = product;
                }

                if (ratingResponse.IsSuccess)
                {
                    ViewData["rating"] = double.Parse(ratingResponse.Result.ToString());
                }

                return View(review);
            }
            TempData["error"] = $"{reviewResponse.Message}";
            return RedirectToAction("Index", "Review");
        }

        [HttpGet("ConfirmReview/{reviewId}")]
        public async Task<IActionResult> ConfirmReview(string reviewId)
        {
            var reiewConfirmResponse = await _reviewRepository.ConfirmReview(reviewId);

            if (reiewConfirmResponse.IsSuccess)
            {
                var review = JsonConvert.DeserializeObject<ReviewDto>(reiewConfirmResponse.Result.ToString());

                if (review.IsConfirm)
                {
                    TempData["success"] = $"Đã bỏ duyệt đánh giá {reviewId}";
                }
                else
                {
                    TempData["success"] = $"Đã duyệt đánh giá {reviewId}";
                }

                return RedirectToAction("Index", "Review");
            }

            TempData["error"] = $"Xảy ra lỗi trong quá trình duyệt đánh giá {reviewId}";
            return RedirectToAction("Index","Review");
        }

        [HttpGet("GetReviewList")]
        public async Task<IActionResult> GetReviewList()
        {
            var reviewWithUserResponse = await _reviewRepository.GetReviewWithUsers();

            if (reviewWithUserResponse.IsSuccess)
            {
                var reviewList = JsonConvert.DeserializeObject<List<UserWithReviewDto>>(reviewWithUserResponse.Result.ToString());

                // Trả về dữ liệu dưới dạng JSON
                return Json(reviewList);
            }

            // Nếu xảy ra lỗi, trả về lỗi 500
            return StatusCode(500, "Không thể lấy danh sách đánh giá");
        }
    }
}
