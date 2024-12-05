using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Helper;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperApiController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly EmailPoster _emailPoster;
        private readonly ResponseDto _response;
        private readonly SignInManager<User> _signInManager;
        public HelperApiController(CloudinaryService cloudinaryService, EmailPoster emailPoster, SignInManager<User> signInManager)
        {
            _cloudinaryService = cloudinaryService;
            _response = new ResponseDto();
            _emailPoster = emailPoster;
            _signInManager = signInManager;
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

        [HttpPost("SendConfirmEmail")]
        public async Task<ResponseDto> SendConfirmEmail()
        {

            var user = await _signInManager.UserManager.GetUserAsync(User);
            try
            {
                var template = _emailPoster.EmailConfirmTemplate(user.FullName,$"https://nanafoodapi20241110164928.azurewebsites.net/api/Auth/EmailConfirmation/{Uri.EscapeDataString(user.Email)}");
                var response = _emailPoster.SendMail(user.Email, "Xác thực email", template);

                if (response == "gửi mail thành công")
                {
                    _response.IsSuccess = true;
                    _response.Message = response;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = response;
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
