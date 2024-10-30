using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NanaFoodWeb.CallAPICenter;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Route("Products")]
    public class ProductsController(IProductRepo productRepo, IHelperRepository helperRepository, ITokenProvider tokenProvider,ICategoryRepository categoryRepository) : Controller
    {
        private CallApiCenter _callApiCenter = new CallApiCenter();

        private readonly IHelperRepository _helperRepository = helperRepository;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        readonly IProductRepo _productRepo = productRepo;
        readonly ICategoryRepository _categoryRepository = categoryRepository;
        public async Task <IActionResult> Index(string searchQuery, int? page = 1, int pageSize = 10 )
        {
            var response = _productRepo.GetAll(page ?? 1, pageSize, true);
            if (response.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());

                ViewBag.lazyLoadData = resultData.Products;

                return View();
            }

            return View(new List<ProductDto>());
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(int page , int pageSize =100)
        {
            var token = _tokenProvider.GetToken();

            if (token != null)
            {
                var response = await _categoryRepository.GetAllCategoriesAsync(page, pageSize, true);

                if (response != null && response.IsSuccess)
                {
                    try
                    {
                        var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(response.Result.ToString());

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
            }

            var product = new ProductDto();
            return View(product);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ProductDto productDto, IFormFile? UploadFile)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }
            string imageUrl = string.Empty;
            if (UploadFile != null && UploadFile.Length > 0)
            {
                // Gọi service để upload hình ảnh
                var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);

                imageUrl = uploadResponse.Result?.ToString() ?? "Tải ảnh lên thất bại";

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Gắn URL của hình ảnh vào model
                    productDto.ImageUrl = imageUrl;
                }
                else
                {
                    productDto.ImageUrl = "Chưa có hình ảnh";
                }

            }
            // Chuyển đổi từ ProductDto sang Product
            var product = new Product()
            {
                ProductName = productDto.ProductName,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                ImageUrl = imageUrl,
                Description = productDto.Description,
                IsActive = productDto.IsActive,
            };
            // Gọi đến repository để tạo sản phẩm
            var response = _productRepo.Create(product);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response?.Message ?? "Có lỗi xảy ra khi tạo sản phẩm");
            return View(productDto);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, int page, int pageSize)
        {

            var token = _tokenProvider.GetToken();

            if (token != null)
            {
                var response = await _categoryRepository.GetAllCategoriesAsync(page, pageSize, true);

                if (response != null && response.IsSuccess)
                {
                    try
                    {
                        var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(response.Result.ToString());

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

                var result = _productRepo.GetById(id);

                if (result != null && result.IsSuccess) // Giả sử ResponseDto có thuộc tính IsSuccess
                {
                    var productDto = JsonConvert.DeserializeObject<ProductDto>(result.Result.ToString()); // Giả sử ProductDto nằm trong Result
                    if (productDto != null)
                    {
                        return View(productDto); // Trả về View với đối tượng ProductDto
                    }
                }
                else
                {
                    return RedirectToAction("Index", new { error = "Không tìm thấy sản phẩm để chỉnh sửa." });
                }
            }

            // Trường hợp token là null, trả về một trang lỗi hoặc điều hướng về một action khác
            return RedirectToAction("Index", new { error = "Token không hợp lệ." });
        }
        [HttpPost("Edit/{id}")]

        public async Task<IActionResult> Edit(ProductDto productDto, int id, IFormFile? UploadFile)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }
            string imageUrl = productDto.ImageUrl; // Giữ URL ảnh cũ

            if (UploadFile != null && UploadFile.Length > 0)
            {
                // Upload ảnh mới
                var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);
                imageUrl = uploadResponse.Result?.ToString() ?? imageUrl; // Nếu upload thất bại, giữ ảnh cũ
            }

            // Cập nhật lại URL ảnh trong DTO
            productDto.ImageUrl = imageUrl;

            var response = _productRepo.Update(productDto);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", response?.Message ?? "Có lỗi xảy ra khi cập nhật sản phẩm");
            return View(productDto);
        }
        [HttpPost("Delete")]
        public IActionResult Delete(int id, string searchQuery, int? page)
        {
            var response = _productRepo.Delete(id);
            if (response == null)
            {
                TempData["ErrorMessage"] = "Xoá thất bại vui lòng kiểm tra lại thông tin.";
                return RedirectToAction("Index");
            }

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Xoá thành công.";

            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete the product.";
            }
            return RedirectToAction("Index", new { searchQuery = searchQuery, page = page });
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var response = _productRepo.GetById(id);

            if (response?.IsSuccess == true && response.Result != null)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(productDto);
            }

            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            return RedirectToAction("Index");
        }


        [HttpGet("CDetails/{id}")]
        public IActionResult CDetails(int id, int currentPage = 1)
        {
            var response = _productRepo.GetById(id);

            if (response?.IsSuccess == true && response.Result != null)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                ViewData["CurrentPage"] = currentPage;
                return View(productDto);
            }

            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            return RedirectToAction("Index");
        }

        [HttpPost("deactive/{id}")]
        public IActionResult Deactivate(int id, string searchQuery, int? page)
        {
            var response = _productRepo.UnActiveCategory(id);
            if (response == null)
            {
                TempData["ErrorMessage"] = "Xoá thất bại vui lòng kiểm tra lại thông tin.";
                return RedirectToAction("Index");
            }
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Xoá thành công.";

            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete the product.";
            }
            return RedirectToAction("Index", new { searchQuery = searchQuery, page = page });
        }

        [HttpGet]
        [Route("SearchByName")]
        public async Task <IActionResult> SearchByName(string? query, int? page= 1 , int pageSize = 9)
        {
            ViewData["Action"] = "SearchByName";
            var response =  _productRepo.GetBySearch(query, 1 , pageSize);
            if (response?.IsSuccess == true && response.Result != null)
            {
                var productList = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());

                if (productList != null)
                {
                    int totalItems = productList.TotalCount;
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                    ViewData["totalPages"] = totalPages;
                    ViewData["currentPage"] = page;

                    return View("~/Views/Home/Menu.cshtml", productList);
                }
            }

            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm nào.";
            return View("Menu", new ProductVM());

        }


    }
}
