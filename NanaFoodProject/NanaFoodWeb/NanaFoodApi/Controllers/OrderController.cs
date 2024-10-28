using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;

namespace NanaFoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepo _cartRepo;
        private readonly IMapper _mapper;
        public OrderController(IOrderRepository orderRepository,
            SignInManager<User> signInManager,
            ICartRepo cartRepo,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _orderRepository = orderRepository;
            _cartRepo = cartRepo;
            _mapper = mapper;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrderAync()
        {
            var ordersResponse = await _orderRepository.GetAllOrderAync();

            return Ok(ordersResponse);
        }

        [HttpPost("orders")]
        public async Task<IActionResult> AddOrderAsync([FromBody] OrderDto orderDto)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);

            var cart = await _cartRepo.GetCart(user);

            if(ModelState.IsValid)
            {
                List<CartResponseDto> cartItem = (List<CartResponseDto>)cart.Result;

                var order = _mapper.Map<Order>(orderDto);

                order.UserId = user.Id;

                var createdOrder = await _orderRepository.AddOrder(order);

                var orderDetails = cartItem.Select(item => new OrderDetails
                {
                    OrderId = createdOrder.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });

                _orderRepository.AddOrderDetails(orderDetails);

                var removeCartItem = await _cartRepo.RemoveAllCartItem(user.Id);

                return Ok(new ResponseDto
                {
                    Message = "Tạo đơn hàng thành công",
                    IsSuccess = true,
                    Result = createdOrder.OrderId
                });
            }

            return BadRequest(ModelState);
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
