using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NanaFoodWeb.Controllers
{
    
    public class NanaController(ITokenProvider tokenProvider, IProductRepo productRepo, IUserRepository userRepo) : Controller
    {
        readonly ITokenProvider _tokenProvider = tokenProvider;
        readonly IProductRepo _productRepo = productRepo;
        readonly IUserRepository _userRepo = userRepo;
        public async Task<IActionResult> Index()
        {
            var token = _tokenProvider.GetToken();
            var role = _tokenProvider.ReadToken("role", token);
            if (token == null)
            {
                return Unauthorized();
            }
            if (role == "customer")
            {
                return Forbid();
            }
            var apiProduct = await _productRepo.GetProduct();
            if (apiProduct.IsSuccess)
            {
                var Product = JsonConvert.DeserializeObject<List<ProductDto>>(apiProduct.Result.ToString());
                ViewBag.ProductCount = Product.Count.ToString();
            }
            var apiUser = await _userRepo.GetAllUserAsync();
            if (apiUser.IsSuccess)
            {
                var User = JsonConvert.DeserializeObject<List<UserDto>>(apiUser.Result.ToString());
                ViewBag.UserCount = User.Count.ToString();
            }
            return View();
        }
    }
}
