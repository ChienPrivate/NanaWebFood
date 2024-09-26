﻿using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IAuthenRepo
    {
        public Task<ResponseDto> Login(LoginDTO login);
        public Task<ResponseDto> Register(RegisterDto regis);
        public Task<ResponseDto> GetAllUser(int page, int pageSize);
        public Task<ResponseDto> SearchMail(string email, int page, int pageSize);
        public Task<ResponseDto> SearchName(string fullname, int page, int pageSize);
        public Task<ResponseDto> DeleteUser(string email);
    }
}
