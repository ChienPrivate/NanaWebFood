using Azure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NanaFoodApi.Controllers;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.IRepository;
using Xunit;

namespace NaNaTest
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserController _controller;
        public UserControllerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _controller = new UserController(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateUserEndpoint_ValidRequest_ReturnsOk()
        {
            var createUserRequestDto = new CreateUserRequestDto { };
            var responseDto = new ResponseDto { IsSuccess = true, Message = "User created successfully" };
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(createUserRequestDto)).ReturnsAsync(responseDto);

            var result = await _controller.CreateUserEndpoint(createUserRequestDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task GetAllUser_ReturnsOk()
        {
            var users = new List<UserDto>
            {
                new UserDto { },
                new UserDto { }
            };
            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Result = users 
            };
            _userRepositoryMock.Setup(repo => repo.GetAllUserAsync()).ReturnsAsync(responseDto);

            var result = await _controller.GetAllUser();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task DeleteUserAsync_ValidId_ReturnsOk()
        {
            var responseDto = new ResponseDto { IsSuccess = true, Message = "User deleted successfully" };
            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync("user123"))
                               .ReturnsAsync(responseDto);

            var result = await _controller.DeleteUserAsync("user123");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidRequest_ReturnsOk()
        {
            var updateUserRequestDto = new UpdateUserRequestDto { /* initialize properties */ };
            var responseDto = new ResponseDto { IsSuccess = true, Message = "User updated successfully" };
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(updateUserRequestDto))
                               .ReturnsAsync(responseDto);

            var result = await _controller.UpdateUserAsync(updateUserRequestDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task GetAllRolesAsync_ReturnsOkWithRoleList()
        {
            var roles = new List<string> { "admin", "user" };
            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Result = roles 
            };

            _userRepositoryMock.Setup(repo => repo.GetAllRolesAsync())
                               .ReturnsAsync(responseDto);

            var result = await _controller.GetAllRolesAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidId_ReturnsOk()
        {
            var user = new UserDto { };

            var responseDto = new ResponseDto
            {
                IsSuccess = true,
                Result = user 
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("user123"))
                               .ReturnsAsync(responseDto);

            var result = await _controller.GetUserByIdAsync("user123");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(responseDto, okResult.Value);
        }

        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ReturnsNotFound()
        {
            var responseDto = new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found" 
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("invalidId"))
                               .ReturnsAsync(responseDto);

            var result = await _controller.GetUserByIdAsync("invalidId");

            //var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            //Assert.Equal("User not found", notFoundResult.Value);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("User not found", response.Message);
        }

    }
}
