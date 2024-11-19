using Moq;
using NanaFoodWeb.Controllers;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

namespace NaNaTest
{
    public class ManageOrderControllerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IReviewRepository> _mockReviewRepository;
        private readonly ManageOrderController _controller;

        public ManageOrderControllerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockReviewRepository = new Mock<IReviewRepository>();
            _controller = new ManageOrderController(_mockOrderRepository.Object, _mockReviewRepository.Object);
        }

        [Fact]
        public async Task Index_Should_Return_View_With_Orders()
        {
            // Arrange
            var orderList = new List<OrderDto>
            {
                new OrderDto { OrderStatus = "Chờ xác nhận" },
                new OrderDto { OrderStatus = "Đang giao" }
            };
            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(orderList)
            };
            _mockOrderRepository.Setup(repo => repo.GetAllOrderAsync()).ReturnsAsync(response);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["ConfirmedYet"]);
            Assert.NotNull(viewResult.ViewData["Delivering"]);
        }

        [Fact]
        public async Task Details_Should_Return_View_When_Order_Found()
        {
            // Arrange
            int orderId = 1;
            var order = new OrderDto { OrderId = orderId, OrderStatus = "Chờ xác nhận" };
            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(order)
            };
            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(response);
            var responseProductFromOrderDetails = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(new List<ReviewProductDto>())
            };
            _mockReviewRepository.Setup(repo => repo.GetOrderDetailsFromOrder(orderId)).ReturnsAsync(responseProductFromOrderDetails);

            // Act
            var result = await _controller.Details(orderId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OrderDto>(viewResult.Model);
            Assert.Equal(orderId, model.OrderId);
        }

        //[Fact]
        //public async Task ModifyDelveringStatus_Should_Return_Success_When_Status_Updated()
        //{
        //    // Arrange
        //    int orderId = 1;
        //    string message = "Đang giao";
        //    var response = new ResponseDto { IsSuccess = true };
        //    _mockOrderRepository.Setup(repo => repo.UpdateOrderStatus(orderId, message)).ReturnsAsync(response);

        //    // Act
        //    var result = await _controller.ModifyDelveringStatus(orderId, message);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //    Assert.Equal("ManageOrder", redirectResult.ControllerName);
        //}

        //[Fact]
        //public async Task ModifyDelveringStatus_Should_Return_Error_When_Failure()
        //{
        //    // Arrange
        //    int orderId = 1;
        //    string message = "Invalid Status";
        //    var response = new ResponseDto { IsSuccess = false };
        //    _mockOrderRepository.Setup(repo => repo.UpdateOrderStatus(orderId, message)).ReturnsAsync(response);

        //    // Act
        //    var result = await _controller.ModifyDelveringStatus(orderId, message);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //    Assert.Equal("ManageOrder", redirectResult.ControllerName);
        //}

        [Fact]
        public async Task GetPreparingOrders_Should_Return_PreparingOrders_As_Json()
        {
            // Arrange
            var orderList = new List<OrderDto>
            {
                new OrderDto { OrderStatus = "Đang chuẩn bị" },
                new OrderDto { OrderStatus = "Chờ xác nhận" }
            };
            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(orderList)
            };
            _mockOrderRepository.Setup(repo => repo.GetAllOrderAsync()).ReturnsAsync(response);

            // Act
            var result = await _controller.GetPreparingOrders();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var preparingOrders = Assert.IsAssignableFrom<List<OrderDto>>(jsonResult.Value);
            Assert.Single(preparingOrders);
        }
    }
}
