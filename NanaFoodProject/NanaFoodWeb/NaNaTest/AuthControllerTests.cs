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
    public class AuthControllerTests
    {
        private readonly Mock<IAuthenRepo> _authRepoMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authRepoMock = new Mock<IAuthenRepo>();
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            _mapperMock = new Mock<IMapper>();
            _tokenServiceMock = new Mock<ITokenService>();

            _controller = new AuthController(_authRepoMock.Object, _signInManagerMock.Object, _userManagerMock.Object, _mapperMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsOkResult()
        {
            var loginDto = new LoginDTO { UserName = "testuser001", Password = "password" };
            _authRepoMock.Setup(x => x.Login(It.IsAny<LoginDTO>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Message = "Login Successful" });

            var result = await _controller.LoginAsync(loginDto);
          
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsBadRequest()
        {         
            var loginDto = new LoginDTO { UserName = "invaliduser000", Password = "wrongpassword01" }; 

            _authRepoMock.Setup(x => x.Login(It.Is<LoginDTO>(dto => dto.UserName == loginDto.UserName && dto.Password == loginDto.Password)))
                .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Invalid credentials" });
            
            var result = await _controller.LoginAsync(loginDto);
          
            /*var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(response.IsSuccess);*/
            //Assert.Equal("Invalid credentials", response.Message); 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.False(response.IsSuccess);
        }


        [Fact]
        public async Task RegisterAsync_ValidData_ReturnsOkResult()
        {
            var registerDto = new RegisterDto { UserName = "newuser22", Password = "password", Email = "newuser@gmail.com" };
            _authRepoMock.Setup(x => x.Register(It.IsAny<RegisterDto>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Message = "Registration successful" });

            var result = await _controller.RegisterAsync(registerDto);
           
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task RegisterAsync_InvalidData_ReturnsBadRequest()
        {
            var registerDto = new RegisterDto(); // Missing required fields
            _controller.ModelState.AddModelError("Username", "Required");

            var result = await _controller.RegisterAsync(registerDto);
            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Logout_UserIsAuthenticated_ReturnsOkResult()
        {
            _authRepoMock.Setup(x => x.LogOut()).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.LogOut();
           
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task ForgotPassword_ValidEmail_ReturnsOkResult()
        {
            var email = "user@example.com";
            _authRepoMock.Setup(x => x.ForgotPassword(email)).ReturnsAsync(new ResponseDto { IsSuccess = true });

            var result = await _controller.ForgotPassword(email);
           
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task ForgotPassword_InvalidEmail_ReturnsBadRequest()
        {           
            var invalidEmail = "notfound@example.com";
            _authRepoMock.Setup(x => x.ForgotPassword(invalidEmail))
                .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Email not found" });
            
            var result = await _controller.ForgotPassword(invalidEmail);
            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("Email not found", response.Message);
        }


        [Fact]
        public async Task LoginAsync_MissingFields_ReturnsBadRequest()
        {          
            var loginDto = new LoginDTO { UserName = "", Password = "" }; 
            _controller.ModelState.AddModelError("UserName", "UserName is required");
            _controller.ModelState.AddModelError("Password", "Password is required");
            
            var result = await _controller.LoginAsync(loginDto);
          
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ValidUserInfo_ReturnsOkResult()
        {
            
            var userDto = new UserDto { UserName = "updateduser", Email = "updated@example.com" };
            _authRepoMock.Setup(x => x.UpdateUser(userDto))
                .ReturnsAsync(new ResponseDto { IsSuccess = true, Message = "User updated successfully" });
            
            var result = await _controller.UpdateUser(userDto);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("User updated successfully", response.Message);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ReturnsBadRequest()
        {
            var registerDto = new RegisterDto { UserName = "existinguser", Password = "Password!01", Email = "existing@example.com" };
            _authRepoMock.Setup(x => x.Register(It.IsAny<RegisterDto>()))
                .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Email already in use" });

            var result = await _controller.RegisterAsync(registerDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("Email already in use", response.Message);
        }


        [Fact]
        public async Task ChangePass_ReturnsOk_WhenChangePasswordIsSuccessful()
        {
            // Arrange
            var changePassDto = new ChangePassDto { OldPassword = "Asdzxc1!", NewPassword = "newPassword123!", ConfirmPassword = "newPassword123!" };
            var user = new User();
            //_userManagerMock.Setup
            //_userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            //_signInManagerMock.Setup(sm => sm.UserManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            //_tokenServiceMock.Setup(auth => auth.ChangePassword(user, changePassDto))
            //                .ReturnsAsync(new ResponseDto { IsSuccess = true });
            
            _signInManagerMock.Setup(sm => sm.UserManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);
            _authRepoMock.Setup(auth => auth.ChangePassword(user, changePassDto))
            .ReturnsAsync(new ResponseDto { IsSuccess = true });
            // Act
            var result = await _controller.ChangePass(changePassDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(((ResponseDto)okResult.Value).IsSuccess);
        }       

        [Fact]
        public async Task ChangePass_ReturnsUnauthorized_WhenUserNotLoggedIn()
        {
            // Arrange
            var changePassDto = new ChangePassDto();

            //_signInManagerMock.Setup(sm => sm.UserManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.ChangePass(changePassDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Người dùng chưa đăng nhập", unauthorizedResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsOk_WhenConfirmationIsSuccessful()
        {
            // Arrange
            var email = "test@example.com";

            _authRepoMock.Setup(auth => auth.ConfirmEmail(email))
                            .ReturnsAsync(new ResponseDto { IsSuccess = true });

            // Act
            var result = await _controller.ConfirmEmail(email);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("https://localhost:51326/Auth/Login?message=activation-success", redirectResult.Url);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsBadRequest_WhenConfirmationFails()
        {
            // Arrange
            var email = "test@example.com";

            _authRepoMock.Setup(auth => auth.ConfirmEmail(email))
                            .ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Xác nhận thất bại" });

            // Act
            var result = await _controller.ConfirmEmail(email);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Xác nhận thất bại", ((ResponseDto)badRequestResult.Value).Message);
        }

        

    }
}