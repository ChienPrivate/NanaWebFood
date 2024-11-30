using NanaFoodWeb.Models.Dto;

namespace NanaFoodWeb.IRepository
{
    public interface IDashBoardRepository
    {
        public Task<ResponseDto> GetProfitInDay();
        public Task<ResponseDto> GetProfitInWeek();
        public Task<ResponseDto> GetProfitByMonthAsync(int month);
        public Task<ResponseDto> GetProfitByYearAsync(int year);
        public Task<ResponseDto> GetProfitAsync();
        public Task<ResponseDto> GetDeliveringOrderAsync();
        public Task<ResponseDto> GetCancelOrderInMonthAsync(int month);
        public Task<ResponseDto> GetCompleteOrderInMonthAsync(int month);
        public Task<ResponseDto> GetProfitEachMonth(int year);
    }
}
