using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IAuthRepository
    {
        public Task<ResponseDto> LoginAsync(LoginDto login);
        public Task<ResponseDto> RegisterAsync(RegisterDto regis);
        public Task<ResponseDto> LogOutAsync();
        public Task<ResponseDto> ChangePasswordAsync(User user, ChangePasswordDto changePass);
    }
}
