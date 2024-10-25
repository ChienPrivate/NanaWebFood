using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface ICouponTypeRepo
    {
        Task<ResponseDto> GetAll(int page, int pageSize, bool isSelectAll = true);
        Task<ResponseDto> Update(CouponType couponType); 
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto> Create(CouponType couponType);
        Task<ResponseDto> GetByCpTypeId(int id, int page, int pageSize);
    }
}
