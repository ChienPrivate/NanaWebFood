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
    //[Authorize(Roles = "admin")]
    public class CartController : Controller
    {
        private readonly CallApiCenter _callAPICenter;
        private readonly ConvertHelper _convertHelper;
        private readonly ITokenProvider _tokenProvider;
        //private readonly ICartRepo _cartRepo;

        public CartController( ITokenProvider tokenProvider)
        {
            _callAPICenter = new CallApiCenter();
            _convertHelper = new ConvertHelper();
            _tokenProvider = tokenProvider;
           // _cartRepo = cartRepo;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = _tokenProvider.GetToken();
                var response = await _callAPICenter.GetMethod<ResponseDto>("Cart/GetCart", token);
                if (response == null || !response.IsSuccess)
                {
                    ViewBag.Message = "Không có dữ liệu";
                    return View(new List<CartDetailsDto>()); // Return empty view with message
                }
                if (response.IsSuccess)
                {
                    if(response.Result == null)
                    {
                        return View(new List<CartDetailsDto>());
                    }
                    else
                    {
                        var resultData = JsonConvert.DeserializeObject<Result<List<CartDetailsDto>>>(response.Result.ToString());
                        ViewBag.TotalPages = resultData.TotalPages;
                        return View(resultData.Data);// Pass the cart details to the view
                    }
                    
                    
                                                 //return View(result.Result); 
                }
                ViewBag.Message = response.Message;
                return View(); // Return empty view with message
            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View(new List<CartDetailsDto>()); // Return empty view with message
            }
            
        }

        [HttpGet("AddToCart/{id}")]
        public async Task<IActionResult> AddToCart(int id)
        {
            var token = _tokenProvider.GetToken();
            CartDetailsDto cartDetailsDto = new CartDetailsDto();
            cartDetailsDto.Quantity = 1;
            cartDetailsDto.UserId = "";
            cartDetailsDto.Image = "";
            cartDetailsDto.ProductName = "";
            cartDetailsDto.ProductId = id;
            var response = await _callAPICenter.PostMethod<ResponseDto>(cartDetailsDto, "Cart/AddToCart", token);
            if (response == null || !response.IsSuccess)
            {
                ViewBag.Message = "Lỗi khi thêm mới dữ liệu";
            }
            else
            {
                ViewBag.Message = response.Message;
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
