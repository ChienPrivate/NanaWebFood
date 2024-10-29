using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface ICategoryRepository
    {
        Task<ResponseDto> DeactivateAsync(int id);
        Task<ResponseDto> GetAllCategoriesAsync(int page, int pageSize, bool isSelectAll);
        Task<ResponseDto> GetCategoryById(int id);
        Task<ResponseDto> CategoryCount();
    }
}
