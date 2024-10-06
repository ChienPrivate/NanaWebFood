using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;

namespace NanaFoodWeb.IRepository.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IBaseService _baseService;
        public AuthRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public Task<ResponseDto> ChangePasswordAsync(User user, ChangePasswordDto changePass)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> LoginAsync(LoginDto login)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> LogOutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> RegisterAsync(RegisterDto regis)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = regis,
                Url = StaticDetails.APIBase + "/api/Auth/Register"
            });
        }
    }
}
