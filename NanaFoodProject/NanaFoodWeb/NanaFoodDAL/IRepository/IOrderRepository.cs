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
        Task<ResponseDto> AddOrderAsync(OrderDto orderDto);
        Task<ResponseDto> AddOrderDetailAsync(IEnumerable<OrderDetailsDto> lisOrderdetailsDto);
        Task<ResponseDto> GetOrderByIdAsync(int id);
        Task<ResponseDto> GetUserOrderIdAsync(string UserId);
        Task<ResponseDto> GetOrderDetailsAsync(int OrderId);
        Task<ResponseDto> CalculateProfitAsync();
        Task<ResponseDto> UpdateOrderStatus(int OrderId, string message);
        Task<ResponseDto> CancelOrderAsync(int OrderId);
    }
}
