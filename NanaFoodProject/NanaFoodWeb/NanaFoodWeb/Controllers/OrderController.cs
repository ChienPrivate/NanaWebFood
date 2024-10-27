using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Momo;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NanaFoodWeb.Controllers
{
    [Authorize(Roles = "customer")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly ICartRepo _cartRepo;
        public OrderController(IOrderRepository orderService, ITokenProvider tokenProvider, ICartRepo cartRepo)
        {
            _orderRepository = orderService;
            _tokenProvider = tokenProvider;
            _cartRepo = cartRepo;
        }

        public async Task<IActionResult> Index()
        {
            var token = _tokenProvider.GetToken();
            if (token == null)
            {
                TempData["error"] = "Vui lòng đăng nhập trước";
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                var response = await _cartRepo.GetCart();
                if (response.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(response.Result.ToString());
                    var provinceRequest = await _orderRepository.GetProvinceAsync();

                    var provinceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<ProvinceDto>>>(provinceRequest.Result.ToString());

                    if (provinceResponse.Code == 200)
                    {
                        var provinceList = provinceResponse.Data.ToList();

                        var selectListProvinces = provinceList.Select(p => new SelectListItem
                        {
                            Text = p.ProvinceName,   // Tên tỉnh làm Text
                            Value = p.ProvinceID.ToString()  // ID tỉnh làm Value
                        }).ToList();
                        ViewBag.cartdetails = Data;
                        ViewBag.ProvinceList = selectListProvinces;
                        return View();
                    }
                }
                return View();
            }

        }




        [HttpPost]
        public async Task<IActionResult> Payment(Order order, int total)
        {
            order.Total = total;
            HttpContext.Session.Set<Order>("order", order);
            HttpContext.Session.Set<int>("total", total);
            if (ModelState.IsValid)
            {
                if (order.PaymentType == "COD")
                {
                    return await COD(order);
                }
                if (order.PaymentType == "MOMO")
                {
                    var momoRequest = await _orderRepository.MomoPayment(total);

                    if (momoRequest.IsSuccess)
                    {
                        var momoResponse = JsonConvert.DeserializeObject<MomoResponse>(momoRequest.Result.ToString());

                        return Redirect(momoResponse.payUrl);
                    }
                }
                if (order.PaymentType == "VNPAY")
                {

                    var redirectUrl = _orderRepository.VNPayPayment(total);

                    if (redirectUrl != null)
                    {
                        return Redirect(redirectUrl);
                    }

                }

                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> OrderHistory()
        {
            var token = _tokenProvider.GetToken();

            var userId = _tokenProvider.ReadToken("nameid", token);

            var response = await _orderRepository.GetUserOrderIdAsync(userId);

            var orders = JsonConvert.DeserializeObject<List<Order>>(response.Result.ToString());

            return View(orders);
        }


        public async Task<IActionResult> OrderHistoryDetail(int id)
        {
            var response = await _orderRepository.GetOrderByIdAsync(id);

            if (response.IsSuccess)
            {
                var order = JsonConvert.DeserializeObject<Order>(response.Result.ToString());

                return View(order);
            }

            return View(new Order());

        }
        public async Task<IActionResult> OrderTracking()
        {
            return View();

        }

        private async Task<IActionResult> COD(Order order)
        {
            var response = await _orderRepository.AddOrderAsync(order);

            if (response.IsSuccess)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("PaymentSuccess", "Order");
            }

            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("PaymentError", "Order");
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            var order = HttpContext.Session.Get<Order>("order");
            ViewBag.total = HttpContext.Session.Get<int>("total");


            return View(order);
        }

        [HttpGet]
        public IActionResult PaymentError()
        {
            var order = HttpContext.Session.Get<Order>("order");
            ViewBag.total = HttpContext.Session.Get<int>("total");

            return View(order);
        }



        #region Momo
        public async Task<IActionResult> MomoReturn()
        {
            var ResultCode = HttpContext.Request.Query["resultCode"];
            if (ResultCode == "0")
            {
                var order = HttpContext.Session.Get<Order>("order");
                order.PaymentStatus = "Đã thanh toán";
                return await COD(order);
            }
            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("PaymentError", "Order");
        }
        #endregion

        #region VNPay
        public async Task<IActionResult> VNPReturn()
        {
            var ResponseCode = HttpContext.Request.Query["vnp_ResponseCode"];
            if (ResponseCode == "00")
            {
                var order = HttpContext.Session.Get<Order>("order");
                order.PaymentStatus = "Đã thanh toán";
                return await COD(order);
            }
            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("PaymentFailure", "Order");
        }
        #endregion


        #region CaculatingShippingFee
        public async Task<JsonResult> GetDistricts(int provinceId)
        {
            // Gọi API để lấy danh sách quận/huyện dựa trên ProvinceID
            var districtRequest = await _orderRepository.GetDistrictAsync(provinceId);

            var districtResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<DistrictDto>>>(districtRequest.Result.ToString());

            if (districtResponse.Code == 200)
            {
                // Chuyển đổi danh sách quận/huyện thành SelectListItem
                var districtList = districtResponse.Data.Select(d => new SelectListItem
                {
                    Text = d.DistrictName, // Tên quận/huyện
                    Value = d.DistrictID.ToString() // ID quận/huyện
                }).ToList();

                // Trả về JSON
                return Json(districtList);
            }

            // Trường hợp không thành công, trả về JSON rỗng
            return Json(new List<SelectListItem>());
        }


        public async Task<JsonResult> GetWards(int districtId)
        {
            // Gọi API để lấy danh sách phường/xã dựa trên DistrictID
            var wardRequest = await _orderRepository.GetWardAsync(districtId);

            var wardResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<WardDto>>>(wardRequest.Result.ToString());

            if (wardResponse.Code == 200)
            {
                // Chuyển đổi danh sách phường/xã thành SelectListItem
                var wardList = wardResponse.Data.Select(w => new SelectListItem
                {
                    Text = w.WardName, // Tên phường/xã
                    Value = w.WardCode.ToString() // ID phường/xã
                }).ToList();

                // Trả về JSON
                return Json(wardList);
            }

            // Trường hợp không thành công, trả về JSON rỗng
            return Json(new List<SelectListItem>());
        }

        public async Task<JsonResult> GetAvailableService(int fromDistrict, int toDistrict)
        {
            fromDistrict = 1454; // id quận của trường nằm ở quận 12
            var response = await _orderRepository.GetAvailableServiceAsync(fromDistrict, toDistrict);

            var serviceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<AvailableServiceDto>>>(response.Result.ToString());

            if (serviceResponse.Code == 200 && serviceResponse.Data.Any())
            {
                // Lấy dịch vụ đầu tiên từ danh sách
                var firstService = serviceResponse.Data.FirstOrDefault();

                // Tạo danh sách với dịch vụ đầu tiên được chọn
                var serviceList = serviceResponse.Data.Select(w => new SelectListItem
                {
                    Text = w.ShortName,
                    Value = w.ServiceId.ToString(),
                    Selected = (firstService != null && w.ServiceId == firstService.ServiceId) // Chọn dịch vụ đầu tiên
                }).ToList();

                // Trả về JSON chứa danh sách dịch vụ
                return Json(serviceList);
            }

            // Trường hợp không thành công, trả về JSON rỗng
            return Json(new List<SelectListItem>());
        }

        [HttpPost]
        public async Task<JsonResult> CalculateShippingFee(int serviceId, int toDistrictId, string wardCode)
        {

            CalculateShippingFeeRequestDto requestDto = new CalculateShippingFeeRequestDto()
            {
                ServiceId = serviceId,
                ToDistrictId = toDistrictId,
                ToWardCode = wardCode,
                Weight = 8600
            };

            var response = await _orderRepository.CalculateShippingFees(requestDto);

            var shippingFeeResponse = JsonConvert.DeserializeObject<GHNResponseDto<ShippingFeeDto>>(response.Result.ToString());

            if (response.IsSuccess)
            {
                // Lấy được phí ship và trả về tổng phí
                var shippingFee = shippingFeeResponse.Data.Total;
                return Json(new { data = shippingFee });
            }

            return Json(new { message = "Lỗi không thể tính phí" });
        }

        #endregion
    }
}
