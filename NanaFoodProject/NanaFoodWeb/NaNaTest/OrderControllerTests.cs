using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Helper;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using FluentAssertions;
using Xunit;

namespace NaNaTest
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICartRepo> _cartRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<EmailPoster> _emailPosterMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _cartRepoMock = new Mock<ICartRepo>();
            _mapperMock = new Mock<IMapper>();
            _emailPosterMock = new Mock<EmailPoster>();
            _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null, null, null, null);

            _controller = new OrderController(
                _orderRepoMock.Object,
                _signInManagerMock.Object,
                _cartRepoMock.Object,
                _mapperMock.Object,
                _emailPosterMock.Object);
        }

        private Mock<SignInManager<User>> MockSignInManager()
        {
            var userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            return new Mock<SignInManager<User>>(
                userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
        }

        [Fact]
        public async Task GetAllOrderAync_ReturnsOk_WithOrderList()
        {
            var orders = new List<OrderDto>
            {
                new OrderDto { OrderId = 1, UserId = "user-id-1" },
                new OrderDto { OrderId = 2, UserId = "user-id-2" }
            };
            _orderRepoMock.Setup(repo => repo.GetAllOrderAync()).ReturnsAsync(new ResponseDto
            {
                IsSuccess = true,
                Result = orders
            });

            var result = await _controller.GetAllOrderAync();

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeEquivalentTo(new ResponseDto
                  {
                      IsSuccess = true,
                      Result = orders
                  });
        }

        [Fact]
        public async Task AddOrderAsync_ShouldReturnOk_WhenOrderIsCreated()
        {
            // Arrange
            var user = new User { Id = "user-id", Email = "user@example.com" };
            var orderDto = new OrderDto
            {
                FullName = "Nguyễn Văn An",
                PhoneNumber = "0123456789",
                Address = "123 Đường ABC, TP. HCM",
                PaymentType = "MOMO",
                PaymentStatus = "Đã thanh toán",
                OrderStatus = "Đang chuẩn bị",
                ShipmentFee = 30000,
                Note = "Giao hàng vào buổi sáng",
                UserId = "user-id-example",
                Total = 500000,
                OrderDate = DateTime.Now,
                ReceiveDate = DateTime.Now.AddDays(7)
            };

            var cartItems = new List<CartResponseDto>
            {
                new CartResponseDto
                {
                    ProductId = 1,
                    ProductName = "Product 1",
                    Price = 100000,
                    Image = "image1.jpg",
                    Quantity = 2,
                    Total = 200000
                }
            };

            var order = new Order
            {
                UserId = "user-id",
                Total = 500000,
                OrderDate = DateTime.Now,
                ReceiveDate = DateTime.Now.AddDays(7)
            };

            var createdOrder = new Order { OrderId = 1 };

            _signInManagerMock.Setup(x => x.UserManager.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _cartRepoMock.Setup(x => x.GetCart(It.IsAny<User>())).ReturnsAsync(new ResponseDto
            {
                IsSuccess = true,
                Result = cartItems
            });
            _mapperMock.Setup(x => x.Map<Order>(It.IsAny<OrderDto>())).Returns(order);
            _orderRepoMock.Setup(x => x.AddOrder(It.IsAny<Order>())).ReturnsAsync(createdOrder);
            _orderRepoMock.Setup(x => x.UpdateProductQuantity(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResponseDto { IsSuccess = true });
            _orderRepoMock.Setup(x => x.AddOrderDetails(It.IsAny<IEnumerable<OrderDetails>>())).Verifiable();
            _cartRepoMock.Setup(x => x.RemoveAllCartItem(It.IsAny<string>())).ReturnsAsync(new ResponseDto { IsSuccess = true });
            
            _emailPosterMock.Setup(x => x.OrderEmailTemplate(It.IsAny<Order>(), It.IsAny<List<OrderDetails>>())).Returns("Email content");

            var result = await _controller.AddOrderAsync(orderDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(responseDto.IsSuccess);
            Assert.Equal("Tạo đơn hàng thành công", responseDto.Message);
        }

        [Fact]
        public async Task AddOrderAsync_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("key", "Some error");

            var orderDto = new OrderDto
            {
                FullName = "Nguyễn Văn An",
                PhoneNumber = "0123456789",
                Address = "123 Đường ABC, TP. HCM",
                PaymentType = "MOMO",
                PaymentStatus = "Đã thanh toán",
                OrderStatus = "Đang chuẩn bị",
                ShipmentFee = 30000,
                Note = "Giao hàng vào buổi sáng",
                UserId = "user-id-example",
                Total = 500000,
                OrderDate = DateTime.Now,
                ReceiveDate = DateTime.Now.AddDays(7)
            };

            var result = await _controller.AddOrderAsync(orderDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }


        [Fact]
        public async Task GetOrderByIdAsync_OrderExists_ReturnsOk()
        {
            int orderId = 1;
            var order = new OrderDto { OrderId = orderId, UserId = "user-id-example" };

            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(new ResponseDto
            {
                IsSuccess = true,
                Result = order
            });

            var result = await _controller.GetOrderByIdAsync(orderId);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeEquivalentTo(new ResponseDto
                  {
                      IsSuccess = true,
                      Result = order
                  });
        }

    }
}
