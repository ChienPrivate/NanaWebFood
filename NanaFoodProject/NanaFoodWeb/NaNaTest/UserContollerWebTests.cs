using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodWeb.Controllers;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using Xunit;

namespace NaNaTest
{
    public class UsersControllerWebTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IHelperRepository> _mockHelperRepository;
        private readonly UsersController _controller;

        public UsersControllerWebTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockHelperRepository = new Mock<IHelperRepository>();
            _controller = new UsersController(_mockUserRepository.Object, _mockHelperRepository.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithUserLists()
        {
            // Arrange
            var users = new List<UserWithRolesDto>
            {
                new UserWithRolesDto { Id = "1", Roles = "admin" },
                new UserWithRolesDto { Id = "2", Roles = "employee" },
                new UserWithRolesDto { Id = "3", Roles = "customer" }
            };
            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(users)
            };
            _mockUserRepository.Setup(repo => repo.GetAllUserAsync()).ReturnsAsync(response);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(users.Count, ((List<UserWithRolesDto>)viewResult.ViewData["Admin"]).Count +
                                      ((List<UserWithRolesDto>)viewResult.ViewData["Customer"]).Count +
                                      ((List<UserWithRolesDto>)viewResult.ViewData["Employee"]).Count);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WithEmptyModel()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetAllRolesAsync()).ReturnsAsync(new ResponseDto
            {
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(new List<IdentityRole>
                {
                    new IdentityRole { Name = "admin" },
                    new IdentityRole { Name = "employee" }
                })
            });

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateUserRequestDto>(viewResult.Model);
            Assert.NotNull(viewResult.ViewData["RoleList"]);
            Assert.NotNull(viewResult.ViewData["StatusList"]);
        }

        //[Fact]
        //public async Task Edit_ReturnsViewResult_WhenUserExists()
        //{
        //    // Arrange
        //    var userId = "b9f3a995-cd27-43b3-98fc-5aafe0c4955f";
        //    var user = new UpdateUserRequestDto { Id = userId };
        //    var response = new ResponseDto
        //    {
        //        IsSuccess = true,
        //        Result = JsonConvert.SerializeObject(user)
        //    };
        //    _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(response);

        //    // Act
        //    var result = await _controller.Edit(userId);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<UpdateUserRequestDto>(viewResult.Model);
        //    Assert.Equal(userId, model.Id);
        //}

        //[Fact]
        //public async Task Edit_ReturnsRedirectToAction_WhenUserDoesNotExist()
        //{
        //    // Arrange
        //    var userId = "invalidId";
        //    _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new ResponseDto { IsSuccess = false });

        //    // Act
        //    var result = await _controller.Edit(userId);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //}

        //[Fact]
        //public async Task DeleteConfirm_ReturnsRedirectToAction_OnSuccess()
        //{
        //    // Arrange
        //    var userId = "1";
        //    _mockUserRepository.Setup(repo => repo.DeleteUserAsync(userId)).ReturnsAsync(new ResponseDto { IsSuccess = true });

        //    // Act
        //    var result = await _controller.DeleteConfirm(userId);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //}

        [Fact]
        public async Task DeleteConfirm_ReturnsNotFound_OnFailure()
        {
            // Arrange
            var userId = "1";
            _mockUserRepository.Setup(repo => repo.DeleteUserAsync(userId)).ReturnsAsync(new ResponseDto
            {
                IsSuccess = false,
                Message = "Error occurred"
            });

            // Act
            var result = await _controller.DeleteConfirm(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Error occurred", notFoundResult.Value);
        }
    }
}
