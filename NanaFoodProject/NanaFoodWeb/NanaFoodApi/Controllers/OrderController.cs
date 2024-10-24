using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrderAync()
        {
            var ordersResponse = await _orderRepository.GetAllOrderAync();

            return Ok(ordersResponse);
        }

        [HttpPost("orders")]
        public async Task<IActionResult> AddOrderAsync(OrderDto orderDto)
        {
            var addOrderResponse = await _orderRepository.AddOrderAsync(orderDto);

            return Ok(addOrderResponse);
        }

        [HttpPost("orderdetails")]
        public async Task<IActionResult> AddOrderDetailAsync(IEnumerable<OrderDetailsDto> lisOrderdetailsDto)
        {
            var addOrderDetailresponse = await _orderRepository.AddOrderDetailAsync(lisOrderdetailsDto);

            return Ok(addOrderDetailresponse);
        }

        [HttpGet("orders/{id:int}")]
        public async Task<IActionResult> GetOrderByIdAsync([FromRoute] int id)
        {
            var getOrderByIdResponse = await _orderRepository.GetOrderByIdAsync(id);

            return Ok(getOrderByIdResponse);
        }

        [HttpGet("orders/{UserId}")]
        public async Task<IActionResult> GetUserOrderIdAsync([FromRoute] string UserId)
        {
            var getUserOrderIdResponse = await _orderRepository.GetUserOrderIdAsync(UserId);

            return Ok(getUserOrderIdResponse);
        }

        [HttpGet("ordersdetails/{OrderId}")]
        public async Task<IActionResult> GetOrderDetailsAsync([FromRoute] int OrderId)
        {
            var getOrderDetailsAsync = await _orderRepository.GetOrderDetailsAsync(OrderId);

            return Ok(getOrderDetailsAsync);
        }

        [HttpGet("profit")]
        public async Task<IActionResult> CalculateProfitAsync()
        {
            var calculateProfitAsyncResponse = await _orderRepository.CalculateProfitAsync();

            return Ok(calculateProfitAsyncResponse);
        }

        [HttpPut("orders/{OrderId}&{message}")]
        public async Task<IActionResult> UpdateOrderStatus(int OrderId, string message)
        {
            var updateOrderStatusResponse = await _orderRepository.UpdateOrderStatus(OrderId, message);

            return Ok(updateOrderStatusResponse);
        }

        [HttpPut("orders/{OrderId}")]
        public async Task<IActionResult> CancelOrderAsync(int OrderId)
        {
            var cancelOrderResponse = await _orderRepository.CancelOrderAsync(OrderId);

            return Ok(cancelOrderResponse);
        }
    }
}
