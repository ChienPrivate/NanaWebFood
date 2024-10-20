using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.IRepository;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUserEndpoint(CreateUserRequestDto createUserRequestDto)
        {
            var result = await _userRepository.CreateUserAsync(createUserRequestDto);

            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _userRepository.GetAllUserAsync();

            return Ok(result);
        }

        [HttpDelete("users")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var result = await _userRepository.DeleteUserAsync(id);

            return Ok(result);
        }

        [HttpPut("users")]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserRequestDto updateUserRequestDto)
        {
            var result = await _userRepository.UpdateUserAsync(updateUserRequestDto);

            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var result = await _userRepository.GetAllRolesAsync();

            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string userId)
        {
            var result = await _userRepository.GetUserByIdAsync(userId);

            return Ok(result);
        }
    }
}
