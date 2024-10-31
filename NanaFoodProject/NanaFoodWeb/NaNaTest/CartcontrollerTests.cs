using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using System.Security.Claims;
using Xunit;

namespace NaNaTest
{
    public class CartControllerTests
    {
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<ICartRepo> _mockCartRepo;
        private readonly CartController _cartController;

        public CartControllerTests()
        {
            // Setup mock SignInManager
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

            _mockCartRepo = new Mock<ICartRepo>();

            _cartController = new CartController(_mockSignInManager.Object, _mockCartRepo.Object);

            // Set up User Claims for SignInManager to simulate an authenticated user
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id-example"),
            }, "mock"));
            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };
        }

        [Fact]
        public async Task AddToCart_ReturnsOk_WhenAddToCartIsSuccessful()
        {            
            var cartDetails = new CartDetailsDto { ProductId = 1, Quantity = 2 };
            _mockCartRepo.Setup(repo => repo.AddToCart(cartDetails))
                         .ReturnsAsync(new ResponseDto { IsSuccess = true });
            
            var result = await _cartController.AddToCart(cartDetails);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task AddToCart_ReturnsBadRequest_WhenAddToCartFails()
        {            
            var cartDetails = new CartDetailsDto { ProductId = 1, Quantity = 2 };
            _mockCartRepo.Setup(repo => repo.AddToCart(cartDetails))
                         .ReturnsAsync(new ResponseDto { IsSuccess = false });
            
            var result = await _cartController.AddToCart(cartDetails);
            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetCart_ReturnsOk_WhenGetCartIsSuccessful()
        {           
            _mockCartRepo.Setup(repo => repo.GetCart(It.IsAny<User>()))
                         .ReturnsAsync(new ResponseDto { IsSuccess = true });
            
            var result = await _cartController.GetCart();
           
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task GetCart_ReturnsBadRequest_WhenGetCartFails()
        {            
            _mockCartRepo.Setup(repo => repo.GetCart(It.IsAny<User>()))
                         .ReturnsAsync(new ResponseDto { IsSuccess = false });
           
            var result = await _cartController.GetCart();
            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateCart_ShouldReturnUnauthorized_WhenUserNotLoggedIn()
        {           
            int productId = 1;
            string message = "Test message";

            // Simulate that the user is not logged in by returning null for the user ID
            _mockSignInManager.Setup(sm => sm.UserManager.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns((string)null);
           
            var result = await _cartController.UpdateCart(productId, message);
           
            Assert.IsType<UnauthorizedResult>(result);
        }        

        [Fact]
        public async Task DeleteProductFromCart_ShouldReturnBadRequest_WhenDeleteFails()
        {           
            int productId = 1;
            string userId = "testUserId";
            var response = new ResponseDto { IsSuccess = false };

            _mockSignInManager.Setup(sm => sm.UserManager.GetUserId(It.IsAny<ClaimsPrincipal>()))
                              .Returns(userId);
            _mockCartRepo.Setup(repo => repo.DeleteCart(productId, userId))
                         .ReturnsAsync(response);
            
            var result = await _cartController.DeleteProductFromCart(productId) as BadRequestObjectResult;
            
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }       

        [Fact]
        public async Task UpdateCart_ShouldReturnBadRequest_WhenUpdateFails()
        {
            int productId = 1;
            string userId = "testUserId";
            string message = "Updated message";
            var response = new ResponseDto { IsSuccess = false };

            _mockSignInManager.Setup(sm => sm.UserManager.GetUserId(It.IsAny<ClaimsPrincipal>()))
                              .Returns(userId);
            _mockCartRepo.Setup(repo => repo.UpdateCart(productId, userId, message))
                         .ReturnsAsync(response);

            var result = await _cartController.UpdateCart(productId, message) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }
    }
}

