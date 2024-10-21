using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface ICartRepo
    {
        public Task<ResponseDto> AddToCart(CartDetailsDto cartdetailDto);
        public Task<ResponseDto> GetCart(User user);
        public Task<ResponseDto> DeleteCart(int ProductId, string UserID);
        public Task<ResponseDto> UpdateCart(int ProductId, string message);
    }
}
