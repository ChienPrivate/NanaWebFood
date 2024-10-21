using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.Extensions;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NanaFoodWeb.Controllers
{
    [ExcludeAdmin]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenProvider _tokenProvider;
        private readonly IProductRepo _productRepo;
        public HomeController(ILogger<HomeController> logger, ITokenProvider tokenProvider, IProductRepo productRepo)
        {
            _logger = logger;
            _tokenProvider = tokenProvider;
            _productRepo = productRepo;
        }

        [AllowAnonymous]
        public async Task <IActionResult> Index(string searchQuery, int? page = 1, int pageSize = 100)
        {
            var token = _tokenProvider.GetToken(); // Lấy token nếu có

            // Nếu có token và vai trò là admin, chuyển hướng đến Dashboard
            if (token != null)
            {
                var role = _tokenProvider.ReadToken("role", token);
                if (role == "admin")
                {
                    return RedirectToAction("Index", "DashBoard");
                }
            }

            // Lấy danh sách sản phẩm
            ViewData["page"] = page;
            ViewData["searchQuery"] = searchQuery;

            var response = _productRepo.GetAll(page ?? 1, pageSize, true);

            if (response.IsSuccess)
            {
                var productList = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
                int totalItems = productList.TotalCount;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);  
                ViewData["totalPages"] = totalPages;
                ViewData["currentPage"] = page;

                if (productList == null)
                {
                    productList = new ProductVM();
                }

                return View(productList); 
            }

            return View(); 
        }


        public IActionResult About() 
        {
            return View();
        }

        public IActionResult Discount()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Menu(string searchQuery, int? page = 1, int pageSize = 12)
        {
            // Lấy danh sách sản phẩm
            ViewData["page"] = page;
            ViewData["searchQuery"] = searchQuery;

            var response = _productRepo.GetAll(page ?? 1, pageSize, true);

            if (response.IsSuccess)
            {
                var productList = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
                int totalItems = productList.TotalCount;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewData["totalPages"] = totalPages;
                ViewData["currentPage"] = page;

                if (productList == null)
                {
                    productList = new ProductVM();
                }

                return View(productList);
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
