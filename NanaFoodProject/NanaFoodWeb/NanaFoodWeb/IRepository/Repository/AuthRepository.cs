using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
        public Task<ResponseDto> ChangePasswordAsync(User user, ChangePasswordDto changePass)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> CheckEmailConfirm(string email)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/Auth/CheckEmailConfirm/{email}"
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
    }
}
