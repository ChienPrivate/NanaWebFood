using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class AuthenRepo : IAuthenRepo
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        ResponseDto response = new ResponseDto();

        public AuthenRepo(SignInManager<User> signInManager, UserManager<User> userManager, ITokenService tokenService, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
        }

        public async Task<ResponseDto> Login(LoginDTO login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Tài khoản hoặc mật khẩu không đúng";
                return response;
            }
            var authen = new AuthenticationProperties() { IsPersistent = login.keepLogined };
            // first params : người dùng có tên đăng nhập là login.UserName (object)
            // second params : mật khẩu mà người dùng nhập  (string)
            // third params : ghi nhớ trạng thái đăng nhập (bool)
            // fourth params : mặc định nếu đăng nhập sai 5 lần liên tiếp sẽ bị khoá đăng nhập trong 5 phút
            var result = await _signInManager.PasswordSignInAsync(user, login.Password, authen.IsPersistent, false);

            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Message = "Tài khoản hoặc mật khẩu không đúng";
                return response;
            }
            var roles = await _userManager.GetRolesAsync(user);
            response.Result = new UserReturn
            {
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Token = _tokenService.CreateToken(user, roles)
            };
            response.Message = "Đăng nhập thành công";
            return response;
        }

        public async Task<ResponseDto> Register(RegisterDto regis)
        {
            try
            {
                var user = new User
                {
                    UserName = regis.UserName,
                    Email = regis.Email,
                    FullName = regis.FullName
                };

                var createdUser = await _userManager.CreateAsync(user, regis.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "customer");
                    if (roleResult.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        response.Result = new UserReturn
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            FullName = user.FullName,
                            Token = _tokenService.CreateToken(user, roles)
                        };
                        response.Message = "Đăng ký tài khoản thành công";
                        return response;
                    }
                    response.IsSuccess = false;
                    response.Message = "Đăng ký thất bại";
                    return response;
                }
                response.IsSuccess = false;
                response.Message = "Đăng ký thất bại";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }
    }
}
