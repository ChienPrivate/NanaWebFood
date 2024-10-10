using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Helper;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperApiController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly ResponseDto _response;
        public HelperApiController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
            _response = new ResponseDto();
        }
        [HttpPost("ImagePoster")]
        public async Task<ResponseDto> PostImage(IFormFile file)
        {
            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file);
                _response.Message = "Tải ảnh thành công";
                _response.Result = imageUrl;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}
