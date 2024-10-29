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

        public async Task<ResponseDto> CategoryCount()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Category/CategoryMenu"
            });
            
        }

        public async Task<ResponseDto> DeactivateAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Category/Unactive/{id}"
            });
        }

        public async Task<ResponseDto> GetAllCategoriesAsync(int page, int pageSize, bool isSelectAll)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Category?page={page}&pageSize={pageSize}&isSelectAll={isSelectAll}"
            });
        }

        public async Task<ResponseDto> GetCategoryById(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Category/{id}"
            });
        }
    }
}
