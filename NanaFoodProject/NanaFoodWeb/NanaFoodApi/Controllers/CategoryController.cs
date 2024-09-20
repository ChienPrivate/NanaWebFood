using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo;

        public CategoryController(ICategoryRepo repo)
        {
            _categoryRepo = repo;
        }


        // GET: api/category
        [HttpGet]
        public ActionResult<ResponseDto> GetAllCategories()
        {
            var response = _categoryRepo.GetAll();
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        // GET: api/category/{id}
        [HttpGet("{id}")]
        public ActionResult<ResponseDto> GetCategoryById(int id)
        {
            var response = _categoryRepo.GetById(id);
            if (!response.IsSuccess)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // GET: api/category/{name}
        [HttpGet("SearchName/{name}")]
        public ActionResult<ResponseDto> GetCategoryByName(string name)
        {
            var response = _categoryRepo.GetByName(name);
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
        public ActionResult<ResponseDto> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid || id != categoryDto.CategoryId)
            {
                return BadRequest("Đầu vào không hợp lệ.");
            }

            var category = new Category
            {
                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName,
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
        public ActionResult<ResponseDto> DeleteCategory(int id)
        {
            var response = _categoryRepo.Delete(id);
            if (!response.IsSuccess)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
