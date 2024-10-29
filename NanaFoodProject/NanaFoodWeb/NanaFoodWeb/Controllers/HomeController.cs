using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NanaFoodDAL.Model;
using NanaFoodWeb.Extensions;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;
using Sprache;
using System.Diagnostics;

namespace NanaFoodWeb.Controllers
{
    [ExcludeAdmin]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenProvider _tokenProvider;
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepository _categoryRepository;
        public HomeController(ILogger<HomeController> logger, ITokenProvider tokenProvider, IProductRepo productRepo, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _tokenProvider = tokenProvider;
            _productRepo = productRepo;
            _categoryRepository = categoryRepository;
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

        public IActionResult Sort(string sort, int page, int pageSize = 9)
        {
            if(page == 0)
            {
                page = 1;
            }
            TempData["sort"] = sort;
            var response = _productRepo.Sorting(sort, page, 9);
            if (response.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
                int totalItems = result.TotalCount;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewData["totalPages"] = totalPages;
                ViewData["currentPage"] = page;

                if (result == null)
                {
                    result = new ProductVM();
                }

                return View(result);
            }
            return NotFound();
        }

        public async Task<IActionResult> FilterLoai(int id, int page , int pageSize = 9)
        {
            var response = await _productRepo.GetByCategoryId(id, 1, 9);
            if (response.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
                int totalItems = result.TotalCount;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewData["totalPages"] = totalPages;
                ViewData["currentPage"] = page;

                if (result == null)
                {
                    result = new ProductVM();
                }

                return View(result);
            }
            return NotFound();
        }

        public async Task <IActionResult> Menu(string searchQuery, int? page = 1, int pageSize = 9)
        {
            ViewData["Action"] = "Menu";
            var result = await _categoryRepository.GetAllCategoriesAsync(1, pageSize, true); 
                if(result != null && result.IsSuccess)
                {
                    try
                    {
                        var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(result.Result.ToString());

                        if (categories != null)
                        {
                            var listItem = new List<SelectListItem>();
                            foreach (var category in categories)
                            {
                                listItem.Add(new SelectListItem
                                {
                                    Text = category.CategoryName,
                                    Value = category.CategoryId.ToString()
                                });
                            }
                            ViewBag.ListCategory = listItem;
                        }
                    }
                    catch (JsonSerializationException ex)
                    {
                        ModelState.AddModelError("", $"Error deserializing JSON: {ex.Message}");
                    }
                }
            // Lấy danh sách sản phẩm
            ViewData["page"] = page;
            ViewData["searchQuery"] = searchQuery;
            var response = _productRepo.GetAll(page ?? 1, pageSize, false);
           
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
        public async Task<IActionResult>FilterCategory(int categoryid, int page, int pageSize)
        {
            ViewData["Action"] = "FilterCategory";

            var result = await _categoryRepository.GetAllCategoriesAsync(1, pageSize, true);
            if (result != null && result.IsSuccess)
            {
                try
                {
                    var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(result.Result.ToString());

                    if (categories != null)
                    {
                        var listItem = new List<SelectListItem>();
                        foreach (var category in categories)
                        {
                            listItem.Add(new SelectListItem
                            {
                                Text = category.CategoryName,
                                Value = category.CategoryId.ToString()
                            });
                        }
                        ViewBag.ListCategory = listItem;
                    }
                }
                catch (JsonSerializationException ex)
                {
                    ModelState.AddModelError("", $"Error deserializing JSON: {ex.Message}");
                }
            }
            var reponse = await _productRepo.GetByCategoryId(categoryid, page, pageSize);
            if (reponse.IsSuccess)
            {
                var productList = JsonConvert.DeserializeObject<ProductVM>(reponse.Result.ToString());

                // Calculate total pages and set up pagination data
                int totalItems = productList.TotalCount;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewData["totalPages"] = totalPages;
                ViewData["currentPage"] = page;
                if (productList == null)
                {
                    productList = new ProductVM();
                }

                return  View("Menu", productList);
            }
            else
            {
                ModelState.AddModelError("", "Error retrieving products from the API.");
                return View(new ProductVM());
            }
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
