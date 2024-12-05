using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.IRepository.Repository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenRepo _auth;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthenRepo auth, SignInManager<User> signInManager, UserManager<User> userManager, IMapper mapper, ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auth = auth;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng đăng nhập bằng cách cung cấp tên đăng nhập và mật khẩu.
        /// Nếu thông tin đăng nhập chính xác, trả về JWT token cho phép xác thực các API khác.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "UserName": "testaccount",
        ///     "Password": "Asdzxc1!",
        ///     "keepLogined": true
        /// }
        /// ```
        /// </remarks>
        /// <param name="login">Thông tin đăng nhập.</param>
        /// <returns>
        /// - 200 OK nếu đăng nhập thành công và trả về JWT token.
        /// - 400 BadRequest nếu thông tin đăng nhập không hợp lệ.
        /// - 500 Internal Server Error nếu xảy ra lỗi từ phía server.
        /// </returns>
        /// <response code="200">Đăng nhập thành công, trả về JWT token.</response>
        /// <response code="400">Thông tin đăng nhập không hợp lệ.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _auth.Login(login);
            /*if (!response.IsSuccess)
            {
                return BadRequest(response);
            }*/
            return Ok(response);
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng mới đăng ký tài khoản bằng cách cung cấp thông tin như tên đăng nhập, mật khẩu, email và tên đầy đủ.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "UserName": "testaccount",
        ///     "Password": "Asdzxc1!",
        ///     "Email": "abcxyc@gmail.com",
        ///     "FullName" : "Nguyễn Văn An"
        /// }
        /// ```
        /// </remarks>
        /// <param name="regis">Thông tin đăng ký.</param>
        /// <returns>
        /// - 201 Created nếu đăng ký thành công và trả về thông tin người dùng.
        /// - 400 BadRequest nếu thông tin không hợp lệ hoặc email đã tồn tại.
        /// - 500 Internal Server Error nếu xảy ra lỗi từ phía server.
        /// </returns>
        /// <response code="201">Đăng ký thành công, trả về thông tin người dùng đã tạo.</response>
        /// <response code="400">Thông tin đăng ký không hợp lệ hoặc email đã tồn tại.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto regis)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _auth.Register(regis);
            /*if (!response.IsSuccess)
            {
                return BadRequest(response);
            }*/
            return Ok(response);
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng đăng xuất tài khoản hiện tại.
        /// </remarks>
        /// <returns>200 OK nếu đăng xuất thành công.</returns>
        /// <response code="200">Đăng xuất thành công.</response>
        /// <response code="400">Có lỗi xảy ra.</response>
        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var response = await _auth.LogOut();
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Đổi mật khẩu của người dùng đã đăng nhập
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng thay đổi mật khẩu hiện tại của họ.
        /// Người dùng cần cung cấp mật khẩu cũ và mật khẩu mới.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "OldPassword": "Asdzxc1!",
        ///     "NewPassword": "newPassword123!",
        ///     "ConfirmPassword" : "newPassword123!"
        /// }
        /// ```
        /// </remarks>
        /// <param name="changepass">Thông tin mật khẩu cần thay đổi</param>
        /// <returns>
        /// - 200 OK nếu đổi mật khẩu thành công.
        /// - 400 BadRequest nếu mật khẩu cũ không chính xác hoặc yêu cầu không hợp lệ.
        /// - 401 Unauthorized nếu người dùng chưa xác thực.
        /// - 500 Internal Server Error nếu xảy ra lỗi từ phía server.
        /// </returns>
        /// <response code="200">Đổi mật khẩu thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc mật khẩu cũ không chính xác.</response>
        /// <response code="401">Người dùng chưa đăng nhập hoặc token không hợp lệ.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePass(ChangePassDto changepass)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _signInManager.UserManager.GetUserAsync(User);
            
            if (user == null)
            {
                return Unauthorized("Người dùng chưa đăng nhập");
            }
            var response = await _auth.ChangePassword(user, changepass);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Xác nhận email
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng xác nhận email để kích hoạt tài khoản.
        /// </remarks>
        /// <param name="email">Email của tài khoản cần xác nhận</param>
        /// <returns>200 OK nếu xác nhận thành công.</returns>
        /// <response code="200">Xác nhận thành công.</response>
        /// <response code="400">Xác nhận thất bại.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpGet("EmailConfirmation/{email}")]
        public async Task<IActionResult> ConfirmEmail([FromRoute] string email)
        {
            var response = await _auth.ConfirmEmail(email);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Redirect("https://nanafoodweb20241114171424.azurewebsites.net/Auth/Login?message=activation-success");
        }


        /// <summary>
        /// Kiểm tra xác nhận email
        /// </summary>
        /// <remarks>
        /// API này kiểm tra trạng thái xác nhận email của tài khoản.
        /// </remarks>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>200 OK nếu email đã xác nhận.</returns>
        /// <response code="200">Email đã xác nhận.</response>
        /// <response code="400">Kiểm tra thất bại.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpGet("CheckEmailConfirm/{userId}")]
        public async Task<IActionResult> CheckEmailConfirm(string userId)
        {
            /*var user = await _userManager.FindByEmailAsync(email);*/
            var user = await _userManager.FindByIdAsync(userId);
            var response = await _auth.CheckEmailConfirm(user);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }



        /// <summary>
        /// Đăng nhập bằng GitHub
        /// </summary>
        /// <remarks>
        /// Chuyển hướng người dùng đến GitHub để đăng nhập.
        /// </remarks>
        /// <returns>200 OK nếu chuyển hướng thành công.</returns>
        /// <response code="200">Điều hướng thành công.</response>
        /// <response code="400">Sai URL hoặc cấu hình credential không đúng.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpGet("github")]
        public async Task<IActionResult> GitHubLogin()
        {
            /*var properties = new AuthenticationProperties() { RedirectUri = Url.Action("GitHubExternalCallBack") };
            return Challenge(properties, "github");*/
            var redirectUri = "https://nanafoodweb20241114171424.azurewebsites.net/Auth/ExternalLoginCallBack";
            return Challenge(new AuthenticationProperties()
            {
                RedirectUri = redirectUri,
            }
            , authenticationSchemes: "github");
        }


        /// <summary>
        /// Đăng nhập bằng Google
        /// </summary>
        /// <remarks>
        /// Chuyển hướng người dùng đến Google để đăng nhập.
        /// </remarks>
        /// <returns>200 OK nếu chuyển hướng thành công.</returns>
        /// <response code="200">Điều hướng thành công.</response>
        /// <response code="400">Sai URL hoặc cấu hình credential không đúng.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpGet("google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var redirectUri = "https://nanafoodweb20241114171424.azurewebsites.net/Auth/ExternalLoginCallBack";
            return Challenge(new AuthenticationProperties()
            {
                RedirectUri = redirectUri,
            }
            , authenticationSchemes: GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Đăng nhập bằng Facebook
        /// </summary>
        /// <remarks>
        /// Chuyển hướng người dùng đến Facebook để đăng nhập.
        /// </remarks>
        /// <returns>200 OK nếu chuyển hướng thành công.</returns>
        /// <response code="200">Điều hướng thành công.</response>
        /// <response code="400">Sai URL hoặc cấu hình credential không đúng.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpGet("facebook")]
        public async Task<IActionResult> FacebookLogin()
        {
            var redirectUri = "https://nanafoodweb20241114171424.azurewebsites.net/Auth/ExternalLoginCallBack";
            return Challenge(new AuthenticationProperties()
            {
                RedirectUri = redirectUri,
            }
            , authenticationSchemes: FacebookDefaults.AuthenticationScheme);
        }


        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <remarks>
        /// API này gửi mật khẩu mới về email của người dùng.
        /// </remarks>
        /// <param name="email">Email của tài khoản cần lấy lại mật khẩu</param>
        /// <returns>200 OK nếu mật khẩu mới đã gửi.</returns>
        /// <response code="200">Gửi mật khẩu mới vào email.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc không gửi được email.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpPost("ForgotPassword/{email}")]
        public async Task<IActionResult> ForgotPassword([FromRoute] string email)
        {
            var response = await _auth.ForgotPassword(email);
            if(response != null && response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <remarks>
        /// API này trả về thông tin của người dùng hiện tại.
        /// </remarks>
        /// <returns>200 OK nếu lấy thông tin thành công.</returns>
        /// <response code="200">Lấy thông tin thành công.</response>
        /// <response code="400">Lấy thông tin thất bại.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [Authorize]
        [HttpGet("GetInformation")]
        public async Task<IActionResult> GetInfo()
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user != null)
            {
                return Ok(new ResponseDto()
                {
                    IsSuccess = true,
                    Result = _mapper.Map<UserDto>(user),
                    Message = "Lấy thông tin người dùng thành công"
                });
            }
            return Unauthorized();
                }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật thông tin của người dùng hiện tại.
        ///
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "FullName": "Nguyễn Văn An",
        ///     "Address": "123 Đường ABC, Quận 1, TP. Hồ Chí Minh",
        ///     "Email": "abcxyc@gmail.com",
        ///     "AvatarUrl": "https://example.com/avatar.jpg",
        ///     "UserName": "testaccount",
        ///     "PhoneNumber": "0987654321",
        ///     "Status": "Active"
        /// }
        /// ```
        /// </remarks>
        /// <param name="userdto">Thông tin cần cập nhật của người dùng.</param>
        /// <returns>
        /// - 200 OK nếu cập nhật thành công.
        /// - 400 BadRequest nếu yêu cầu không hợp lệ.
        /// - 500 Internal Server Error nếu xảy ra lỗi từ phía server.
        /// </returns>
        /// <response code="200">Cập nhật thông tin thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ, ví dụ: thiếu thông tin bắt buộc hoặc định dạng email sai.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserDto userdto)
        {
            var response = await _auth.UpdateUser(userdto);
            if (response != null && response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetUserStatus/{userId}")]
        public async Task<IActionResult> GetUserStatus(string userId)
        {
            var response = await _auth.GetUserStatus(userId);

            return Ok(response);
        }
    }
}
