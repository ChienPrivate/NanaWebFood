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

            var deliveringList = orderList.Where(o => o.OrderStatus == "Đang giao").ToList();
            var preparingList = orderList.Where(o => o.OrderStatus == "Đang chuẩn bị").ToList();
            var completedList = orderList.Where(o => o.OrderStatus == "Đã giao").ToList();
            var cancelOrder = orderList.Where(o => o.OrderStatus == "Đã huỷ").ToList();

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
            var response = await _orderRepository.CancelOrderAsync(orderId, message);
            if (response.IsSuccess)
            {
                TempData["success"] = "Hủy đơn thành công";
                return RedirectToAction("Index", "ManageOrder");
            }
            TempData["error"] = "Bạn không thể Hủy đơn hàng này";
            return RedirectToAction("Index", "ManageOrder");
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
