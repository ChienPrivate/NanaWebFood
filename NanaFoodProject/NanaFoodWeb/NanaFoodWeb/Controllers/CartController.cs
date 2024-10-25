using Azure;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.CallAPICenter;
using NanaFoodWeb.Convert;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Route("Carts")]
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
                return RedirectToAction("Login", "Auth");
            } else
            {
                var response = await _cartRepo.GetCart();
                if (response.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(response.Result.ToString());
                    return View(Data);
                }
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
                TempData["success"] = message;
            }
            return RedirectToAction("Index");
        }

        // GET: Cart/DeleteCart (Remove Item from Cart)
        [HttpGet("DeleteCart/{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var token = _tokenProvider.GetToken();
            var response = await _callAPICenter.DeleteMethod<ResponseDto>("Cart/deletecart/"+ id, token);
            if (response == null || !response.IsSuccess)
            {
                ViewBag.Message = "Lỗi khi xoá dữ liệu"; 
            }
            else
            {
                ViewBag.Message = response.Message;
            }
            return RedirectToAction("Index");
            
        }
    }
}
