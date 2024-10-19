using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IUserRepository
    {
        Task<ResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequestDto);
        Task<ResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequestDto);
        Task<ResponseDto> DeleteUserAsync(string userId);
        Task<ResponseDto> GetAllRolesAsync();
        Task<ResponseDto> GetAllUserAsync();
        Task<ResponseDto> GetUserByIdAsync(string userId);
    }
}
