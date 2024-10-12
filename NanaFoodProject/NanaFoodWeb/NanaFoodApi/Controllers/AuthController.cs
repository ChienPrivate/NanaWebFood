using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.IRepository;
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

        public AuthController(IAuthenRepo auth, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auth = auth;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng đăng nhập bằng cách cung cấp tên đăng nhập và mật khẩu.
        /// Nếu thông tin đăng nhập chính xác, trả về JWT token cho phép xác thực các API khác.
        /// </remarks>
        /// <param name="login">Thông tin đăng nhập.</param>
        /// <returns>
        /// - Nếu thành công, trả về mã trạng thái 200 OK và JWT token.
        /// - Nếu thất bại, trả về mã lỗi 400 BadRequest với mô tả lỗi cụ thể.
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
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Đăng ký người dùng mới.
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng mới đăng ký tài khoản bằng cách cung cấp tên đăng nhập và mật khẩu.
        /// </remarks>
        /// <param name="model">Thông tin đăng ký.</param>
        /// <returns>
        /// Trả về đối tượng <see cref="ResponseDto"/> với thông tin về trạng thái thực hiện yêu cầu.
        /// - Nếu thành công, trả về mã trạng thái 201 Created và thông tin người dùng.
        /// - Nếu thất bại, trả về mã lỗi 400 BadRequest với mô tả lỗi.
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
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns>Đăng xuất thành công</returns>
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
        /// Đổi mật khẩu của người dùng đã đăng nhập.
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng thay đổi mật khẩu hiện tại của họ. Người dùng cần cung cấp mật khẩu cũ và mật khẩu mới.
        /// </remarks>
        /// <param name="changepass">Thông tin mật khẩu cần thay đổi</param>
        /// <returns>
        /// - Nếu thành công, trả về mã trạng thái 200 OK và thông báo "Mật khẩu đã được cập nhật"
        /// - Nếu thất bại, trả về mã lỗi 400 BadRequest với mô tả lỗi cụ thể thể.
        /// </returns>
        /// <response code="200">Đổi mật khẩu thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc mật khẩu cũ không chính xác.</response>
        /// <response code="401">Người dùng chưa xác thực hoặc token không hợp lệ.</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePass(ChangePassDto changepass)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.GetUserAsync(User);
            var response = await _auth.ChangePassword(user, changepass);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        
        [HttpGet("EmailConfirmation/{email}")]
        public async Task<IActionResult> ConfirmEmail([FromRoute] string email)
        {
            var response = await _auth.ConfirmEmail(email);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Redirect("https://localhost:51326/Auth/Login?message=activation-success");
        }

        [HttpGet("CheckEmailConfirm/{email}")]
        public async Task<IActionResult> CheckEmailConfirm(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var response = await _auth.CheckEmailConfirm(user);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }



        /// <summary>
        /// Đổi mật khẩu của người dùng đã đăng nhập.
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng thay đổi mật khẩu hiện tại của họ. Người dùng cần cung cấp mật khẩu cũ và mật khẩu mới.
        /// </remarks>
        /// <param></param>
        /// <returns>
        /// - Nếu thành công, trả về mã trạng thái 200 OK và thông báo "Mật khẩu đã được cập nhật"
        /// - Nếu thất bại, trả về mã lỗi 400 BadRequest với mô tả lỗi cụ thể thể.
        /// </returns>
        /// <response code="200">Điều hướng đến trang của github</response>
        /// <response code="400">Sai url hoặc cấu hình credential bị sai</response>
        /// <response code="401">Không được ủy quyền bởi tài khoản GitHub</response>
        /// <response code="500">Có lỗi xảy ra từ phía server.</response>
        [HttpGet("github")]
        public async Task<IActionResult> GitHubLogin()
        {
            /*var properties = new AuthenticationProperties() { RedirectUri = Url.Action("GitHubExternalCallBack") };
            return Challenge(properties, "github");*/

            return Challenge(new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GitHubExternalCallBack"),
            }
            , authenticationSchemes: ["github"]);
        }

        /// <summary>
        /// Người dùng lấy lại mật khẩu
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng yêu cầu cung cấp 1 mật khẩu mới tới email
        /// </remarks>
        /// <param name="email">Email của tài khoản cần lấy lại mật khẩu</param>
        /// <returns>
        /// - Nếu thành công, trả về mã trạng thái 200 OK và thông báo "Mật khẩu mới đã được gửi về email của bạn"
        /// - Nếu thất bại, trả về mã lỗi 400 BadRequest với mô tả lỗi cụ thể.
        /// </returns>
        /// <response code="200">Gửi mật khẩu mới vào email</response>
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



        //[HttpGet("SeachNameUser/{fullname}")]
        //public async Task<IActionResult> SeachNameUser([FromRoute] string fullname, int page = 1, int pageSize = 10)
        //{
        //    var response = await _auth.SearchName(fullname, page, pageSize);
        //    if (!response.IsSuccess)
        //    {
        //        return NotFound(response.Message);
        //    }
        //    return Ok(response);
        //}
        //[HttpGet("SeachEmail/{email}")]
        //public async Task<IActionResult> SeachEmail([FromRoute] string email, int page = 1, int pageSize = 10)
        //{
        //    var response = await _auth.SearchMail(email, page, pageSize);
        //    if (!response.IsSuccess)
        //    {
        //        return NotFound(response.Message);
        //    }
        //    return Ok(response);
        //}
        //[HttpDelete("DeleteUser/{email}")]
        //public async Task<IActionResult> DeleteUser(string email)
        //{
        //    var response = await _auth.DeleteUser(email);
        //    if (!response.IsSuccess)
        //    {
        //        return BadRequest(response.Message);
        //    }
        //    return Ok(response);
        //}
        //[HttpGet("GetAllUser")]
        //public async Task<IActionResult> GetAllUser(int page = 1, int pageSize = 10)
        //{
        //    var reponse = await _auth.GetAllUser(page, pageSize);
        //    if (!reponse.IsSuccess)
        //    {
        //        return BadRequest(reponse.Message);
        //    }
        //    return Ok(reponse);
        //}
    }
}
