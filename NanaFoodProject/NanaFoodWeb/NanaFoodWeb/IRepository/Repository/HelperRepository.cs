using Microsoft.AspNetCore.Components.Forms;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;

namespace NanaFoodWeb.IRepository.Repository
{
    public class HelperRepository : IHelperRepository
    {
        private readonly IBaseService _baseService;
        public HelperRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> UploadImageAsync(IFormFile file)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + "/api/HelperApi/ImagePoster",
                Data = file,
            });
        }

        public async Task<ResponseDto> SendConfirmEmail()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + $"/api/HelperApi/SendConfirmEmail",
            });
        }
    }
}
