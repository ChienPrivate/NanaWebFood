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
using System.Security.Claims;
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
        public async Task AddOrderAsync_ValidOrder_ReturnsOk()
        {
            var orderDto = new OrderDto { UserId = "user-id-example", Total = 500000 };
            var user = new User { Id = "user-id-example" };

            typeof(SignInManager<User>)
                .GetProperty("UserManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_signInManagerMock.Object, _userManagerMock.Object);

            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(user);

            var order = new Order { OrderId = 1, UserId = user.Id };
            _mapperMock.Setup(mapper => mapper.Map<Order>(orderDto)).Returns(order);
            _orderRepoMock.Setup(repo => repo.AddOrder(order)).ReturnsAsync(order);
            _cartRepoMock.Setup(repo => repo.GetCart(user)).ReturnsAsync(new ResponseDto
            {
                IsSuccess = true,
                Result = new List<CartResponseDto> { new CartResponseDto { ProductId = 1, Quantity = 2 } }
            });

            var result = await _controller.AddOrderAsync(orderDto);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ResponseDto;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(new ResponseDto
            {
                Message = "Create Order successfully",
                IsSuccess = true,
                Result = order.OrderId
            });
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

        [Fact]
        public async Task CancelOrderAsync_ValidOrderId_ReturnsOk()
        {
            int orderId = 1;
            string message = "Order canceled";
            var user = new User { Id = "user-id-example" };

            _signInManagerMock.Setup(manager => manager.UserManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                              .ReturnsAsync(user);

            _orderRepoMock.Setup(repo => repo.CancelOrderAsync(orderId, message))
                          .ReturnsAsync(new ResponseDto
                          {
                              IsSuccess = true,
                              Message = "Order canceled successfully"
                          });

            _orderRepoMock.Setup(repo => repo.UpdateProductQuantity(orderId, -1))
                          .ReturnsAsync(new ResponseDto
                          {
                              IsSuccess = true,
                              Message = "Product quantity updated successfully"
                          });

            _orderRepoMock.Setup(repo => repo.GetOrderDetailsAsync(orderId))
                          .ReturnsAsync(new ResponseDto
                          {
                              IsSuccess = true,
                              Result = new List<OrderDetailsDto> { new OrderDetailsDto { ProductId = 1, Quantity = 2 } }
                          });

            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId))
                          .ReturnsAsync(new ResponseDto
                          {
                              IsSuccess = true,
                              Result = new OrderDto { UserId = user.Id, Email = "user@example.com" }
                          });

            var result = await _controller.CancelOrderAsync(orderId, message);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as ResponseDto;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(new ResponseDto
            {
                IsSuccess = true,
                Message = "Order canceled successfully"
            });
        }
    }
}
