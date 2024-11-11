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
        private readonly Mock<UserManager<User>> _userManagerMock;

        public CartControllerTests()
        {
            // Setup mock SignInManager
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
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
        public async Task DeleteProductFromCart_ShouldReturnOk_WhenProductIsDeleted()
        {
            // Arrange
            var userId = "user-id";
            var productId = 1;

            // Mock the GetUserId() method to return a user ID
            _mockSignInManager.Setup(x => x.UserManager.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);

            // Mock the repository response
            _mockCartRepo.Setup(x => x.DeleteCart(productId, userId))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Message = "Sản phẩm đã được xóa khỏi giỏ hàng" });

            // Act
            var result = await _cartController.DeleteProductFromCart(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(responseDto.IsSuccess);
            Assert.Equal("Sản phẩm đã được xóa khỏi giỏ hàng", responseDto.Message);
        }

        [Fact]
        public async Task DeleteProductFromCart_ShouldReturnBadRequest_WhenDeleteFails()
        {
            // Arrange
            var userId = "user-id";
            var productId = 1;

            // Mock the GetUserId() method to return a user ID
            _mockSignInManager.Setup(x => x.UserManager.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);

            // Mock the repository response for failure
            _mockCartRepo.Setup(x => x.DeleteCart(productId, userId))
                .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Lỗi khi xóa sản phẩm khỏi giỏ hàng" });

            // Act
            var result = await _cartController.DeleteProductFromCart(productId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(responseDto.IsSuccess);
            Assert.Equal("Lỗi khi xóa sản phẩm khỏi giỏ hàng", responseDto.Message);
        }

        [Fact]
        public async Task UpdateCart_ShouldReturnOk_WhenQuantityIsUpdated()
        {
            // Arrange
            var userId = "user-id";
            var productId = 1;
            var message = "increase"; // or "decrease"

            // Mock the GetUserId() method directly on SignInManager
            _mockSignInManager.Setup(x => x.UserManager.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);

            // Mock the repository response
            _mockCartRepo.Setup(x => x.UpdateCart(productId, userId, message))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Message = "Cập nhật số lượng thành công" });

            // Act
            var result = await _cartController.UpdateCart(productId, message);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(responseDto.IsSuccess);
            Assert.Equal("Cập nhật số lượng thành công", responseDto.Message);
        }


        [Fact]
        public async Task UpdateCart_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var userId = "user-id";
            var productId = 1;
            var message = "increase"; // or "decrease"

            // Mock the GetUserId() method to return a user ID
            _mockSignInManager.Setup(x => x.UserManager.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);

            // Mock the repository response
            _mockCartRepo.Setup(x => x.UpdateCart(productId, userId, message))
                .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Lỗi cập nhật số lượng" });

            // Act
            var result = await _cartController.UpdateCart(productId, message);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(responseDto.IsSuccess);
            Assert.Equal("Lỗi cập nhật số lượng", responseDto.Message);
        }
    }
}

