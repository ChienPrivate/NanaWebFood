using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Helper;
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
        private readonly EmailPoster _emailPoster;

        public OrderController(IOrderRepository orderRepository,
            SignInManager<User> signInManager,
            ICartRepo cartRepo,
            IMapper mapper,
            EmailPoster emailPoster)
        {
            _signInManager = signInManager;
            _orderRepository = orderRepository;
            _cartRepo = cartRepo;
            _mapper = mapper;
            _emailPoster = emailPoster;
        }

        /// <summary>
        /// Lấy danh sách tất cả các đơn hàng
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả các đơn hàng.
        /// </remarks>
        /// <returns>
        /// - 200 OK nếu lấy danh sách đơn hàng thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách đơn hàng.</response>
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrderAync()
        {
            var ordersResponse = await _orderRepository.GetAllOrderAync();

            return Ok(ordersResponse);
        }

        /// <summary>
        /// Thêm đơn hàng mới
        /// </summary>
        /// <remarks>
        /// API này cho phép tạo một đơn hàng mới cho người dùng hiện tại. Sau khi tạo đơn hàng thành công, các sản phẩm trong giỏ hàng sẽ bị xóa.
        /// 
        /// **Sample Request**:
        /// ```json
        /// {
        ///     "fullName": "Nguyễn Văn An",
        ///     "phoneNumber": "0123456789",
        ///     "address": "123 Đường ABC, TP. HCM",
        ///     "paymentType": "MOMO",
        ///     "paymentStatus": "Đã thanh toán",
        ///     "orderStatus": "Đang chuẩn bị",
        ///     "shipmentFee": 30000,
        ///     "note": "Giao hàng vào buổi sáng",
        ///     "userId": "user-id-example",
        ///     "total": 500000,
        ///     "orderDate": "2024-10-30T14:30:00",
        ///     "receiveDate": "2024-11-05T14:30:00"
        /// }
        /// ```
        /// </remarks>
        /// <param name="orderDto">Thông tin đơn hàng mới.</param>
        /// <returns>
        /// - 200 OK nếu tạo đơn hàng thành công.
        /// - 400 BadRequest nếu có lỗi trong dữ liệu đầu vào.
        /// </returns>
        /// <response code="200">Đơn hàng được tạo thành công.</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc xảy ra lỗi khi tạo đơn hàng.</response>
        [HttpPost("orders")]
        public async Task<IActionResult> AddOrderAsync([FromBody] OrderDto orderDto)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);

            var cart = await _cartRepo.GetCart(user);

            if (ModelState.IsValid)
            {
                List<CartResponseDto> cartItem = (List<CartResponseDto>)cart.Result;

                var order = _mapper.Map<Order>(orderDto);

                order.UserId = user.Id;
                /*order.Email = user.Email;*/

                var createdOrder = await _orderRepository.AddOrder(order);

                var orderDetails = cartItem.Select(item => new OrderDetails
                {
                    OrderId = createdOrder.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    ImageUrl = item.Image,
                    Quantity = item.Quantity,
                    Total = item.Total,
                });

                _orderRepository.AddOrderDetails(orderDetails);

                var productQuantity = await _orderRepository.UpdateProductQuantity(order.OrderId, 1);

                if (!productQuantity.IsSuccess)
                {
                    return Ok(productQuantity);
                }

                var removeCartItem = await _cartRepo.RemoveAllCartItem(user.Id);

                var emailContent = _emailPoster.OrderEmailTemplate(order, orderDetails.ToList());

                if (order.Email != string.Empty)
                {
                    _emailPoster.SendMail(order.Email, "Chi tiết đơn hàng", emailContent);
                }


                return Ok(new ResponseDto
                {
                    Message = "Tạo đơn hàng thành công",
                    IsSuccess = true,
                    Result = createdOrder.OrderId
                });
            }

            return BadRequest(ModelState);
        }


        /// <summary>
        /// Lấy thông tin chi tiết của đơn hàng theo ID
        /// </summary>
        /// <remarks>
        /// API này trả về thông tin chi tiết của một đơn hàng dựa trên ID của đơn hàng.
        /// </remarks>
        /// <param name="id">ID của đơn hàng.</param>
        /// <returns>
        /// - 200 OK nếu lấy thông tin đơn hàng thành công.
        /// - 404 NotFound nếu không tìm thấy đơn hàng với ID này.
        /// </returns>
        /// <response code="200">Trả về thông tin đơn hàng.</response>
        /// <response code="404">Không tìm thấy đơn hàng.</response>
        [HttpGet("orders/{id:int}")]
        public async Task<IActionResult> GetOrderByIdAsync([FromRoute] int id)
        {
            var getOrderByIdResponse = await _orderRepository.GetOrderByIdAsync(id);

            return Ok(getOrderByIdResponse);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của người dùng
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả các đơn hàng của một người dùng cụ thể dựa trên UserID.
        /// </remarks>
        /// <param name="UserId">ID của người dùng.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách đơn hàng thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách đơn hàng của người dùng.</response>
        [HttpGet("orders/{UserId}")]
        public async Task<IActionResult> GetUserOrderIdAsync([FromRoute] string UserId)
        {
            var getUserOrderIdResponse = await _orderRepository.GetUserOrderIdAsync(UserId);

            return Ok(getUserOrderIdResponse);
        }

        /// <summary>
        /// Lấy chi tiết các sản phẩm trong đơn hàng
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách chi tiết các sản phẩm trong một đơn hàng dựa trên OrderID.
        /// </remarks>
        /// <param name="OrderId">ID của đơn hàng.</param>
        /// <returns>
        /// - 200 OK nếu lấy chi tiết đơn hàng thành công.
        /// </returns>
        /// <response code="200">Trả về danh sách chi tiết sản phẩm trong đơn hàng.</response>
        [HttpGet("ordersdetails/{OrderId}")]
        public async Task<IActionResult> GetOrderDetailsAsync([FromRoute] int OrderId)
        {
            var getOrderDetailsAsync = await _orderRepository.GetOrderDetailsAsync(OrderId);

            return Ok(getOrderDetailsAsync);
        }

        /// <summary>
        /// Tính toán lợi nhuận từ các đơn hàng
        /// </summary>
        /// <remarks>
        /// API này tính toán tổng lợi nhuận từ các đơn hàng.
        /// </remarks>
        /// <returns>
        /// - 200 OK nếu tính toán lợi nhuận thành công.
        /// </returns>
        /// <response code="200">Trả về tổng lợi nhuận từ các đơn hàng.</response>
        [HttpGet("profit")]
        public async Task<IActionResult> CalculateProfitAsync()
        {
            var calculateProfitAsyncResponse = await _orderRepository.CalculateProfitAsync();

            return Ok(calculateProfitAsyncResponse);
        }

        /// <summary>
        /// Cập nhật trạng thái của đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật trạng thái của một đơn hàng dựa trên OrderID và thông báo trạng thái.
        /// </remarks>
        /// <param name="OrderId">ID của đơn hàng.</param>
        /// <param name="message">Thông báo trạng thái mới của đơn hàng.</param>
        /// <returns>
        /// - 200 OK nếu cập nhật trạng thái đơn hàng thành công.
        /// </returns>
        /// <response code="200">Trạng thái đơn hàng được cập nhật thành công.</response>
        [HttpPut("orders/{OrderId}&{message}")]
        public async Task<IActionResult> UpdateOrderStatus(int OrderId, string message)
        {
            var updateOrderStatusResponse = await _orderRepository.UpdateOrderStatus(OrderId, message);

            return Ok(updateOrderStatusResponse);
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép hủy một đơn hàng dựa trên OrderID.
        /// </remarks>
        /// <param name="OrderId">ID của đơn hàng cần hủy.</param>
        /// <returns>
        /// - 200 OK nếu hủy đơn hàng thành công.
        /// </returns>
        /// <response code="200">Đơn hàng đã được hủy thành công.</response>
        [HttpPut("CancelOrders/{OrderId}/{message}")]
        public async Task<IActionResult> CancelOrderAsync(int OrderId, string message)
        {
            // thực hiện thay đổi và kiểm tra về số lượng

            var user = await _signInManager.UserManager.GetUserAsync(User);

            var productQuantity = await _orderRepository.UpdateProductQuantity(OrderId, -1);

            if (!productQuantity.IsSuccess)
            {
                return Ok(productQuantity);
            }
            
            // thực hiện hủy đơn

            

            var cancelOrderResponse = await _orderRepository.CancelOrderAsync(OrderId, message);


            // gửi mail về cho người dùng 

            var orderDetails = await _orderRepository.GetOrderDetailsAsync(OrderId);

            var listOd = (List<OrderDetailsDto>)orderDetails.Result;

            var listOdmap = _mapper.Map<List<OrderDetails>>(listOd);

            var orderResponse = await _orderRepository.GetOrderByIdAsync(OrderId);

            var od = (OrderDto)orderResponse.Result;

            var order = _mapper.Map<Order>(od);

            var emailContent = _emailPoster.OrderEmailTemplate(order, listOdmap);

            if (order.Email != string.Empty)
            {
                _emailPoster.SendMail(order.Email, "Chi tiết hủy đơn", emailContent);
            }

            return Ok(cancelOrderResponse);
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép hủy một đơn hàng dựa trên OrderID.
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần hủy.</param>
        /// <returns>
        /// - 200 OK nếu lấy danh sách các món đã mua thành công.
        /// </returns>
        /// <response code="200">Đơn hàng đã được hủy thành công.</response>
        [HttpGet("GetRebuyOrder/{orderId}")]
        public async Task<IActionResult> GetRebuyOrder(int orderId)
        {
            var response = await _orderRepository.GetRebuyOrder(orderId);

            return Ok(response);
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép hủy một đơn hàng dựa trên OrderID.
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần hủy.</param>
        /// <returns>
        /// - 200 OK mua lại các đơn hàng thành công.
        /// </returns>
        /// <response code="200">Đơn hàng đã được mua lại thành công.</response>
        [HttpPost("RebuyOrder/{orderId}")]
        public async Task<IActionResult> RebuyOrder(int orderId)
        {
            var response = await _orderRepository.RebuyOrder(orderId);

            return Ok(response);
        }

        /// <summary>
        /// Lưu mã giảm giá vào đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép hủy một đơn hàng dựa trên OrderID.
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần hủy.</param>
        /// <param name="couponCode">ID của đơn hàng cần hủy.</param>
        /// <returns>
        /// - 200 OK áp dụng mã giảm giá cho đơn hàng thành công.
        /// </returns>
        /// <response code="200">áp dụng mã giảm giá cho đơn hàng thành công.</response>
        [HttpPut("ApplyCoupon/{orderId}/{couponCode}")]
        public async Task<IActionResult> ApplyCoupon(int orderId, string couponCode)
        {
            var response = await _orderRepository.ApplyCoupon(orderId, couponCode);

            return Ok(response);
        }


    }
}
