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
    public class ProductsController(IProductRepo productRepo, IHelperRepository helperRepository, ITokenProvider tokenProvider) : Controller
    {
        private CallApiCenter _callApiCenter = new CallApiCenter();

        private readonly IHelperRepository _helperRepository = helperRepository;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        readonly IProductRepo _productRepo = productRepo;
        public async Task <IActionResult> Index(string searchQuery, int? page = 1, int pageSize = 10)
        {
            ViewData["page"] = page;
            ViewData["searchQuery"] = searchQuery;

            ViewBag.CurrentFilter = searchQuery;
            var response = _productRepo.GetAll(page ?? 1, pageSize);
            if (response.IsSuccess == true)
            {
                var productList = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
                int totalItems = productList.TotalCount;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                // Truyền dữ liệu về tổng số trang và trang hiện tại cho View
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

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var token = _tokenProvider.GetToken();
            string apiName = "Category/SearchName?" + 1 + "&pageSize=100";
            ResponseDto respone = await _callApiCenter.GetMethod<ResponseDto>(apiName, token);
            if (respone.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<Result<List<CategoryDto>>>(respone.Result.ToString());
                var ListItem = new List<SelectListItem>();
                foreach (var e in resultData.Data)
                {
                    ListItem.Add(new SelectListItem { Text = e.CategoryName, Value = e.CategoryId.ToString() });
                }
                ViewBag.ListCategory = ListItem;
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
        public async Task<IActionResult> Edit(int id)
        {
            var token = _tokenProvider.GetToken();
            string apiName = "Category/SearchName?" + 1 + "&pageSize=100";
            ResponseDto respone = await _callApiCenter.GetMethod<ResponseDto>(apiName, token);
            if (respone.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<Result<List<CategoryDto>>>(respone.Result.ToString());
                var ListItem = new List<SelectListItem>();
                foreach (var e in resultData.Data)
                {
                    ListItem.Add(new SelectListItem { Text = e.CategoryName, Value = e.CategoryId.ToString() });
                }
                ViewBag.ListCategory = ListItem;
            }
            var response = _productRepo.GetById(id);

            if (response != null && response.IsSuccess)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(productDto);
            }
            return RedirectToAction("Index", new { error = "Không tìm thấy sản phẩm để chỉnh sửa." });
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
        [HttpPost]
        [Route("Delete")]
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

        [HttpGet("SearchByName")]
        public IActionResult SearchByName(string? query, int? page, int pageSize = 10)
        {
            var response = _productRepo.GetBySearch(query, 1, pageSize);
            if (response?.IsSuccess == true && response.Result != null)
            {
                var productDto = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
                return View(productDto);
            }

            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm nào.";
            return RedirectToAction("Index");
        }
    }
}
