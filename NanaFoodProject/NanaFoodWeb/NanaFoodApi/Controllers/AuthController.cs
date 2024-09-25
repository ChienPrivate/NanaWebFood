using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.IRepository;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenRepo _auth;

        public AuthController(IAuthenRepo auth)
        {
            _auth = auth;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="login">Thông tin đăng nhập</param>
        /// <returns>Đăng nhập thành công</returns>
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
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng tạo tài khoản mới trên hệ thống.
        /// 
        /// Ví dụ
        /// ```json
        /// {
        ///   "username": "testuser",
        ///   "password": "password123",
        ///   "email": "testuser@example.com"
        /// }
        /// ```
        /// 
        /// </remarks>
        /// <param name="regis">Thông tin tài khoản đăng ký.</param>
        /// <response code="200">Đăng ký thành công.</response>
        /// <response code="400">Thông tin đăng ký không hợp lệ.</response>
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
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
    }
}
