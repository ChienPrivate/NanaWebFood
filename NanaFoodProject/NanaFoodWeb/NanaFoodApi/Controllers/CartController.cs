using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;


namespace NanaFoodApi.Controllers
{
    [Authorize(Roles = "customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(SignInManager<User> sm, ICartRepo cartrepo) : ControllerBase
    {
        SignInManager<User> _SignInManager = sm;
        ICartRepo _cartrepo = cartrepo;


        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng thêm một sản phẩm vào giỏ hàng.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "userId": "user-id-example"
        ///     "productId": 1,
        ///     "quantity": 2,
        /// }
        /// ```
        /// </remarks>
        /// <param name="cartDetails">Thông tin chi tiết của sản phẩm cần thêm vào giỏ hàng.</param>
        /// <returns>
        /// - 200 OK nếu thêm sản phẩm vào giỏ hàng thành công.
        /// - 400 BadRequest nếu có lỗi xảy ra.
        /// </returns>
        /// <response code="200">Sản phẩm được thêm vào giỏ hàng thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi thêm sản phẩm.</response>

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody]CartDetailsDto cartDetails)
        {
            var userid = _SignInManager.UserManager.GetUserId(User);
            cartDetails.UserId = userid;
            var response = await _cartrepo.AddToCart(cartDetails);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Lấy thông tin giỏ hàng của người dùng
        /// </summary>
        /// <remarks>
        /// API này trả về thông tin giỏ hàng của người dùng hiện tại.
        /// </remarks>
        /// <returns>
        /// - 200 OK nếu lấy thông tin giỏ hàng thành công.
        /// - 400 BadRequest nếu có lỗi xảy ra.
        /// </returns>
        /// <response code="200">Trả về thông tin giỏ hàng của người dùng.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi lấy thông tin giỏ hàng.</response>
        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            var user = await _SignInManager.UserManager.GetUserAsync(User);
            var response = await _cartrepo.GetCart(user);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng xóa một sản phẩm khỏi giỏ hàng bằng cách cung cấp ID của sản phẩm.
        /// </remarks>
        /// <param name="productId">ID của sản phẩm cần xóa.</param>
        /// <returns>
        /// - 200 OK nếu xóa sản phẩm thành công.
        /// - 400 BadRequest nếu có lỗi xảy ra.
        /// </returns>
        /// <response code="200">Sản phẩm đã được xóa khỏi giỏ hàng thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi xóa sản phẩm.</response>
        [HttpDelete("deletecart/{productId:int}")]
        public async Task<IActionResult> DeleteProductFromCart([FromRoute] int productId)
        {
            var userid = _SignInManager.UserManager.GetUserId(User);
            var response = await _cartrepo.DeleteCart(productId, userid);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong giỏ hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép người dùng cập nhật số lượng của một sản phẩm trong giỏ hàng. Người dùng có thể chỉ định tăng hoặc giảm số lượng.
        /// </remarks>
        /// <param name="productId">ID của sản phẩm cần cập nhật.</param>
        /// <param name="message">Thông báo hành động, có thể là "increase" hoặc "decrease" để tăng hoặc giảm số lượng.</param>
        /// <returns>
        /// - 200 OK nếu cập nhật số lượng sản phẩm thành công.
        /// - 400 BadRequest nếu có lỗi xảy ra.
        /// </returns>
        /// <response code="200">Cập nhật số lượng sản phẩm thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi cập nhật số lượng sản phẩm.</response>
        [HttpPut("UpdateCart/{productId:int}&{message}")]
        public async Task<IActionResult> UpdateCart([FromRoute] int productId, string message)
        {
            var userid = _SignInManager.UserManager.GetUserId(User);
            var response = await _cartrepo.UpdateCart(productId, userid, message);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
