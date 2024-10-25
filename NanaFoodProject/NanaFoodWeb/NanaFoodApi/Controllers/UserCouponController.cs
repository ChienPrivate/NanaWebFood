using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCouponController : ControllerBase
    {
        private readonly IUserCouponRepo  _userCouponRepo;
        private readonly IMapper  _mapper;
        public UserCouponController(IUserCouponRepo userCouponRepo, IMapper mapper)
        {
            _userCouponRepo = userCouponRepo;
            _mapper = mapper;
        }
        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> Apply([FromBody] UserCouponDto dto)
        {
            if(ModelState.IsValid)
            {
                var result = _mapper.Map<UserCoupon>(dto);
                var response = await _userCouponRepo.ApplyCoupon(result);
                return response;
            }
            return null;
        }
    }
}
