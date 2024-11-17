using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,employee")]
    public class ProductController(IProductRepository foodService, IMapper mapper) : ControllerBase
    {
        readonly IProductRepository _foodService = foodService;
        readonly IMapper _mapper = mapper;

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả các sản phẩm, hỗ trợ phân trang và lựa chọn lấy tất cả.
        /// </remarks>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <param name="isSelectAll">Nếu là true, trả về tất cả các sản phẩm.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm.</response>
        /// <response code="404">Không tìm thấy sản phẩm nào.</response>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<ResponseDto> GetAll(int page = 1, int pageSize = 10, bool isSelectAll = true)
        {
            var response = _foodService.GetAll(page, pageSize, isSelectAll);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        /// <remarks>
        /// API này trả về thông tin chi tiết của một sản phẩm dựa trên ID của sản phẩm.
        /// </remarks>
        /// <param name="id">ID của sản phẩm.</param>
        /// <returns>
        /// - 200 OK nếu lấy thông tin sản phẩm thành công.
        /// - 404 NotFound nếu không tìm thấy sản phẩm với ID này.
        /// </returns>
        /// <response code="200">Trả về thông tin sản phẩm.</response>
        /// <response code="404">Không tìm thấy sản phẩm.</response>
        [HttpGet("getbyId/{id:int}")]
        [AllowAnonymous]
        public ResponseDto GetById([FromRoute] int id)
        {
            return _foodService.GetById(id);
        }

        /// <summary>
        /// Tạo sản phẩm mới
        /// </summary>
        /// <remarks>
        /// API này cho phép tạo một sản phẩm mới.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "productName": "Product Name",
        ///     "imageUrl": "https://example.com/image.jpg",
        ///     "price": 100000,
        ///     "view": 0,
        ///     "description": "Product description",
        ///     "isActive": true,
        ///     "categoryId": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="productDto">Thông tin sản phẩm mới.</param>
        /// <returns>
        /// - 200 OK nếu tạo sản phẩm thành công.
        /// - 400 BadRequest nếu có lỗi trong dữ liệu đầu vào.
        /// </returns>
        /// <response code="200">Sản phẩm được tạo thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi tạo sản phẩm.</response>
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

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép xóa một sản phẩm dựa trên ID của sản phẩm.
        /// </remarks>
        /// <param name="id">ID của sản phẩm cần xóa.</param>
        /// <returns>
        /// - 200 OK nếu xóa sản phẩm thành công.
        /// </returns>
        /// <response code="200">Sản phẩm đã được xóa thành công.</response>
        [HttpDelete("delete/{id:int}")]
        public ResponseDto Delete([FromRoute] int id)
        {
            return _foodService.Delete(id);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo ID danh mục
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các sản phẩm trong một danh mục cụ thể, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="categoryId">ID của danh mục.</param>
        /// <param name="page">Số trang.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách sản phẩm theo danh mục thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm theo danh mục.</response>
        [HttpGet("FilterCategoryID/{categoryId:int}")]
        [AllowAnonymous]
        public ResponseDto GetByCategoryId([FromRoute] int categoryId, int page = 1, int pageSize = 10)
        {
            var products = _foodService.GetByCategoryId(categoryId, page, pageSize);
            return products;
        }

        /// <summary>
        /// Lọc sản phẩm theo giá
        /// </summary>
        /// <remarks>
        /// API này cho phép lọc danh sách sản phẩm theo khoảng giá, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="minrange">Giá thấp nhất.</param>
        /// <param name="maxrange">Giá cao nhất.</param>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu lọc sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm theo khoảng giá.</response>
        [HttpGet("Filter")]
        [AllowAnonymous]
        public ResponseDto GetByFilter(double? minrange, double? maxrange, int page = 1, int pageSize = 10)
        {
            return _foodService.GetByFilter(minrange, maxrange, page, pageSize);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên
        /// </summary>
        /// <remarks>
        /// API này cho phép tìm kiếm các sản phẩm dựa trên tên sản phẩm, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="query">Tên sản phẩm cần tìm kiếm.</param>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu tìm kiếm sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm theo tên.</response>
        [HttpGet("Get-by-name")]
        [AllowAnonymous]
        public ResponseDto GetBySearch(string query, int page = 1, int pageSize = 10)
        {
            return _foodService.GetBySearch(query, page, pageSize);
        }

        /// <summary>
        /// Sắp xếp danh sách sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép sắp xếp danh sách sản phẩm theo tiêu chí, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="sort">Tiêu chí sắp xếp, ví dụ "price" hoặc "view".</param>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu sắp xếp sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm đã sắp xếp.</response>
        [HttpGet("Sorting")]
        [AllowAnonymous]
        public ResponseDto Sorting(string sort, int page = 1, int pageSize = 10)
        {
            return _foodService.Sorting(sort, page, pageSize);
        }

        /// <summary>
        /// Lấy sản phẩm được xem nhiều nhất
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các sản phẩm có lượt xem cao nhất, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách sản phẩm có lượt xem cao nhất thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm có lượt xem cao nhất.</response>
        [HttpGet("Get-top-view")]
        [AllowAnonymous]
        public ResponseDto TopViewed(int page = 1, int pageSize = 10)
        {
            return _foodService.TopViewed(page, pageSize);
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật thông tin của một sản phẩm.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "productId": 1,
        ///     "productName": "Updated Product Name",
        ///     "imageUrl": "https://example.com/image.jpg",
        ///     "price": 120.0,
        ///     "view": 10,
        ///     "description": "Updated product description",
        ///     "isActive": true,
        ///     "categoryId": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="productDto">Thông tin sản phẩm cần cập nhật.</param>
        /// <returns>
        /// - 200 OK nếu cập nhật sản phẩm thành công.
        /// - 400 BadRequest nếu có lỗi trong dữ liệu đầu vào.
        /// </returns>
        /// <response code="200">Sản phẩm được cập nhật thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi cập nhật sản phẩm.</response>
        [HttpPut("update")]
        public ResponseDto Update([FromBody] ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var response = _foodService.Update(_mapper.Map<Product>(productDto));
                return response;
            }
            return null;
        }

        /// <summary>
        /// Kích hoạt sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép kích hoạt trạng thái của một sản phẩm dựa trên ID.
        /// 
        /// **Sample Request**:
        /// PUT /api/Product/active/1
        /// </remarks>
        /// <param name="id">ID của sản phẩm cần kích hoạt.</param>
        /// <returns>
        /// - 200 OK nếu kích hoạt sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trạng thái sản phẩm được kích hoạt thành công.</response>
        /// <response code="400">Không thể kích hoạt sản phẩm.</response>
        [HttpPut("active/{id}")]
        public ActionResult<ResponseDto> ActiveProduct([FromRoute] int id)
        {
            var response = _foodService.ModifyStatus(id, true);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Hủy kích hoạt sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép hủy kích hoạt trạng thái của một sản phẩm dựa trên ID.
        /// 
        /// **Sample Request**:
        /// PUT /api/Product/Unactive/1
        /// </remarks>
        /// <param name="id">ID của sản phẩm cần hủy kích hoạt.</param>
        /// <returns>
        /// - 200 OK nếu hủy kích hoạt sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trạng thái sản phẩm được hủy kích hoạt thành công.</response>
        /// <response code="400">Không thể hủy kích hoạt sản phẩm.</response>
        [HttpPut("Unactive/{id}")]
        public ActionResult<ResponseDto> UnActiveProduct([FromRoute] int id)
        {
            var response = _foodService.ModifyStatus(id, false);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo danh mục nhưng không bao gồm sản phẩm hiện tại
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các sản phẩm cùng danh mục nhưng không bao gồm sản phẩm hiện tại, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="productId">ID của sản phẩm hiện tại.</param>
        /// <param name="categoryid">ID của danh mục sản phẩm.</param>
        /// <param name="page">Số trang.</param>
        /// <param name="pageSize">Số lượng sản phẩm trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách sản phẩm thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách sản phẩm cùng danh mục trừ sản phẩm hiện tại.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc có lỗi khi lấy sản phẩm.</response>
        [HttpGet("ExcludeSameProduct/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategoryIdExcludeSameProduct(int productId, int categoryid, int page, int pageSize)
        {
            var response = await _foodService.GetByCategoryIdExcludeSameProduct(productId, categoryid, page, pageSize);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("get_product")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct()
        {
            var response = _foodService.GetProduct();
            if(!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("get_product_images/{ProductId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImages([FromRoute] int ProductId)
        {
            var response = await _foodService.GetImages(ProductId);
            if(response != null)
            {
                return Ok(new ResponseDto { IsSuccess = true, Result = response, Message = "Lấy danh sánh ảnh thành công" });
            }
            return NotFound();
        }
    }
}
