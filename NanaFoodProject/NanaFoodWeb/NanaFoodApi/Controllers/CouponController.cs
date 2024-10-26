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
        [HttpGet("GetAllCoupon")]
        public async Task<ActionResult<ResponseDto> >GetAll(int page =1, int pageSize=10)
        {
            var result = await _couponRepo.GetAll(page, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result); 
        }
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
