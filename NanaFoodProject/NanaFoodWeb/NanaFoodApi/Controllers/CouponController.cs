using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepo _couponRepo;
        private readonly IUserCouponRepo _userCouponRepo;
        private readonly SignInManager<User> _sm;
        private readonly IMapper _mapper; 
        public CouponController(ICouponRepo couponRepo, IMapper mapper, SignInManager<User> sm, IUserCouponRepo userCouponRepo)
        {
            _couponRepo = couponRepo;
            _mapper = mapper;
            _sm = sm;
            _userCouponRepo = userCouponRepo;
        }

        /// <summary>
        /// Tạo mã giảm giá mới
        /// </summary>
        /// <remarks>
        /// API này cho phép tạo một mã giảm giá mới.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "couponTypeId": 1,
        ///     "couponCode": "SALE25",
        ///     "description": "Giảm giá 25.000 cho đơn hàng trên 100.000",
        ///     "discount": 25000,
        ///     "minAmount": 100000,
        ///     "couponStartDate": "2024-10-30T04:14:13.237Z",
        ///     "endStart": "2024-11-30T04:14:13.237Z",
        ///     "maxUsage": 100,
        ///     "timesUsed": 0
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">Thông tin chi tiết của mã giảm giá.</param>
        /// <returns>
        /// - 200 OK nếu tạo mã giảm giá thành công.
        /// - 400 BadRequest nếu có lỗi trong dữ liệu đầu vào.
        /// </returns>
        /// <response code="200">Mã giảm giá được tạo thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi tạo mã giảm giá.</response>
        [HttpPost("createCP")]
        public async Task<IActionResult> CreatCoupon([FromBody] CouponDto dto)
        {
            if(ModelState.IsValid)
            {
                var reuslt = _mapper.Map<Coupon>(dto);
                var res = await _couponRepo.Create(reuslt);
                return Ok(res);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Lấy danh sách tất cả mã giảm giá
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả mã giảm giá, hỗ trợ phân trang.
        /// </remarks>
        /// <param name="page">Số trang hiện tại.</param>
        /// <param name="pageSize">Số lượng coupon mục trên mỗi trang.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách mã giảm giá thành công.
        /// - 400 BadRequest nếu có lỗi xảy ra.
        /// </returns>
        /// <response code="200">Trả về danh sách mã giảm giá.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi lấy danh sách mã giảm giá.</response>
        [HttpGet("GetAllCoupon")]
        public async Task<ActionResult<ResponseDto>>GetAll()
        {
            var result = await _couponRepo.GetAll();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result); 
        }

        /// <summary>
        /// Cập nhật mã giảm giá
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật thông tin của một mã giảm giá.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "couponTypeId": 1,
        ///     "couponCode": "SALE25",
        ///     "description": "Giảm giá 25.000 cho đơn hàng trên 100.000",
        ///     "discount": 25000,
        ///     "minAmount": 100000,
        ///     "couponStartDate": "2024-10-30T04:14:13.237Z",
        ///     "endStart": "2024-11-30T04:14:13.237Z",
        ///     "maxUsage": 100,
        ///     "timesUsed": 0
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">Thông tin mã giảm giá cần cập nhật.</param>
        /// <returns>
        /// - 200 OK nếu cập nhật mã giảm giá thành công.
        /// - 400 BadRequest nếu có lỗi trong dữ liệu đầu vào.
        /// </returns>
        /// <response code="200">Mã giảm giá được cập nhật thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi cập nhật mã giảm giá.</response>
        /// <response code="404">Mã giảm giá không tồn tại.</response>
        [HttpPut("updateCoupon")]
        public async Task<IActionResult> Update([FromBody] CouponDto dto)
        {
            if( ModelState.IsValid )
            {
                var result = _mapper.Map<Coupon>(dto);
                var res = await _couponRepo.Update(result);
                return Ok(res);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Xóa mã giảm giá
        /// </summary>
        /// <remarks>
        /// API này cho phép xóa một mã giảm giá bằng cách cung cấp mã (code) của nó.
        /// </remarks>
        /// <param name="code">Mã của mã giảm giá cần xóa.</param>
        /// <returns>
        /// - 200 OK nếu xóa mã giảm giá thành công.
        /// - 400 BadRequest nếu có lỗi xảy ra.
        /// </returns>
        /// <response code="200">Mã giảm giá đã được xóa thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi xóa mã giảm giá.</response>
        /// <response code="404">Mã giảm giá không tồn tại.</response>
        [HttpDelete("DeleteCoupon")]
        public async Task<ActionResult<ResponseDto>> DeleteCoupon(string code)
        {
            var result = await _couponRepo.DeleteById(code);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("getbyId/{code}")]
        public async Task<IActionResult>GetById([FromRoute] string code)
        {
            var result = await _couponRepo.GetById(code);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra và áp dụng mã giảm giá cho người dùng
        /// </summary>
        /// <remarks>
        /// API này kiểm tra xem mã giảm giá có hợp lệ và có thể áp dụng cho người dùng hiện tại hay không.
        /// </remarks>
        /// <param name="code">Mã giảm giá cần kiểm tra.</param>
        /// <returns>
        /// - 200 OK nếu mã giảm giá hợp lệ và được áp dụng.
        /// - 400 BadRequest nếu mã giảm giá không hợp lệ hoặc không thể áp dụng.
        /// </returns>
        /// <response code="200">Mã giảm giá hợp lệ và được áp dụng cho người dùng.</response>
        /// <response code="400">Mã giảm giá không hợp lệ hoặc không thể áp dụng.</response>
        /// <response code="404">Mã giảm giá không tồn tại.</response>

        [HttpGet("Check/{code}")]
        public async Task<IActionResult>Check(string code)
        {
            var userId = _sm.UserManager.GetUserId(User);
            var result = await _userCouponRepo.ApplyCoupon(userId,code);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
