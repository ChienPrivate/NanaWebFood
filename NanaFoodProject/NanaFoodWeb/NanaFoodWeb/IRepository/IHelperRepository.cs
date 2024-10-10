using Microsoft.AspNetCore.Components.Forms;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IHelperRepository
    {
        public Task<ResponseDto> UploadImageAsync(IFormFile file);
    }
}
