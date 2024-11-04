using Xunit;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using System.Security.Claims;

namespace NaNaTest
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ICartRepo> _cartRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(userStoreMock.Object, null, null, null, null, null, null);

            _orderRepositoryMock = new Mock<IOrderRepository>();
            _cartRepoMock = new Mock<ICartRepo>();
            _mapperMock = new Mock<IMapper>();

            _orderController = new OrderController(
                _orderRepositoryMock.Object,
                _signInManagerMock.Object,
                _cartRepoMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllOrderAync_ReturnOkResult_WithOrderList()
        {
            
            var response = new ResponseDto
            {
                IsSuccess = true,
                Message = "Success",
                Result = new List<OrderDto> { new OrderDto { OrderId = 1, Total = 100 } }
            };
            _orderRepositoryMock.Setup(repo => repo.GetAllOrderAync()).ReturnsAsync(response);

            
            var result = await _orderController.GetAllOrderAync();

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(responseDto.IsSuccess);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Result, responseDto.Result);
        }

        [Fact]
        public async Task AddOrderAsync_ValidOrder_ReturnOkResult()
        {
            
            var user = new User { Id = "user-id", Email = "user@gmail.com" };
            _signInManagerMock.Setup(sm => sm.UserManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var cartResponse = new ResponseDto
            {
                IsSuccess = true,
                Message = "Cart items fetched successfully",
                Result = new List<CartResponseDto> { new CartResponseDto { ProductId = 1, ProductName = "Sample Product", Quantity = 1 } }
            };
            _cartRepoMock.Setup(repo => repo.GetCart(It.IsAny<User>())).ReturnsAsync(cartResponse);

            var orderDto = new OrderDto { FullName = "John Doe", Total = 2000 };
            var order = new Order { OrderId = 1, UserId = user.Id, Total = 2000 };
            _mapperMock.Setup(m => m.Map<Order>(orderDto)).Returns(order);

            _orderRepositoryMock.Setup(repo => repo.AddOrder(It.IsAny<Order>())).ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.AddOrderDetails(It.IsAny<IEnumerable<OrderDetails>>()));

            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Message = "Items removed successfully"
            };
            _cartRepoMock.Setup(repo => repo.RemoveAllCartItem(It.IsAny<string>())).ReturnsAsync(responseDto);

            
            var result = await _orderController.AddOrderAsync(orderDto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(order.OrderId, response.Result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderExists_ReturnOkResult()
        {
            
            var order = new OrderDto { OrderId = 1, Total = 100 };
            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Message = "Order found",
                Result = order
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(responseDto);

            
            var result = await _orderController.GetOrderByIdAsync(1);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.Equal(responseDto.Result, returnedDto.Result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderDoesNotExist_ReturnNotFoundResult()
        {
            
            var responseDto = new ResponseDto
            {
                IsSuccess = false,
                Message = "Order not found",
                Result = null
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(2)).ReturnsAsync(responseDto);

            
            var result = await _orderController.GetOrderByIdAsync(2);

            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(notFoundResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Null(response.Result);
        }

        [Fact]
        public async Task CalculateProfitAsync_ReturnOkResult_WithProfitValue()
        {
            
            var profit = 10000m;
            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Message = "Profit calculated successfully",
                Result = profit
            };

            _orderRepositoryMock.Setup(repo => repo.CalculateProfitAsync()).ReturnsAsync(responseDto);

            
            var result = await _orderController.CalculateProfitAsync();

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.Equal(profit, returnedDto.Result);
        }

        [Fact]
        public async Task UpdateOrderStatus_OrderExists_ReturnOkResult()
        {
            
            var orderId = 1;
            var statusMessage = "Shipped";
            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Message = "Order status updated successfully",
                Result = true
            };

            _orderRepositoryMock.Setup(repo => repo.UpdateOrderStatus(orderId, statusMessage)).ReturnsAsync(responseDto);

            
            var result = await _orderController.UpdateOrderStatus(orderId, statusMessage);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(returnedDto.IsSuccess);
        }

        [Fact]
        public async Task UpdateOrderStatus_OrderDoesNotExist_ReturnNotFoundResult()
        {
            
            var orderId = 2;
            var statusMessage = "Shipped";
            var responseDto = new ResponseDto
            {
                IsSuccess = false,
                Message = "Order not found",
                Result = null
            };

            _orderRepositoryMock.Setup(repo => repo.UpdateOrderStatus(orderId, statusMessage)).ReturnsAsync(responseDto);

            
            var result = await _orderController.UpdateOrderStatus(orderId, statusMessage);

            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(notFoundResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Null(response.Result);
        }

    }
}
