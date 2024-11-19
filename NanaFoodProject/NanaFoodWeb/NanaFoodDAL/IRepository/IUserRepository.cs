using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IUserRepository
    {
        Task<ResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequestDto);
        Task<ResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequestDto);
        Task<ResponseDto> DeleteUserAsync(string userId);
        Task<ResponseDto> GetAllRolesAsync();
        Task<ResponseDto> GetAllUserAsync();
        Task<ResponseDto> GetUserByIdAsync(string userId);
        Task<ResponseDto> GetUsersByRole(string role);
        Task<ResponseDto> UpdateUserState(string userId, int state);

    }
}
