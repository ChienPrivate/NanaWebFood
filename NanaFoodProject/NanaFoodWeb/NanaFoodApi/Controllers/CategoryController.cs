using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IProductRepository _productRepo;
        public CategoryController(ICategoryRepo repo, IProductRepository productRepo)
        {
            _categoryRepo = repo;
            _productRepo = productRepo;
        }

        private T GetResult<T>(ResponseDto response)
        {
            if (response.IsSuccess && response.Result is T typedResult)
            {
                return typedResult;
            }
            throw new InvalidCastException("Không thể chuyển đổi kết quả sang kiểu dữ liệu mong muốn.");
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục.
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy danh sách tất cả các danh mục. Có thể sử dụng phân trang hoặc lấy toàn bộ danh sách.
        /// </remarks>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng danh mục trên mỗi trang.</param>
        /// <param name="isSelectAll">Nếu là true, trả về tất cả các mục.</param>
        /// <returns>Danh sách các danh mục.</returns>
        /// <response code="200">Trả về danh sách danh mục.</response>
        /// <response code="400">Yêu cầu không hợp lệ.</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto>> GetAllCategories(int page = 1, int pageSize = 10, bool isSelectAll = true)
        {
            var response =  await _categoryRepo.GetAll(page, pageSize, isSelectAll);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh mục theo ID.
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy thông tin chi tiết của một danh mục dựa vào ID.
        /// </remarks>
        /// <param name="id">ID của danh mục.</param>
        /// <returns>Thông tin chi tiết của danh mục.</returns>
        /// <response code="200">Trả về thông tin danh mục.</response>
        /// <response code="404">Không tìm thấy danh mục với ID này.</response>
        [HttpGet("{id}")]
        public ActionResult<ResponseDto> GetCategoryById([FromRoute] int id)
        {
            var response = _categoryRepo.GetById(id);
            if (!response.IsSuccess)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Tìm kiếm danh mục theo tên.
        /// </summary>
        /// <remarks>
        /// API này cho phép tìm kiếm danh mục theo tên và hỗ trợ phân trang.
        /// </remarks>
        /// <param name="name">Tên danh mục cần tìm kiếm.</param>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng danh mục trên mỗi trang.</param>
        /// <returns>Danh sách các danh mục phù hợp.</returns>
        /// <response code="200">Trả về danh sách các danh mục phù hợp.</response>
        /// <response code="404">Không tìm thấy danh mục nào phù hợp.</response>
        [HttpGet("SearchName")]
        public async Task<ActionResult<ResponseDto>> GetCategoryByName( string name="", int page = 1, int pageSize = 10)
        {
            var response = await _categoryRepo.GetByName(name, page, pageSize);
            if (!response.IsSuccess)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Tạo danh mục mới.
        /// </summary>
        /// <remarks>
        /// API này cho phép tạo một danh mục mới.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "CategoryName": "Thực phẩm",
        ///     "Description": "Danh mục các sản phẩm thực phẩm",
        ///     "CategoryImage": "https://example.com/image.jpg",
        ///     "IsActive": true
        /// }
        /// ```
        /// </remarks>
        /// <param name="categoryDto">Thông tin danh mục mới.</param>
        /// <returns>Thông tin danh mục vừa tạo.</returns>
        /// <response code="201">Tạo danh mục thành công.</response>
        /// <response code="400">Thông tin danh mục không hợp lệ.</response>
        [HttpPost]
        public ActionResult<ResponseDto> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Đầu vào không hợp lệ.");
            }

            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                CategoryImage = categoryDto.CategoryImage,
                IsActive = categoryDto.IsActive
            };

            var response = _categoryRepo.Create(category);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, response);
        }


        /// <summary>
        /// Cập nhật thông tin danh mục.
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật thông tin của một danh mục dựa vào ID.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "CategoryId": 1,
        ///     "CategoryName": "Thực phẩm đã chỉnh sửa",
        ///     "Description": "Danh mục các sản phẩm thực phẩm đã cập nhật",
        ///     "CategoryImage": "https://example.com/new-image.jpg",
        ///     "IsActive": true
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">ID của danh mục cần cập nhật.</param>
        /// <param name="categoryDto">Thông tin danh mục cần cập nhật.</param>
        /// <returns>Thông tin danh mục sau khi cập nhật.</returns>
        /// <response code="200">Cập nhật danh mục thành công.</response>
        /// <response code="400">Thông tin không hợp lệ.</response>
        /// <response code="404">Danh mục không tồn tại.</response>
        [HttpPut("{id}")]
        public ActionResult<ResponseDto> UpdateCategory([FromRoute] int id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid || id != categoryDto.CategoryId)
            {
                return BadRequest("Đầu vào không hợp lệ.");
            }

            var category = new Category
            {

                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName,
                CategoryImage = categoryDto.CategoryImage,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive
            };

            var response = _categoryRepo.Update(category);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Xóa danh mục.
        /// </summary>
        /// <remarks>
        /// API này cho phép xóa một danh mục dựa vào ID.
        /// </remarks>
        /// <param name="id">ID của danh mục cần xóa.</param>
        /// <returns>Kết quả xóa danh mục.</returns>
        /// <response code="200">Xóa danh mục thành công.</response>
        /// <response code="404">Không tìm thấy danh mục với ID này.</response>
        [HttpDelete("{id}")]
        public ActionResult<ResponseDto> DeleteCategory([FromRoute] int id)
        {
            var response = _categoryRepo.Delete(id);
            if (!response.IsSuccess)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Kích hoạt danh mục.
        /// </summary>
        /// <remarks>
        /// API này cho phép kích hoạt một danh mục dựa vào ID.
        /// </remarks>
        /// <param name="id">ID của danh mục cần kích hoạt.</param>
        /// <returns>Kết quả kích hoạt danh mục.</returns>
        /// <response code="200">Kích hoạt danh mục thành công.</response>
        /// <response code="404">Danh mục không tồn tại.</response>
        [HttpPut("active/{id}")]
        public ActionResult<ResponseDto> ActiveCategory([FromRoute] int id)
        {
            var response = _categoryRepo.ModifyStatus(id, true);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        /// <summary>
        /// Hủy kích hoạt danh mục.
        /// </summary>
        /// <remarks>
        /// API này cho phép hủy kích hoạt một danh mục dựa vào ID.
        /// 
        /// **Sample Request**:
        /// PUT /api/category/Unactive/{id}
        /// </remarks>
        /// <param name="id">ID của danh mục cần hủy kích hoạt.</param>
        /// <returns>Kết quả hủy kích hoạt danh mục.</returns>
        /// <response code="200">Hủy kích hoạt danh mục thành công.</response>
        /// <response code="404">Danh mục không tồn tại</response>
        [HttpPut("Unactive/{id}")]
        public ActionResult<ResponseDto> UnActiveCategory([FromRoute] int id)
        {
            var response = _categoryRepo.ModifyStatus(id, false);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Lấy số lượng sản phẩm của mỗi danh mục.
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các danh mục cùng với số lượng sản phẩm của từng danh mục.
        /// </remarks>
        /// <returns>Danh sách các danh mục với số lượng sản phẩm.</returns>
        /// <response code="200">Trả về danh sách số lượng sản phẩm của mỗi danh mục.</response>
        [AllowAnonymous]
        [HttpGet("CategoryMenu")]
        public IActionResult GetFoods()
        {
            var foods = _productRepo.Products.Where(a => a.IsActive).GroupBy(x => x.CategoryId).Select(c => new
            {
                CategoryId = c.Key,
                Name = GetResult<CategoryDto>(_categoryRepo.GetById(c.Key)).CategoryName,
                Quantity = c.Count()
            });
            return Ok(new ResponseDto {
                IsSuccess = true,
                Message = "Lấy số lượng mỗi category thành công",
                Result = foods
            });
        }


    }
}
