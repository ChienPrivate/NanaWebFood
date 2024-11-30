using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository.Repository
{
    public class DashBoardRepository : IDashBoardRepository
    {
        private ResponseDto _response;
        private readonly ApplicationDbContext _context;
        public DashBoardRepository(ApplicationDbContext context)
        {
            _context = context;
            _response = new ResponseDto();
        }

        public async Task<ResponseDto> GetCancelOrderInMonthAsync(int month)
        {
            try
            {
                var cancelOrders = await _context.Orders.Where(o => o.OrderDate.Month == DateTime.Now.Month && o.OrderStatus == "Đã huỷ").ToListAsync();

                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách đơn bị hủy trong tháng này";
                _response.Result = cancelOrders;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetCompleteOrderInMonthAsync(int month)
        {
            try
            {
                var completeOrder = await _context.Orders.Where(o => o.OrderDate.Month == DateTime.Now.Month && o.OrderStatus == "Đã giao").ToListAsync();

                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách đã giao thành công ";
                _response.Result = completeOrder;
            }
            catch(Exception ex) 
            {
                _response.IsSuccess= false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetDeliveringOrderAsync()
        {
            try
            {
                var deliveringOrder = await _context.Orders.Where(o => o.OrderDate.Date == DateTime.Now.Date && o.OrderStatus == "Đang giao").ToListAsync();

                _response.IsSuccess = true;
                _response.Message = "Lấy danh sách đang giao thành công";
                _response.Result = deliveringOrder;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message= ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetProfitAsync()
        {
            try
            {
                var profit = await _context.Orders
                    .Where(o => o.OrderStatus == "Đã giao" && o.PaymentStatus == "Đã thanh toán")
                    .SumAsync(o => o.Total);

                _response.IsSuccess = true;
                _response.Message = "Tính tổng doanh thu thành công";
                _response.Result = profit;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message= ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetProfitByMonthAsync(int month)
        {
            try
            {
                var profit = await _context.Orders
                    .Where(o => o.OrderStatus == "Đã giao" && o.PaymentStatus == "Đã thanh toán" && o.OrderDate.Month == DateTime.Now.Month)
                    .SumAsync(o => o.Total);

                _response.IsSuccess = true;
                _response.Message = "Tính tổng doanh thu trong tháng thành công";
                _response.Result = profit;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetProfitByYearAsync(int year)
        {
            try
            {
                var profit = await _context.Orders
                    .Where(o => o.OrderStatus == "Đã giao" && o.PaymentStatus == "Đã thanh toán" && o.OrderDate.Year == DateTime.Now.Year)
                    .SumAsync(o => o.Total);

                _response.IsSuccess = true;
                _response.Message = "Tính tổng doanh thu trong tháng thành công";
                _response.Result = profit;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetProfitInDay(DateTime dateTime)
        {
            try
            {
                var profit = await _context.Orders
                    .Where(o => o.OrderStatus == "Đã giao" && o.PaymentStatus == "Đã thanh toán" && o.OrderDate.Date == DateTime.Now.Date)
                    .SumAsync(o => o.Total);

                _response.IsSuccess = true;
                _response.Message = $"Tính tổng doanh thu ngày {dateTime.ToString("dd/MM/yy HH tt")}";
                _response.Result = profit;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message= ex.Message;
            }
            return _response;
        }

        // Phương thức để lấy doanh thu theo từng tháng trong năm
        public async Task<ResponseDto> GetProfitEachMonth(int year)
        {
            try
            {
                var monthlyRevenue = await _context.Orders.Where(o => o.OrderStatus == "Đã giao" && o.PaymentStatus == "Đã thanh toán" && o.OrderDate.Year == year).GroupBy(o => o.OrderDate.Month).Select(g => new { Month = g.Key, Revenue = g.Sum(o => o.Total) }).OrderBy(m => m.Month).ToListAsync();
                var fullYearRevenue = Enumerable.Range(1, 12).Select(month => new LineChartDto { Period = new DateTime(year, month, 1), Revenue = monthlyRevenue.FirstOrDefault(m => m.Month == month)?.Revenue ?? 0 }).ToList();
                _response.IsSuccess = true;
                _response.Message = "Thành công";
                _response.Result = fullYearRevenue;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Lỗi khi lấy dữ liệu: {ex.Message}";
            }
            return _response;
        }

        public async Task<ResponseDto> GetProfitInWeek(DateTime dateTime)
        {
            try
            {
                // Xác định ngày bắt đầu và kết thúc của tuần
                int diff = dateTime.DayOfWeek - DayOfWeek.Monday;  // Thứ hai là ngày đầu tuần (có thể thay đổi tùy vào hệ thống bạn chọn)
                DateTime startOfWeek = dateTime.AddDays(-diff).Date;  // Ngày đầu tuần
                DateTime endOfWeek = startOfWeek.AddDays(6);          // Ngày cuối tuần (Thứ bảy)

                // Lọc các đơn hàng trong tuần
                var weeklyRevenue = await _context.Orders
                    .Where(o => o.OrderStatus == "Đã giao"
                                && o.PaymentStatus == "Đã thanh toán"
                                && o.OrderDate >= startOfWeek
                                && o.OrderDate <= endOfWeek)
                    .SumAsync(o => o.Total);  // Tính tổng doanh thu trong tuần

                _response.IsSuccess = true;
                _response.Message = $"Tính tổng doanh thu tuần từ {startOfWeek.ToString("dd/MM/yyyy")} đến {endOfWeek.ToString("dd/MM/yyyy")}";
                _response.Result = weeklyRevenue;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
