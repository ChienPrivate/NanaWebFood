using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;

namespace NanaFoodWeb.IRepository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IBaseService _baseService;
        public UserRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + $"/api/User/users",
                Data = createUserRequestDto,
            });
        }

        public async Task<ResponseDto> DeleteUserAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto() 
            { 
                ApiType = StaticDetails.ApiType.DELETE, 
                Url = StaticDetails.APIBase + $"/api/User/users?id={userId}"
            });
        }

        public async Task<ResponseDto> GetAllRolesAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/User/roles"
            });
        }

        public async Task<ResponseDto> GetAllUserAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + "/api/User/users"
            });
        }

        public async Task<ResponseDto> GetUserByIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/User/users/{userId}"
            });
        }

        public async Task<ResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto() 
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.APIBase + "/api/User/users",
                Data = updateUserRequestDto,
            });
        }
    }
}
