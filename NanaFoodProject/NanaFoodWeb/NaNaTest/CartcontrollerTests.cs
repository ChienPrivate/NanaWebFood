using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
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
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthController _authcontroller;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IAuthenRepo> _authRepoMock;
        public static class MockHelpers
        {
            public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
            {
                var store = new Mock<IUserStore<TUser>>();
                return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            }
            public static Mock<SignInManager<TUser>> MockSignInManager<TUser>(UserManager<TUser> userManager) where TUser : class
            {
                var contextAccessor = new Mock<IHttpContextAccessor>();
                var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
                return new Mock<SignInManager<TUser>>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
            }
        }
        public CartControllerTests()
        {
            _authRepoMock = new Mock<IAuthenRepo>();
            _userManagerMock = MockHelpers.MockUserManager<User>();
            _mockSignInManager = MockHelpers.MockSignInManager(_userManagerMock.Object);
            _mapperMock = new Mock<IMapper>();
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockCartRepo = new Mock<ICartRepo>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cartController = new CartController(_mockSignInManager.Object, _mockCartRepo.Object);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "4536e838-7775-4bf2-a86f-0bf8a4db22f1"),
            }, "mock"));
            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };
            _authcontroller = new AuthController(_authRepoMock.Object, _mockSignInManager.Object, _userManagerMock.Object, _mapperMock.Object, _tokenServiceMock.Object);
            
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
            var userId = "e3582fd2-9f86-4c2c-901a-a9cbf81134a7";
            var productId = 1;

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)  // Đặt Claim để chứa userId
            }, "mock"));
            //_mockSignInManager.Setup(x => x.UserManager.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            _mockCartRepo.Setup(x => x.DeleteCart(productId, userId))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Message = "Sản phẩm đã được xóa khỏi giỏ hàng" });

            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var result = await _cartController.DeleteProductFromCart(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(responseDto.IsSuccess);
            Assert.Equal("Sản phẩm đã được xóa khỏi giỏ hàng", responseDto.Message);
        }

        [Fact]
        public async Task DeleteProductFromCart_ShouldReturnBadRequest_WhenDeleteFails()
        {
            var userId = "e3582fd2-9f86-4c2c-901a-a9cbf81134a7";
            var productId = 1;

            _userManagerMock
                .Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);
            _mockCartRepo
                .Setup(x => x.DeleteCart(productId, userId))
                .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Lỗi khi xóa sản phẩm khỏi giỏ hàng" });
            var result = await _cartController.DeleteProductFromCart(productId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(responseDto.IsSuccess);
            Assert.Equal("Lỗi khi xóa sản phẩm khỏi giỏ hàng", responseDto.Message);
        }

        [Fact]
        public async Task UpdateCart_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            int productId = 1;
            string message = "increase";
            var userId = "e3582fd2-9f86-4c2c-901a-a9cbf81134a7";
            var responseDto = new ResponseDto { IsSuccess = true };

            _userManagerMock
                .Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);

            _mockCartRepo
                .Setup(x => x.UpdateCart(productId, userId, message))
                .ReturnsAsync(responseDto);

            var result = await _cartController.UpdateCart(productId, message);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task UpdateCart_ShouldReturnBadRequest_WhenUpdateFails()
        {
            int productId = 1;
            string message = "increase";
            var userId = "e3582fd2-9f86-4c2c-901a-a9cbf81134a7";
            var responseDto = new ResponseDto { IsSuccess = false, Message = "Update failed" };

            _userManagerMock
                .Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);

            _mockCartRepo
                .Setup(x => x.UpdateCart(productId, userId, message))
                .ReturnsAsync(responseDto);

            var result = await _cartController.UpdateCart(productId, message);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(responseDto, badRequestResult.Value);
        }
    }
}

