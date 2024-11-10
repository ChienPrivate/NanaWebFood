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
        private readonly EmailPoster _emailPoster;
        private readonly ResponseDto _response;
        public HelperApiController(CloudinaryService cloudinaryService, EmailPoster emailPoster)
        {
            _cloudinaryService = cloudinaryService;
            _response = new ResponseDto();
            _emailPoster = emailPoster;
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


        [HttpDelete("ImageRemover")]
        public async Task<ResponseDto> RemoveImage(string imageUrl)
        {
            try
            {
                var result = await _cloudinaryService.DeleteImage(imageUrl);
                if (result)
                {
                    _response.Message = "Gỡ ảnh thành công";
                    _response.Result = imageUrl;
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.Message = "Gỡ ảnh không thành công";
                    _response.IsSuccess = false;
                }
                
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
