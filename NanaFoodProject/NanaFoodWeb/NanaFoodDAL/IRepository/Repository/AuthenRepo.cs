using AutoMapper;
using Azure.Core;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.Helper;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
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
        private readonly EmailPoster _emailPoster;
        ResponseDto response = new ResponseDto();

        public AuthenRepo(SignInManager<User> signInManager,
            UserManager<User> userManager, 
            ITokenService tokenService, 
            IConfiguration config, 
            ApplicationDbContext context, 
            IMapper mapper,
            EmailPoster emailPoster)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _mapper = mapper;
            _context = context;
            _emailPoster = emailPoster;
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
                var userEmail = await _userManager.FindByEmailAsync(regis.Email);

                if (userEmail != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Email này đã được sử dụng";
                    return response;
                }

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
                        var template = _emailPoster.EmailConfirmTemplate($"https://localhost:7094/api/Auth/EmailConfirmation/{Uri.EscapeDataString(user.Email)}");
                        var sendmailResult = _emailPoster.SendMail(user.Email,"Xác thực email",template);
                        if (sendmailResult != "gửi mail thành công")
                        {
                            response.IsSuccess = false;
                            response.Message = "Xảy ra lỗi xảy ra khi gửi mail";
                            return response;
                        }
                        return response;
                    }
                    response.IsSuccess = false;
                    response.Message = "Có lỗi xảy ra khi xác nhận role";
                    return response;
                }
                response.IsSuccess = false;
                response.Message = "Tên đăng nhập đã được sử dụng";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                response.Message = "Đăng xuất thành công";
            } catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Lỗi : "+ ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> ChangePassword(User user,ChangePassDto changePass)
        {
            try
            {

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePass.OldPassword, changePass.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    response.IsSuccess = false;
                    string error = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    if (error == "Incorrect password.")
                    {
                        response.Message = "Mật khẩu hiện tại không chính xác";
                    }
                    else
                    {
                        response.Message = error;
                    }
                    return response;
                }

                await _signInManager.RefreshSignInAsync(user);
                response.Message = "Mật khẩu đã được cập nhật";;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Lỗi : {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> ConfirmEmail(string email)
        {
            try
            {
                var euser = await _userManager.FindByEmailAsync(email);
                if (euser != null)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(euser);
                    await _userManager.ConfirmEmailAsync(euser, token);
                    response.IsSuccess = true;
                    response.Message = "Xác thực thành công";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Xác thực thất bại kiểm tra lại email";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> CheckEmailConfirm(User user)
        {
            if (user.EmailConfirmed)
            {
                response.IsSuccess = true;
                return response;
            }
            response.IsSuccess = false;
            response.Message = "Email chưa được xác thực";
            return response;
        }

        public async Task<ResponseDto> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    response.IsSuccess= false;
                    response.Message = "Người dùng không tồn tại hoặc chưa kích hoạt email";
                    return response;
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, GenerateRandomPassword());
                if (result.Succeeded)
                {
                    var template = _emailPoster.EmailConfirmTemplate($"https://localhost:7094/api/Auth/EmailConfirmation/{Uri.EscapeDataString(user.Email)}");
                    var sendmailResult = _emailPoster.SendMail(user.Email, "Khôi phục mật khẩu", template);
                    if (sendmailResult != "gửi mail thành công")
                    {
                        response.IsSuccess = false;
                        response.Message = "Xảy ra lỗi xảy ra khi gửi mail";
                        return response;
                    }
                    response.IsSuccess = true;
                    response.Message = "Mật khẩu mới đã được gửi về email của bạn";
                    return response;
                }
                response.IsSuccess = false;
                response.Message = string.Join(",",result.Errors.Select(error => error.Description));
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            
        }

        private string GenerateRandomPassword()
        {
            Random random = new Random();
            string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";
            string specialChars = "!@#$%^&*()";
            string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";

            char[] result = new char[6];

            // Đảm bảo có ít nhất 1 ký tự in hoa, 1 chữ số, 1 ký tự đặc biệt
            result[0] = uppercase[random.Next(uppercase.Length)];
            result[1] = digits[random.Next(digits.Length)];
            result[2] = specialChars[random.Next(specialChars.Length)];

            // Tạo 3 ký tự ngẫu nhiên từ tất cả các ký tự
            for (int i = 3; i < 6; i++)
            {
                result[i] = allChars[random.Next(allChars.Length)];
            }

            // Trộn lại các ký tự ngẫu nhiên
            result = result.OrderBy(x => random.Next()).ToArray();

            return new string(result);
        }

        //public async Task<ResponseDto> DeleteUser(string email)
        //{
        //    try
        //    {
        //        var eUser = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
        //        if (eUser != null)
        //        {
        //            _context.Users.Remove(eUser);
        //            _context.SaveChanges();
        //        }
        //        response.IsSuccess = false;
        //        response.Message = "Người dùng không tồn tại trong hệ thống.";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        //public async Task<ResponseDto> GetAllUser(int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var getAllU = _context.Users.ToList();
        //        var totalUser = getAllU.Count;
        //        var totalPages = (int)Math.Ceiling((decimal)totalUser / pageSize);

        //        var users = getAllU
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();
        //        response.Result = new
        //        {
        //            TotalCount = totalUser,
        //            TotalPages = totalPages,
        //            Users = _mapper.Map<List<UserDto>>(users)

        //        };
        //        response.IsSuccess = true;
        //        response.Message = "Lấy danh sách người dùng thành công.";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = $"Lỗi : {ex.Message}";
        //    }

        //    return response;

        //}

        //public async Task<ResponseDto> SearchMail(string email, int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var lowerEmail = email.ToLower();
        //        var userEmail = await _context.Users.Where(x => x.Email.ToLower().Contains(lowerEmail)).ToListAsync();
        //        if (string.IsNullOrEmpty(email))
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Email không được để trống.";
        //            return response;
        //        }
        //        if (lowerEmail == null)
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Email này không tồn tại trong hệ thống.";
        //        }
        //        else
        //        {
        //            var totalUser = userEmail.Count;
        //            var totalPage = (int)Math.Ceiling((decimal)totalUser / pageSize);

        //            var UserPage = userEmail
        //                .Skip((page - 1) * pageSize)
        //                .Take(pageSize)
        //                .ToList();

        //            response.Result = new
        //            {
        //                TotalCount = totalUser,
        //                TotalPages = totalPage,

        //                Users = _mapper.Map<List<UserDto>>(UserPage)
        //            };
        //            response.IsSuccess = true;
        //            response.Message = "Lấy tên người dùng thành công ";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = $"Lỗi : {ex.Message}";
        //    }
        //    return response;
        //}

        //public async Task<ResponseDto> SearchName(string fullname, int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var lowerName = fullname.ToLower();
        //        if (string.IsNullOrEmpty(lowerName))
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Vui lòng nhập tên người dùng.";
        //        }

        //        var usersName = await _context.Users.Where(x => x.FullName.ToLower().Contains(lowerName)).ToListAsync();
        //        if (usersName == null)
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Người dùng không tồn tại trong hệ thống.";
        //        }
        //        else
        //        {
        //            var totalUser = usersName.Count;
        //            var totalPage = (int)Math.Ceiling((decimal)totalUser / pageSize);

        //            var NamePage = usersName
        //                .Skip((page - 1) * pageSize)
        //                .Take(pageSize)
        //                .ToList();

        //            response.Result = new
        //            {
        //                TotalCount = totalUser,
        //                TotalPage = totalPage,
        //                Users = _mapper.Map<List<UserDto>>(NamePage)
        //            };

        //            response.IsSuccess = true;
        //            response.Message = "Lấy tên người dùng thành công";

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = $"Lỗi : {ex.Message}";
        //    }
        //    return response;


        //}

    }
}
