using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IWishListRepo
    {
        public Task<ResponseDto> AddToWishList(WishListDto dto);
        public Task<ResponseDto> GetWishList(User user);
        public Task<ResponseDto> DeleteWishList(int ProductId, string UserID);
        public Task<ResponseDto> RemoveAllWishList(string userId);
    }
}
