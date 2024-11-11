using Moq;
using Xunit;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace NaNaTest
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewRepository> _mockReviewRepo;
        private readonly ReviewController _controller;

        public ReviewControllerTests()
        {
            _mockReviewRepo = new Mock<IReviewRepository>();
            _controller = new ReviewController(_mockReviewRepo.Object);
        }

        [Fact]
        public async Task PostReviewAsync_ValidReview_ReturnsOkResult()
        {            
            var reviewDto = new ReviewDto { Comment = "Great!", Rating = 5, UserId = "user123", ProductId = 1, OrderId = 101 };
            _mockReviewRepo.Setup(repo => repo.PostReviewAsync(reviewDto)).ReturnsAsync(new ResponseDto { IsSuccess = true });
            _mockReviewRepo.Setup(repo => repo.UpdateOrderDetailsReviewState(reviewDto.OrderId, reviewDto.ProductId, true)).ReturnsAsync(new ResponseDto { IsSuccess = true });
            
            var result = await _controller.PostReviewAsync(reviewDto) as OkObjectResult;
            
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<ResponseDto>(result.Value);
        }

        [Fact]
        public async Task GetAllReview_ReturnsOkWithReviews()
        {           
            var reviews = new List<ReviewDto> { new ReviewDto { Comment = "Nice product", Rating = 4, UserId = "user456", ProductId = 2, OrderId = 102 } };
            _mockReviewRepo.Setup(repo => repo.GetAllReview()).ReturnsAsync(new ResponseDto { IsSuccess = true, Result = reviews });
            
            var result = await _controller.GetAllReview() as OkObjectResult;
            
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<ResponseDto>(result.Value);
        }

        [Fact]
        public async Task GetReviewByIdAsync_ExistingId_ReturnsOkWithReview()
        {           
            var reviewId = "review123";
            var review = new ReviewDto { Comment = "Excellent", Rating = 5, UserId = "user789", ProductId = 3, OrderId = 103 };
            _mockReviewRepo.Setup(repo => repo.GetReviewByIdAsync(reviewId)).ReturnsAsync(new ResponseDto { IsSuccess = true, Result = review });
            
            var result = await _controller.GetReviewByIdAsync(reviewId) as OkObjectResult;      
            
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<ResponseDto>(result.Value);
        }

        [Fact]
        public async Task GetReviewByUserId_ValidUserId_ReturnsOkWithReviews()
        {            
            var userId = "user123";
            var reviews = new List<ReviewDto> { new ReviewDto { Comment = "Great!", Rating = 5, UserId = userId, ProductId = 4, OrderId = 104 } };
            _mockReviewRepo.Setup(repo => repo.GetReviewByUserId(userId)).ReturnsAsync(new ResponseDto { IsSuccess = true, Result = reviews });
            
            var result = await _controller.GetReviewByUserId(userId) as OkObjectResult;
            
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<ResponseDto>(result.Value);
        }

        [Fact]
        public async Task GetReviewByProductId_ValidProductId_ReturnsOkWithPagedReviews()
        {            
            int productId = 5, page = 1, pageSize = 10;
            var reviews = new List<ReviewDto> { new ReviewDto { Comment = "Nice product", Rating = 4, UserId = "user234", ProductId = productId, OrderId = 105 } };
            _mockReviewRepo.Setup(repo => repo.GetReviewByProductId(productId, page, pageSize)).ReturnsAsync(new ResponseDto { IsSuccess = true, Result = reviews });
            
            var result = await _controller.GetReviewByProductId(productId, page, pageSize) as OkObjectResult;
            
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<ResponseDto>(result.Value);
        }

        [Fact]
        public async Task CalculateAvgRating_ValidProductId_ReturnsOkWithAverageRating()
        {           
            var productId = 5;
            var averageRating = 4.5;
            _mockReviewRepo.Setup(repo => repo.CalculateAvgRating(productId)).ReturnsAsync(averageRating);
            
            var result = await _controller.CalculateAvgRating(productId) as OkObjectResult;
            
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<ResponseDto>(result.Value);
            Assert.Equal(averageRating.ToString(), (result.Value as ResponseDto).Message);
        }
    }
}
