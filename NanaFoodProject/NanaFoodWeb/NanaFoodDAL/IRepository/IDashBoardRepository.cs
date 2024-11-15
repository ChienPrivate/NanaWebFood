using NanaFoodDAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IDashBoardRepository
    {
        public Task<ResponseDto> GetProfitInDay(DateTime dateTime);
        public Task<ResponseDto> GetProfitByMonthAsync(int month);
        public Task<ResponseDto> GetProfitByYearAsync(int year);
        public Task<ResponseDto> GetProfitAsync();
        public Task<ResponseDto> GetDeliveringOrderAsync();
        public Task<ResponseDto> GetCancelOrderInMonthAsync(int month);
        public Task<ResponseDto> GetCompleteOrderInMonthAsync(int month);
        public Task<ResponseDto> GetProfitEachMonth(int year);
    }
}
