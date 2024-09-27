using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDTO, bool withBearer = true);
    }
}
