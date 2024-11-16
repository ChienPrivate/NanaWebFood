using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;
using NuGet.Protocol.Plugins;

namespace NanaFoodWeb.IRepository.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IBaseService _baseService;
        public AuthRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> ChangePasswordAsync(ChangePasswordDto changePass)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = changePass,
                Url = StaticDetails.APIBase + $"/api/Auth/ChangePassword"
            });
        }

        public async Task<ResponseDto> CheckEmailConfirm(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Auth/CheckEmailConfirm/{userId}"
            });
        }

        public async Task<ResponseDto> ForgotPassword(string email)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.APIBase + $"/api/Auth/ForgotPassword/{email}"
            });
        }

        public async Task<ResponseDto> GetInfo()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + "/api/Auth/GetInformation",
            });
        }

        public async Task<ResponseDto> LoginAsync(LoginDto login)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = login,
                Url = StaticDetails.APIBase + "/api/Auth/Login"
            });
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

        public async Task<ResponseDto> UpdateInfo(UserDto user)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = user,
                Url = StaticDetails.APIBase + "/api/Auth/UpdateUser"
            });
        }
    }
}
