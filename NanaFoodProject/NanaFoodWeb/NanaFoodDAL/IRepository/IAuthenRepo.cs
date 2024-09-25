using NanaFoodDAL.Dto;
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
    }
}
