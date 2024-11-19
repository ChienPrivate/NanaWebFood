using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.Controllers;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;
using NanaFoodWeb.Models;

namespace NaNaTest
{
    public class DashBoardControllerTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepo;
        private readonly Mock<IHelperRepository> _mockHelperRepo;
        private readonly Mock<ITokenProvider> _mockTokenProvider;
        private readonly Mock<IDashBoardRepository> _mockDashBoardRepo;
        private readonly DashBoardController _controller;

        public DashBoardControllerTests()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockHelperRepo = new Mock<IHelperRepository>();
            _mockTokenProvider = new Mock<ITokenProvider>();
            _mockDashBoardRepo = new Mock<IDashBoardRepository>();
            _controller = new DashBoardController(
                _mockAuthRepo.Object,
                _mockHelperRepo.Object,
                _mockTokenProvider.Object,
                _mockDashBoardRepo.Object
            );
        }

        //[Fact]
        //public async Task Index_ReturnsRedirectToManageOrder_WhenRoleIsEmployee()
        //{
        //    // Arrange
        //    _mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock-token");
        //    _mockTokenProvider.Setup(tp => tp.ReadToken("role", "mock-token")).Returns("employee");

        //    // Act
        //    var result = await _controller.Index();

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //    Assert.Equal("ManageOrder", redirectResult.ControllerName);
        //}

        [Fact]
        public async Task Index_ReturnsView_WhenRoleIsAdmin()
        {
            // Arrange
            _mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock-token");
            _mockTokenProvider.Setup(tp => tp.ReadToken("role", "mock-token")).Returns("admin");
            _mockDashBoardRepo.Setup(repo => repo.GetProfitByMonthAsync(It.IsAny<int>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Result = 1000 });
            _mockDashBoardRepo.Setup(repo => repo.GetProfitByYearAsync(It.IsAny<int>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Result = 12000 });
            _mockDashBoardRepo.Setup(repo => repo.GetDeliveringOrderAsync())
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(new List<Order> { new Order(), new Order() }) });
            _mockDashBoardRepo.Setup(repo => repo.GetCompleteOrderInMonthAsync(It.IsAny<int>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(new List<Order> { new Order() }) });
            _mockDashBoardRepo.Setup(repo => repo.GetProfitEachMonth(It.IsAny<int>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(new List<LineChartDto> { new LineChartDto { Revenue = 1000 } }) });

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Should return the default view
        }

        [Fact]
        public async Task AdministratorInformation_ReturnsView_WithViewModel()
        {
            // Arrange
            var mockUser = new UserDto { FullName = "Test User" };
            var response = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(mockUser) };
            _mockAuthRepo.Setup(repo => repo.GetInfo()).ReturnsAsync(response);

            // Act
            var result = await _controller.AdministratorInformation();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChangePassAndUserDto>(viewResult.Model);
            Assert.Equal("Test User", model.UserDto.FullName);
        }

        //[Fact]
        //public async Task AdministratorInformation_Post_ReturnsView_OnSuccess()
        //{
        //    // Arrange
        //    var viewModel = new ChangePassAndUserDto
        //    {
        //        UserDto = new UserDto { FullName = "Updated User" }
        //    };

        //    var response = new ResponseDto { IsSuccess = true, Result = JsonConvert.SerializeObject(viewModel.UserDto), Message = "Update successful" };
        //    _mockAuthRepo.Setup(repo => repo.UpdateInfo(viewModel.UserDto)).ReturnsAsync(response);

        //    // Act
        //    var result = await _controller.AdministratorInformation(viewModel, null);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<ChangePassAndUserDto>(viewResult.Model);
        //    Assert.Equal("Update successful", _controller.TempData["success"]);
        //    Assert.Equal("Updated User", model.UserDto.FullName);
        //}

        [Fact]
        public async Task AdministratorInformation_Post_ReturnsView_OnFailure()
        {
            // Arrange
            var viewModel = new ChangePassAndUserDto
            {
                UserDto = new UserDto { FullName = "Test User" }
            };

            var response = new ResponseDto { IsSuccess = false };
            _mockAuthRepo.Setup(repo => repo.UpdateInfo(viewModel.UserDto)).ReturnsAsync(response);

            // Act
            var result = await _controller.AdministratorInformation(viewModel, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChangePassAndUserDto>(viewResult.Model);
            Assert.Equal("Test User", model.UserDto.FullName);
        }
    }
}
