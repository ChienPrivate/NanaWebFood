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

        // GET: api/category
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



        // GET: api/category/{id}
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

        // GET: api/category/SearchName/{name}
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

        // POST: api/category
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

        // PUT: api/category/{id}
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

        // DELETE: api/category/{id}
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

        // Số lượng sản phẩm của mỗi category
        [AllowAnonymous]
        [HttpGet("CategoryMenu")]
        public IActionResult GetFoods()
        {
            var foods = _productRepo.Products.GroupBy(x => x.CategoryId).Select(c => new
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
