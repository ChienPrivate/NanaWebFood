﻿using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IAuthRepository
    {
        public Task<ResponseDto> LoginAsync(LoginDto login);
        public Task<ResponseDto> RegisterAsync(RegisterDto regis);
        public Task<ResponseDto> LogOutAsync();
        public Task<ResponseDto> ChangePasswordAsync(ChangePasswordDto changePass);
        public Task<ResponseDto> CheckEmailConfirm(string userId);
        public Task<ResponseDto> ForgotPassword(string email);
        public Task<ResponseDto> GetInfo();
        public Task<ResponseDto> UpdateInfo(UserDto user);
        public Task<ResponseDto> GetUserStatus(string userId);
    }
}
