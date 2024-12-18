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
using Sprache;

namespace NanaFoodWeb.Controllers
{
    [Route("Products")]
    public class ProductsController(IProductRepo productRepo, IHelperRepository helperRepository, ITokenProvider tokenProvider,ICategoryRepository categoryRepository, IReviewRepository reviewRepository) : Controller
    {
        private CallApiCenter _callApiCenter = new CallApiCenter();
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IHelperRepository _helperRepository = helperRepository;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        readonly IProductRepo _productRepo = productRepo;
        readonly ICategoryRepository _categoryRepository = categoryRepository;
        public async Task<IActionResult> Index()
        {
            var response = await _productRepo.GetProduct();
            if (response.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<List<Product>>(response.Result.ToString());
            

                ViewBag.lazyLoadData = resultData;
                return View();
            }
            // Nếu không thành công, trả về View với danh sách rỗng
            return View(new List<ProductVM>());
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
                Quantity = productDto.Quantity,
                IsActive = productDto.IsActive,
            };
            // Gọi đến repository để tạo sản phẩm
            var response = _productRepo.Create(product);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Tạo sản phẩm thành công";
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

                if (result != null && result.IsSuccess) 
                {
                    var productDto = JsonConvert.DeserializeObject<ProductDto>(result.Result.ToString());
                    if (productDto != null)
                    {
                        return View(productDto);
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
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", response?.Message ?? "Có lỗi xảy ra khi cập nhật sản phẩm");
            return View(productDto);
        }
        [HttpGet("Delete/{id}")]
        public async Task <IActionResult> Delete(int id)
        {
            var response = _productRepo.GetById(id);

            if (response?.IsSuccess == true && response.Result != null)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(productDto);
            }

            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            TempData["error"] = "Không tìm thấy sản phẩm.";
            return RedirectToAction("Index");
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

        [HttpPost("DeleteConfirm")]
        public IActionResult DeleteConfirm(int id)
        {
            var response = _productRepo.Delete(id);
            if (response.IsSuccess)
            {
                TempData["success"] = "Xóa sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                string message = response.Message;
                return NotFound(message);
            }
        }

        [HttpGet("CDetails/{id}")]
        public async Task<IActionResult> CDetails(int id, int page = 1)
        {
            var response = _productRepo.GetById(id);
            var ratingResponse = await _reviewRepository.GetProductRating(id);
            var reviewListResponse = await _reviewRepository.GetProducReview(id, page, 5);
            var ListProductImage = await _productRepo.GetImages(id);
            var productImageVM = ListProductImage;
            var reviewVM = JsonConvert.DeserializeObject<ReviewVM>(reviewListResponse.Result.ToString());

            ViewBag.ListImage = productImageVM;

            ViewData["totalPages"] = reviewVM.TotalPages;
            ViewData["currentPage"] = page;
            ViewBag.ReviewList = reviewVM.Reviews;


            ViewData["rating"] = double.Parse(ratingResponse.Result.ToString());

            if ((double)ViewData["rating"] == 0)
            {
                ViewData["rating"] = 5.0;
            }

            if (response?.IsSuccess == true && response.Result != null)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
               
                return View(productDto);
            }

            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            return NotFound();
        }

        [HttpPost("deactive/{id}")]
        public IActionResult Deactivate(int id, string searchQuery, int? page)
        {
            var response = _productRepo.UnActiveCategory(id);
            if (response == null)
            {
                TempData["ErrorMessage"] = "Vô hiệu hóa thất bại vui lòng kiểm tra lại thông tin.";
                TempData["error"] = "Vô hiệu hóa không thành công";
                return RedirectToAction("Index");
            }
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Vô hiệu hóa thành công.";
                TempData["success"] = "Vô hiệu hóa thành công";

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
