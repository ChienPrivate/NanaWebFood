using NanaFoodDAL.Dto;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface IOrderRepository
    {
        Task<ResponseDto> GetAllOrderAync();
        Task<Order> AddOrder(Order order);
        void AddOrderDetails(IEnumerable<OrderDetails> lisOrderdetails);
        Task<ResponseDto> GetOrderByIdAsync(int id);
        Task<ResponseDto> GetUserOrderIdAsync(string UserId);
        Task<ResponseDto> GetOrderDetailsAsync(int OrderId);
        Task<ResponseDto> CalculateProfitAsync();
        Task<ResponseDto> UpdateOrderStatus(int OrderId, string message);
        Task<ResponseDto> CancelOrderAsync(int OrderId, string message);
        Task<ResponseDto> GetRebuyOrder(int orderId);
        Task<ResponseDto> RebuyOrder(int orderId);
        Task<ResponseDto> UpdateProductQuantity(int orderId, int state);
        Task<ResponseDto> ApplyCoupon(int orderId, string couponCode);
        Task<int> GetCancelOrderInWeek(string userId);
    }
}
