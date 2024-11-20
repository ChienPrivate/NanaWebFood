using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Route("ManageOrder")]
    [Authorize(Roles = "admin,employee")]
    public class ManageOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;
        public ManageOrderController(IOrderRepository orderRepository,
            IReviewRepository reviewRepository)
        {
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<IActionResult> Index()
        {
            var repsonse = await _orderRepository.GetAllOrderAsync();

            var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(repsonse.Result.ToString());

            var confirmedYet = orderList.Where(o => o.OrderStatus == "Chờ xác nhận").ToList();
            var deliveringList = orderList.Where(o => o.OrderStatus == "Đang giao").ToList();
            var preparingList = orderList.Where(o => o.OrderStatus == "Đang chuẩn bị").ToList();
            var completedList = orderList.Where(o => o.OrderStatus == "Đã giao").ToList();
            var cancelOrder = orderList.Where(o => o.OrderStatus == "Đã huỷ").ToList();

            ViewBag.ConfirmedYet = confirmedYet;

            ViewBag.Delivering = deliveringList;

            ViewBag.Preparing = preparingList;

            ViewBag.Complete = completedList;

            ViewBag.CancelOrder = cancelOrder;

            return View();
        }

        [HttpGet("Details/{orderId}")]
        public async Task<IActionResult> Details(int orderId)
        {
            var response = await _orderRepository.GetOrderByIdAsync(orderId);
            var responseProductFromOrderDetails = await _reviewRepository.GetOrderDetailsFromOrder(orderId);

            if (responseProductFromOrderDetails.IsSuccess)
            {
                var listproductFromOrderDetails = JsonConvert.DeserializeObject<List<ReviewProductDto>>(responseProductFromOrderDetails.Result.ToString());
                ViewBag.ProductList = listproductFromOrderDetails;
            }

            OrderDto order = new OrderDto();

            if (response.IsSuccess)
            {
                order = JsonConvert.DeserializeObject<OrderDto>(response.Result.ToString());
            }
            else
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet("ModifyStatus/{orderId}/{message}")]
        public async Task<IActionResult> ModifyDelveringStatus(int orderId, string message)
        {
            var response = await _orderRepository.UpdateOrderStatus(orderId, message);
            if (response.IsSuccess)
            {
                var updatedResponse = await _orderRepository.GetAllOrderAsync();
                var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(updatedResponse.Result.ToString());

                /*var confirmedYet = orderList.Where(o => o.OrderStatus == "Chờ xác nhận").ToList();
                var deliveringList = orderList.Where(o => o.OrderStatus == "Đang giao").ToList();
                var preparingList = orderList.Where(o => o.OrderStatus == "Đang chuẩn bị").ToList();
                var completedList = orderList.Where(o => o.OrderStatus == "Đã giao").ToList();
                var cancelOrder = orderList.Where(o => o.OrderStatus == "Đã huỷ").ToList();

                // Làm mới ViewBag
                ViewBag.ConfirmedYet = confirmedYet;
                ViewBag.Delivering = deliveringList;
                ViewBag.Preparing = preparingList;
                ViewBag.Complete = completedList;
                ViewBag.CancelOrder = cancelOrder;*/

                TempData["success"] = "Cập nhật trạng thái thành công";
                return RedirectToAction("Index", "ManageOrder");
            }
            TempData["error"] = "Không thể cập nhật trạng thái đơn hàng hoặc đơn hàng không tồn tại";
            return RedirectToAction("Index", "ManageOrder");
        }

        [HttpGet("GetPreparingOrders")]
        public async Task<IActionResult> GetPreparingOrders()
        {
            // Lấy tất cả đơn hàng từ repository hoặc API
            var response = await _orderRepository.GetAllOrderAsync();
            if (response.IsSuccess)
            {
                // Chuyển dữ liệu từ API response thành danh sách OrderDto
                var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(response.Result.ToString());

                // Lọc danh sách đơn hàng có trạng thái "Đang chuẩn bị"
                var preparingOrders = orderList.Where(o => o.OrderStatus == "Đang chuẩn bị").ToList();

                // Trả về dữ liệu dưới dạng JSON
                return Json(preparingOrders);
            }

            // Nếu xảy ra lỗi, trả về lỗi 500
            return StatusCode(500, "Không thể lấy dữ liệu đơn hàng");
        }



        [HttpGet("GetConfirmedOrders")]
        public async Task<IActionResult> GetConfirmedOrders()
        {
            // Lấy tất cả đơn hàng từ repository hoặc API
            var response = await _orderRepository.GetAllOrderAsync();
            if (response.IsSuccess)
            {
                // Chuyển dữ liệu từ API response thành danh sách OrderDto
                var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(response.Result.ToString());

                // Lọc danh sách đơn hàng có trạng thái "Đang chuẩn bị"
                var confirmedYet = orderList.Where(o => o.OrderStatus == "Chờ xác nhận").ToList();

                // Trả về dữ liệu dưới dạng JSON
                return Json(confirmedYet);
            }

            // Nếu xảy ra lỗi, trả về lỗi 500
            return StatusCode(500, "Không thể lấy dữ liệu đơn hàng");
        }

        [HttpGet("GetDeliveringOrders")]
        public async Task<IActionResult> GetDeliveringOrders()
        {
            // Lấy tất cả đơn hàng từ repository hoặc API
            var response = await _orderRepository.GetAllOrderAsync();
            if (response.IsSuccess)
            {
                // Chuyển dữ liệu từ API response thành danh sách OrderDto
                var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(response.Result.ToString());

                // Lọc danh sách đơn hàng có trạng thái "Đang chuẩn bị"
                var delivering = orderList.Where(o => o.OrderStatus == "Đang giao").ToList();

                // Trả về dữ liệu dưới dạng JSON
                return Json(delivering);
            }

            // Nếu xảy ra lỗi, trả về lỗi 500
            return StatusCode(500, "Không thể lấy dữ liệu đơn hàng");
        }

        [HttpGet("GetCompleteOrders")]
        public async Task<IActionResult> GetCompleteOrders()
        {
            // Lấy tất cả đơn hàng từ repository hoặc API
            var response = await _orderRepository.GetAllOrderAsync();
            if (response.IsSuccess)
            {
                // Chuyển dữ liệu từ API response thành danh sách OrderDto
                var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(response.Result.ToString());

                // Lọc danh sách đơn hàng có trạng thái "Đang chuẩn bị"
                var completed = orderList.Where(o => o.OrderStatus == "Đã giao").ToList();

                // Trả về dữ liệu dưới dạng JSON
                return Json(completed);
            }

            // Nếu xảy ra lỗi, trả về lỗi 500
            return StatusCode(500, "Không thể lấy dữ liệu đơn hàng");
        }

        [HttpGet("GetCanceledOrders")]
        public async Task<IActionResult> GetCanceledOrders()
        {
            // Lấy tất cả đơn hàng từ repository hoặc API
            var response = await _orderRepository.GetAllOrderAsync();
            if (response.IsSuccess)
            {
                // Chuyển dữ liệu từ API response thành danh sách OrderDto
                var orderList = JsonConvert.DeserializeObject<List<OrderDto>>(response.Result.ToString());

                // Lọc danh sách đơn hàng có trạng thái "Đang chuẩn bị"
                var canceled = orderList.Where(o => o.OrderStatus == "Đã huỷ").ToList();


                // Trả về dữ liệu dưới dạng JSON
                return Json(canceled);
            }

            // Nếu xảy ra lỗi, trả về lỗi 500
            return StatusCode(500, "Không thể lấy dữ liệu đơn hàng");
        }


        [HttpGet("CancelOrder/{orderId}/{message}")]
        public async Task<IActionResult> CancelOrder(int orderId, string message)
        {
            var response = await _orderRepository.CancelOrderAsync(orderId, message);
            if (response.IsSuccess)
            {
                TempData["success"] = "Hủy đơn thành công";
                return RedirectToAction("Index", "ManageOrder");
            }
            TempData["error"] = "Bạn không thể Hủy đơn hàng này";
            return RedirectToAction("Index", "ManageOrder");
        }


    }
}
