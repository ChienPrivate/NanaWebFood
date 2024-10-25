using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
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
        private readonly IMapper _mapper; 
        public CouponController(ICouponRepo couponRepo, IMapper mapper)
        {
            _couponRepo = couponRepo;
            _mapper = mapper;

        }
        [HttpPost("createCP")]
        public async Task<ResponseDto> CreatCoupon([FromBody] CouponDto dto)
        {
            if(ModelState.IsValid)
            {
                var reuslt = _mapper.Map<Coupon>(dto);
                var res = await _couponRepo.Create(reuslt);
                return res;
            }
            return null;
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
        public async Task<ResponseDto> Update([FromBody] CouponDto dto)
        {
            if( ModelState.IsValid )
            {
                var result = _mapper.Map<Coupon>(dto);
                var res = await _couponRepo.Update(result);
                return res;
            }
            return null;
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
    }
}
