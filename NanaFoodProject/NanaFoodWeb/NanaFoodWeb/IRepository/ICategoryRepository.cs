using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICategoryRepository
    {
        Task<ResponseDto> DeactivateAsync(int id);
    }
}
