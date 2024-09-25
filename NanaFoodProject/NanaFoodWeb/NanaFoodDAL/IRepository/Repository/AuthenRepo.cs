using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NanaFoodDAL.Context;
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
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        ResponseDto response = new ResponseDto();


        public AuthenRepo(SignInManager<User> signInManager, UserManager<User> userManager, ITokenService tokenService, IConfiguration config, ApplicationDbContext context, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseDto> DeleteUser(string email)
        {
            try
            {
                var eUser = await _context.Users.FirstOrDefaultAsync(e => e.Email == email); 
                if (eUser != null)
                {
                    _context.Users.Remove(eUser);
                    _context.SaveChanges();
                }
                response.IsSuccess = false;
                response.Message = "Người dùng không tồn tại trong hệ thống."; 
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response; 
        }

        public async Task<ResponseDto> GetAllUser(int page = 1 , int pageSize = 10)
        {
            try
            {
                var getAllU = _context.Users.ToList();
                var totalUser = getAllU.Count;
                var totalPages = (int)Math.Ceiling((decimal)totalUser / pageSize);

                var users = getAllU
                    .Skip((page - 1) * pageSize) 
                    .Take(pageSize)
                    .ToList();
                response.Result = new
                {
                    TotalCount = totalUser,
                    TotalPages = totalPages,
                    Users = _mapper.Map<List<UserDto>>(users)

                };
                response.IsSuccess = true;
                response.Message = "Lấy danh sách người dùng thành công.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }

            return response;

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

        public async Task<ResponseDto> SearchMail(string email, int page= 1, int pageSize= 10)
        {
            try
            {
               var lowerEmail = email.ToLower();
                var userEmail = await _context.Users.Where(x => x.Email.ToLower().Contains(lowerEmail)).ToListAsync();
                if (string.IsNullOrEmpty(email))
                {
                    response.IsSuccess = false;
                    response.Message = "Email không được để trống.";
                    return response;
                }
                if (lowerEmail == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Email này không tồn tại trong hệ thống.";
                }
                else
                {
                    var totalUser = userEmail.Count;
                    var totalPage = (int)Math.Ceiling((decimal)totalUser / pageSize);

                    var UserPage = userEmail
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    response.Result = new
                    {
                        TotalCount = totalUser,
                        TotalPages = totalPage,

                        Users = _mapper.Map<List<UserDto>>(UserPage)
                    };
                    response.IsSuccess = true;
                    response.Message = "Lấy tên người dùng thành công ";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> SearchName(string fullname, int page = 1, int pageSize = 10)
        {
            try
            {
                var lowerName = fullname.ToLower();
                if (string.IsNullOrEmpty(lowerName))
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng nhập tên người dùng.";
                }

                var usersName = await _context.Users.Where(x => x.FullName.ToLower().Contains(lowerName)).ToListAsync();
                if(usersName == null)
                {
                    response.IsSuccess = false;
                    response.Message ="Người dùng không tồn tại trong hệ thống."; 
                }
                else
                {
                    var totalUser = usersName.Count;
                    var totalPage = (int)Math.Ceiling((decimal)totalUser / pageSize);

                    var NamePage = usersName
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    response.Result = new
                    {
                        TotalCount = totalUser,
                        TotalPage = totalPage,
                        Users = _mapper.Map<List<UserDto>>(NamePage)
                    }; 

                    response.IsSuccess = true;
                    response.Message = "Lấy tên người dùng thành công"; 

                }  
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
