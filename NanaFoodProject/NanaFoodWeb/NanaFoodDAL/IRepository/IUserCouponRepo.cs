using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IUserCouponRepo
    {
        Task<ResponseDto> ApplyCoupon(UserCoupon userCoupon);
    }
}
