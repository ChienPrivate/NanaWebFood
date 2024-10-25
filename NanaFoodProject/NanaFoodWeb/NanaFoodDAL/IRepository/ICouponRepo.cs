using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface ICouponRepo
    {
        Task<ResponseDto>GetAll(int page , int pageSize);
        Task<ResponseDto>Create(Coupon  coupon);
        Task<ResponseDto> GetById(string id, int page, int pageSize);
        Task<ResponseDto> Update(Coupon coupon);
        Task<ResponseDto> DeleteById(string id);
    }
}
