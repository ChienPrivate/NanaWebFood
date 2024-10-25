using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using System.Net.WebSockets;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponTypeController: ControllerBase
    {
        private readonly ICouponTypeRepo _couponTypeRepo;
        private readonly IMapper _mapper;
        
        public CouponTypeController(ICouponTypeRepo couponTypeRepo, IMapper mapper)
        {
            _couponTypeRepo = couponTypeRepo;
            _mapper = mapper;
            ResponseDto response = new ResponseDto();

        }
        [HttpPost("couponCP")]
        public async Task<ResponseDto>CreateCouponType([FromBody] CouponTypeDto dto)
        {
            if(ModelState.IsValid)
            {
                var reustl = _mapper.Map<CouponType>(dto);
                var response= await _couponTypeRepo.Create(reustl); 
                return response;
            }
            return null;

        }
        [HttpDelete("delete")]
        public async Task<ResponseDto>Delete(int id)
        {
            var result = await _couponTypeRepo.Delete(id);
            return result;
        }
        [HttpPut("updateCpType")]
        public async Task<ResponseDto> Update([FromBody] CouponTypeDto dto)
        {
            if (ModelState.IsValid)
            {
              var result =  _mapper.Map<CouponType>(dto);
              var response = await _couponTypeRepo.Update(result);
                return response;
            }
            return null; 
        }
        [HttpGet("getAllCp")]
        public async Task<ResponseDto>GetAll(int page =1, int pageSize= 10)
        {
            if (ModelState.IsValid)
            {
                var result = await _couponTypeRepo.GetAll(page, pageSize);
                return result;
            }
            return null;
        }
       
        
    }
}
