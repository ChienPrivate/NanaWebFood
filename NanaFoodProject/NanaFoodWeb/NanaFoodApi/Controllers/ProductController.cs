using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.IRepository.Repository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ProductController(IProductRepository foodService, IMapper mapper) : ControllerBase
    {
        readonly IProductRepository _foodService = foodService;
        readonly IMapper _mapper = mapper;

        [HttpGet]
        public ActionResult<ResponseDto> GetAll(int page = 1, int pageSize = 10)
        {
            var response = _foodService.GetAll(page, pageSize);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public ResponseDto GetById([FromRoute]int id)
        {
            return _foodService.GetById(id);
        }

        [HttpPost]
        public ResponseDto Create([FromBody] ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<Product>(productDto);
                var response = _foodService.Create(product);
                return response;
            }
            return null;
        }

        [HttpDelete("{id:int}")]
        public ResponseDto Delete([FromRoute] int id)
        {
            return _foodService.Delete(id);
        }

        [HttpGet("FilterCategoryID/{categoryId:int}")]
        public ResponseDto GetByCategoryId([FromRoute] int categoryId, int page = 1, int pageSize = 10)
        {
            var products = _foodService.GetByCategoryId(categoryId, page, pageSize);
            return products;
        }

        [HttpGet("Filter")]
        public ResponseDto GetByFilter(double? minrange, double? maxrange, int page = 1, int pageSize = 10)
        {
            return _foodService.GetByFilter(minrange, maxrange, page, pageSize);
        }

        [HttpGet("Get-by-name")]
        public ResponseDto GetBySearch(string query, int page = 1, int pageSize = 10)
        {
            return _foodService.GetBySearch(query, page, pageSize);
        }


        [HttpGet("Sorting")]
        public ResponseDto Sorting(string sort, int page = 1, int pageSize = 10)
        {
            return _foodService.Sorting(sort, page, pageSize);
        }

        [HttpGet("Get-top-view")]
        public ResponseDto TopViewed(int page = 1, int pageSize = 10)
        {
            return _foodService.TopViewed(page, pageSize);
        }

        [HttpPut]
        public ResponseDto Update([FromBody] ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var response = _foodService.Update(_mapper.Map<Product>(productDto));
                return response;
            }
            return null;
        }

        [HttpPut("active/{id}")]
        public ActionResult<ResponseDto> ActiveCategory([FromRoute] int id)
        {
            var response = _foodService.ModifyStatus(id, true);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("Unactive/{id}")]
        public ActionResult<ResponseDto> UnActiveCategory([FromRoute] int id)
        {
            var response = _foodService.ModifyStatus(id, false);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
