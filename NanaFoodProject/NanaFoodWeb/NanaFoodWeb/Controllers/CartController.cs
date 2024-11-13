using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.CallAPICenter;
using NanaFoodWeb.Convert;
using NanaFoodWeb.Extensions;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NanaFoodWeb.Controllers
{
    [Route("Carts")]
    [Authorize(Roles = "customer")]
    public class CartController : Controller
    {
        private readonly ICartRepo _cartRepo;
        private readonly CallApiCenter _callAPICenter;
        private readonly ConvertHelper _convertHelper;
        private readonly ITokenProvider _tokenProvider;

        public CartController( ITokenProvider tokenProvider, ICartRepo cartRepo)
        {
            _callAPICenter = new CallApiCenter();
            _convertHelper = new ConvertHelper();
            _tokenProvider = tokenProvider;
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> Index()
        {
            var token = _tokenProvider.GetToken();
            if (token == null)
            {
                TempData["error"] = "Vui lòng đăng nhập trước";
                return Unauthorized();
            } else
            {
                var response = await _cartRepo.GetCart();
                if (response.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(response.Result.ToString());
                    _tokenProvider.SetCartCount(Data.Count.ToString());
                    return View(Data);
                }
                _tokenProvider.SetCartCount("0");
                return View(new List<CartResponseDto>());
            }           
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            var cartDetail = new CartDetailsDto()
            {
                ProductId = id,
                Quantity = quantity
            };
            var response = await _cartRepo.AddToCart(cartDetail);
            string message = response.Message?.ToString() ?? "Có lỗi xảy ra";

            if (response.IsSuccess)
            {
                var responseCart = await _cartRepo.GetCart();
                if (responseCart.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(responseCart.Result.ToString());
                    _tokenProvider.SetCartCount(Data.Count.ToString());
                }
                TempData["success"] = message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Cart/ModifyCartItemQuantity")]
        public async Task<IActionResult> ModifyCartItemQuantity(int productId, string message)
        {
            var response = await _cartRepo.ModifyCartQuantity(productId, message);

            if (response.IsSuccess)
            {
                // Chuyển đổi response.Result thành đối tượng JSON để lấy quantity và total
                var resultData = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString());
                int quantity = resultData.quantity;
                int total = resultData.total;

                return Json(new
                {
                    success = true,
                    quantity = quantity,
                    total = total
                });
            }

            return Json(new { success = false, message = "không tìm thấy sản phẩm" });
        }

        [HttpPost]
        [Route("Cart/RemoveCartItem")]
        public async Task<IActionResult> RemoveCartItem(int productId)
        {
            var response = await _cartRepo.DeleteCartItem(productId);

            if (response.IsSuccess)
            {
                var cartCountResponse = await _cartRepo.GetCart();
                int cartcount = 0;
                if (cartCountResponse.Result == null && cartCountResponse.IsSuccess)
                {
                    cartcount = 0;
                    _tokenProvider.SetCartCount(cartcount.ToString());
                }
                else
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(cartCountResponse.Result.ToString());
                    cartcount = Data.Count();
                    _tokenProvider.SetCartCount(Data.Count.ToString());
                }

                HttpContext.Session.SetString("CartCount", cartcount.ToString());
                return Json(new { success = true, cartCount = cartcount });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }
    }
}
