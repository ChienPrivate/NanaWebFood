using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;
using System;

namespace NanaFoodWeb.IRepository.Repository
{
    public class DashBoardRepository : IDashBoardRepository
    {
        private readonly IBaseService _baseService;
        public DashBoardRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> GetCancelOrderInMonthAsync(int month)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetCancelOrderInMonth/{month}"
            });
        }

        public async Task<ResponseDto> GetCompleteOrderInMonthAsync(int month)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetCompleteOrderInMonth/{month}"
            });
        }

        public async Task<ResponseDto> GetDeliveringOrderAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetDeliveringOrder"
            });
        }

        public async Task<ResponseDto> GetProfitAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetProfit"
            });
        }

        public async Task<ResponseDto> GetProfitByMonthAsync(int month)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetProfitByMonth/{month}"
            });
        }

        public async Task<ResponseDto> GetProfitByYearAsync(int year)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetProfitByYear/{year}"
            });
        }

        public async Task<ResponseDto> GetProfitEachMonth(int year)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetProfitEachMonth/{year}"
            });
        }

        public async Task<ResponseDto> GetProfitInDay(DateTime dateTime)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.APIBase + $"/api/DashBoard/GetProfitInDay/{dateTime}"
            });
        }
    }
}
