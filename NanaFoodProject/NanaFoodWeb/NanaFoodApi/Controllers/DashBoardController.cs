using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.IRepository.Repository;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardRepository _dashboardRepository;
        public DashBoardController(IDashBoardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("GetProfitInDay")]
        public async Task<IActionResult> GetProfitInDay()
        {
            var reponse = await _dashboardRepository.GetProfitInDay(DateTime.Now);

            return Ok(reponse);
        }

        [HttpGet("GetprofitInWeek")]
        public async Task<IActionResult> GetProfitInWeek()
        {
            var response = await _dashboardRepository.GetProfitInWeek(DateTime.Now);

            return Ok(response);
        }

        [HttpGet("GetProfitByMonth/{month}")]
        public async Task<IActionResult> GetProfitByMonthAsync(int month)
        {
            var reponse = await _dashboardRepository.GetProfitByMonthAsync(month);

            return Ok(reponse);
        }

        [HttpGet("GetProfitByYear/{year}")]
        public async Task<IActionResult> GetProfitByYearAsync(int year)
        {
            var reponse = await _dashboardRepository.GetProfitByYearAsync(year);

            return Ok(reponse);
        }

        [HttpGet("GetProfit")]
        public async Task<IActionResult> GetProfitAsync()
        {
            var reponse = await _dashboardRepository.GetProfitAsync();

            return Ok(reponse);
        }

        [HttpGet("GetDeliveringOrder")]
        public async Task<IActionResult> GetDeliveringOrderAsync()
        {
            var reponse = await _dashboardRepository.GetDeliveringOrderAsync();

            return Ok(reponse);
        }

        [HttpGet("GetCancelOrderInMonth/{month}")]
        public async Task<IActionResult> GetCancelOrderInMonthAsync(int month)
        {
            var reponse = await _dashboardRepository.GetCancelOrderInMonthAsync(month);

            return Ok(reponse);
        }

        [HttpGet("GetCompleteOrderInMonth/{month}")]
        public async Task<IActionResult> GetCompleteOrderInMonthAsync(int month)
        {
            var reponse = await _dashboardRepository.GetCompleteOrderInMonthAsync(month);

            return Ok(reponse);
        }

        [HttpGet("GetProfitEachMonth/{year}")]
        public async Task<IActionResult> GetProfitEachMonth(int year)
        {
            var reponse = await _dashboardRepository.GetProfitEachMonth(year);

            return Ok(reponse);
        }
    }
}
