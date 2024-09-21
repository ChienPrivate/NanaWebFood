using Azure;
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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly ApplicationDbContext _applicationDbContext;
        public CategoryController(ICategoryRepo repo, ApplicationDbContext applicationDbContext)
        {
            _categoryRepo = repo;
            _applicationDbContext = applicationDbContext;
        }


        // GET: api/category
        [HttpGet]
        public ActionResult<ResponseDto> GetAllCategories(int page = 1, int pageSize = 10)
        {
            var response = _categoryRepo.GetAll(page, pageSize);
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
        [HttpGet("SearchName/{name}")]
        public ActionResult<ResponseDto> GetCategoryByName([FromRoute] string name, int page = 1, int pageSize = 10)
        {
            var response = _categoryRepo.GetByName(name, page, pageSize);
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

        /*[HttpDelete("DeleteAllInDb")]
        public async Task<ResponseDto> DeleteAllEnity()
        {
            var response = await _applicationDbContext.Database.EnsureDeletedAsync();

            if (response != null)
            {
                return new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Delete successful",
                    Result = response
                };
            }

            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Delete failed",
                Result = response
            };
        }*/
    }
}
