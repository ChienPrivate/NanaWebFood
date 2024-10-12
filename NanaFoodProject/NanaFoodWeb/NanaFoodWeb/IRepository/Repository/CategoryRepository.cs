using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;

namespace NanaFoodWeb.IRepository.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IBaseService _baseService;
        public CategoryRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> DeactivateAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Category/Unactive/{id}"
            });
        }
    }
}
