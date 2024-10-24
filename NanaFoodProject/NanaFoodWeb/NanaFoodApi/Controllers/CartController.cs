using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;


namespace NanaFoodApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(SignInManager<User> sm, ICartRepo cartrepo) : ControllerBase
    {
        SignInManager<User> _SignInManager = sm;
        ICartRepo _cartrepo = cartrepo;


        // POST api/Cart/AddtoCart
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

        // GET api/Cart/GetCart
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

        // DELETE api/Cart/deletecart/3
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

        // PUT api/Cart/UpdateCart?3&increase
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
