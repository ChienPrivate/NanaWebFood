using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NanaFoodWeb.Extensions;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.GHNDto;
using NanaFoodWeb.Models.Momo;
using Newtonsoft.Json;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Http.Headers;
using System.Text;

namespace NanaFoodWeb.Controllers
{
    [Authorize(Roles = "customer")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICartRepo _cartRepo;
        private readonly IAuthRepository _authRepo;
        private readonly ICouponRepo _couponRepo;

        public OrderController(IOrderRepository orderService,
            ITokenProvider tokenProvider,
            ICartRepo cartRepo,
            IReviewRepository reviewRepository,
            IAuthRepository authRepo,
            ICouponRepo couponRepo)
        {
            _orderRepository = orderService;
            _tokenProvider = tokenProvider;
            _cartRepo = cartRepo;
            _reviewRepository = reviewRepository;
            _authRepo = authRepo;
            _couponRepo = couponRepo;
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
                var userInfoResponse = await _authRepo.GetInfo();
                var response = await _cartRepo.GetCart();
                if (response.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(response.Result.ToString());
                    
                    if (userInfoResponse.IsSuccess)
                    {
                        var userInfo = JsonConvert.DeserializeObject<UserDto>(userInfoResponse.Result.ToString());
                        ViewBag.UserInfo = userInfo;
                    }

                    var provinceRequest = await _orderRepository.GetProvinceAsync();

                    var provinceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<ProvinceDto>>>(provinceRequest.Result.ToString());

                    if (provinceResponse.Code == 200)
                    {
                        var provinceList = provinceResponse.Data.ToList();

                        var selectListProvinces = provinceList.Where(p => p.ProvinceID == 202).Select(p => new SelectListItem
                        {
                            Text = p.ProvinceName,   // Tên tỉnh làm Text
                            Value = p.ProvinceID.ToString(),  // ID tỉnh làm Value
                        }).ToList();
                        ViewBag.cartdetails = Data;
                        ViewBag.ProvinceList = selectListProvinces;
                        return View();
                    }
                }
                return View();
            }
        }
        public async Task<IActionResult> Payment()
        {
            var token = _tokenProvider.GetToken();
            if (token == null)
            {
                TempData["error"] = "Vui lòng đăng nhập trước";
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                var userInfoResponse = await _authRepo.GetInfo();

                var discountListRes = await _couponRepo.GetAvailableCoupon();

                if (discountListRes.IsSuccess)
                {
                    var coupons = JsonConvert.DeserializeObject<List<CouponDto>>(discountListRes.Result.ToString());

                    ViewBag.couponlist = coupons;
                }

                var response = await _cartRepo.GetCart();
                if (response.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(response.Result.ToString());

                    if (userInfoResponse.IsSuccess)
                    {
                        var userInfo = JsonConvert.DeserializeObject<UserDto>(userInfoResponse.Result.ToString());
                        ViewBag.UserInfo = userInfo;
                    }

                    var provinceRequest = await _orderRepository.GetProvinceAsync();

                    var provinceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<ProvinceDto>>>(provinceRequest.Result.ToString());

                    if (provinceResponse.Code == 200)
                    {
                        var provinceList = provinceResponse.Data.ToList();

                        var selectListProvinces = provinceList.Where(p => p.ProvinceID == 202).Select(p => new SelectListItem
                        {
                            Text = p.ProvinceName,   // Tên tỉnh làm Text
                            Value = p.ProvinceID.ToString(),  // ID tỉnh làm Value
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
            else
            {
                var userInfoResponse = await _authRepo.GetInfo();
                var response = await _cartRepo.GetCart();
                if (response.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(response.Result.ToString());

                    if (userInfoResponse.IsSuccess)
                    {
                        var userInfo = JsonConvert.DeserializeObject<UserDto>(userInfoResponse.Result.ToString());
                        ViewBag.UserInfo = userInfo;
                    }

                    var provinceRequest = await _orderRepository.GetProvinceAsync();

                    var provinceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<ProvinceDto>>>(provinceRequest.Result.ToString());

                    if (provinceResponse.Code == 200)
                    {
                        var provinceList = provinceResponse.Data.ToList();

                        var selectListProvinces = provinceList.Where(p => p.ProvinceID == 202).Select(p => new SelectListItem
                        {
                            Text = p.ProvinceName,   // Tên tỉnh làm Text
                            Value = p.ProvinceID.ToString(),  // ID tỉnh làm Value
                        }).ToList();
                        ViewBag.cartdetails = Data;
                        ViewBag.ProvinceList = selectListProvinces;
                        return View();
                    }
                }
                return View();
            }
        }

        public async Task<IActionResult> OrderHistory()
        {
            var token = _tokenProvider.GetToken();

            var userId = _tokenProvider.ReadToken("nameid", token);

            var response = await _orderRepository.GetUserOrderIdAsync(userId);

            var orders = JsonConvert.DeserializeObject<List<Order>>(response.Result.ToString());

            return View(orders.OrderByDescending(o => o.OrderDate));
        }


        public async Task<IActionResult> OrderHistoryDetail(int id)
        {
            var response = await _orderRepository.GetOrderByIdAsync(id);

            var rebuyOrderResponse = await _orderRepository.GetRebuyOrder(id);

            ViewBag.RebuyOrder = JsonConvert.DeserializeObject<List<RebuyOrderDto>>(rebuyOrderResponse.Result.ToString());

            var responseProductFromOrderDetails = await _reviewRepository.GetOrderDetailsFromOrder(id);

            if (response.IsSuccess)
            {
                var order = JsonConvert.DeserializeObject<Order>(response.Result.ToString());
                if (responseProductFromOrderDetails.IsSuccess)
                {
                    var listproductFromOrderDetails = JsonConvert.DeserializeObject<List<ReviewProductDto>>(responseProductFromOrderDetails.Result.ToString());
                    ViewBag.ReviewList = listproductFromOrderDetails;
                }
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

            int orderId = int.Parse(response.Result.ToString()); 

            if (!response.IsSuccess)
            {
                TempData["error"] = response.Message;
                var userInfoResponse = await _authRepo.GetInfo();
                var responseFromcart = await _cartRepo.GetCart();
                if (responseFromcart.Result != null)
                {
                    var Data = JsonConvert.DeserializeObject<List<CartResponseDto>>(responseFromcart.Result.ToString());

                    if (userInfoResponse.IsSuccess)
                    {
                        var userInfo = JsonConvert.DeserializeObject<UserDto>(userInfoResponse.Result.ToString());
                        ViewBag.UserInfo = userInfo;
                    }

                    var provinceRequest = await _orderRepository.GetProvinceAsync();

                    var provinceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<ProvinceDto>>>(provinceRequest.Result.ToString());

                    if (provinceResponse.Code == 200)
                    {
                        var provinceList = provinceResponse.Data.ToList();

                        var selectListProvinces = provinceList.Where(p => p.ProvinceID == 202).Select(p => new SelectListItem
                        {
                            Text = p.ProvinceName,   // Tên tỉnh làm Text
                            Value = p.ProvinceID.ToString(),  // ID tỉnh làm Value
                        }).ToList();
                        ViewBag.cartdetails = Data;
                        ViewBag.ProvinceList = selectListProvinces;
                        return RedirectToAction("Payment","Order");
                    }
                }
                return RedirectToAction("Payment", "Order");
            }

            if (order.CouponCode != null || !string.IsNullOrEmpty(order.CouponCode))
            {
                await _orderRepository.ApplyCoupon(orderId, order.CouponCode);
            }

            if (response.IsSuccess)
            {

                /*var fileBytes = System.IO.File.ReadAllBytes(filePath);

                RedirectToAction("PaymentSuccess", "Order");

                return File(fileBytes, "application/pdf", fileName);*/

                TempData["FirstTime"] = "true";

                TempData["OrderId"] = orderId;

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

            var invoiceFilePath = TempData["InvoiceFilePath"] as string;

            if (!string.IsNullOrEmpty(invoiceFilePath))
            {
                ViewBag.InvoiceFilePath = invoiceFilePath;
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult PaymentError()
        {
            var order = HttpContext.Session.Get<Order>("order");
            ViewBag.total = HttpContext.Session.Get<int>("total");

            return View(order);
        }

        public async Task<IActionResult> CancelOrder(int orderId, string message)
        {
            var response = await _orderRepository.CancelOrderAsync(orderId, message);
            if (response.IsSuccess)
            {
                TempData["success"] = "Hủy đơn thành công";
                return RedirectToAction("OrderHistory","Order");
            }

            TempData["error"] = "Bạn không thể Hủy đơn hàng này";
            return RedirectToAction("OrderHistory", "Order");

        }

        public async Task<IActionResult> ReviewOrder(int orderId)
        {
            var response = await _reviewRepository.GetOrderDetailsFromOrder(orderId);

            if (response.IsSuccess)
            {
                var listproductNeedToBeReview = JsonConvert.DeserializeObject<List<ReviewProductDto>>(response.Result.ToString());
                ViewBag.ReviewList = listproductNeedToBeReview;
                return View();
            }

            TempData["error"] = "Đơn hàng không tồn tại";
            return RedirectToAction("OrderHistory", "Order");
        }

        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] Review review)
        {
            var token = _tokenProvider.GetToken();

            var userId = _tokenProvider.ReadToken("nameid", token);

            review.UserId = userId;

            if (ModelState.IsValid)
            {
                var response = await _reviewRepository.PostReviewAsync(review);

                if (response.IsSuccess)
                {
                    /*var listproductNeedToBeReview = JsonConvert.DeserializeObject<List<ReviewProductDto>>(response.Result.ToString());*/
                    ViewBag.ReviewList = response.Result;
                    return RedirectToAction("ReviewOrder", "Order", new { orderId = review.OrderId });
                }
            }

            TempData["error"] = "xảy ra lỗi trong quá trình gửi đánh giá";
            return RedirectToAction("ReviewOrder", "Order", new { orderId = review.OrderId });
        }

        [HttpPost]
        public async Task<JsonResult> RebuyOrder(int orderId)
        {
            var response = await _orderRepository.RebuyOrder(orderId);
            
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ApplyCoupon(string code)
        {
            var couponResponse = await _couponRepo.GetById(code);

            if (couponResponse.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<Coupon>(couponResponse.Result.ToString());

                return Json(coupon);
            }

            return Json(new Coupon());
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
            return RedirectToAction("PaymentError", "Order");
        }
        #endregion

        #region GHN Service
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

        [HttpPost]
        public async Task<JsonResult> CalculateShippingTime(int toDistrict, string toWardCode, int serviceId)
        {
            CalculateShippingTimeRequestDto requestDto = new CalculateShippingTimeRequestDto()
            {
                FromDistrictId = 1454,
                FromWardCode = "21204",
                ToDistrictId = toDistrict,
                ToWardCode = toWardCode,
                ServiceId = serviceId,
            };
            var response = await _orderRepository.CalculateShippingTime(requestDto);

            var serviceResponse = JsonConvert.DeserializeObject<GHNResponseDto<ExpetedShippingTimeDto>>(response.Result.ToString());


            if (serviceResponse.Code == 200)
            {
                // Lấy đối tượng đầu tiên từ danh sách dữ liệu
                var expectedShippingTime = serviceResponse.Data;

                var expectedShippingTimeGG = CalculateAdjustedDeliveryTime(toDistrict);

                expectedShippingTime.LeadTimeUnix = ((DateTimeOffset)expectedShippingTimeGG).ToUnixTimeSeconds();

                // Trả về đối tượng `ExpetedShippingTimeDto` dưới dạng JSON
                return Json(expectedShippingTime);
            }

            // Trường hợp không thành công, trả về JSON rỗng hoặc đối tượng null
            return Json(null);
        }

        private DateTime CalculateAdjustedDeliveryTime(int toDistrictId)
        {
            // Biến lưu số phút sẽ cộng thêm, dựa trên quận
            double additionalMinutes;

            // Xét quận đích và đặt số phút bổ sung dựa trên khoảng cách từ Quận 12
            switch (toDistrictId)
            {
                case 1442: // Quận 1
                    additionalMinutes = 41;
                    break;
                case 1443: // Quận 2
                    additionalMinutes = 46;
                    break;
                case 1444: // Quận 3
                    additionalMinutes = 33;
                    break;
                case 1446: // Quận 4
                    additionalMinutes = 47;
                    break;
                case 1447: // Quận 5
                    additionalMinutes = 38;
                    break;
                case 1448: // Quận 6
                    additionalMinutes = 35;
                    break;
                case 1449: // Quận 7
                    additionalMinutes = 56;
                    break;
                case 1450: // Quận 8
                    additionalMinutes = 42;
                    break;
                case 1451: // Quận 9
                    additionalMinutes = 55;
                    break;
                case 1452: // Quận 10
                    additionalMinutes = 32;
                    break;
                case 1453: // Quận 11
                    additionalMinutes = 33;
                    break;
                case 1454: // Quận 12
                    additionalMinutes = 15;
                    break;
                case 3695: // Thành phố Thủ Đức
                    additionalMinutes = 30;
                    break;
                case 2090: // Huyện Cần Giờ
                    additionalMinutes = 129; // 2 giờ 9 phút = 129 phút
                    break;
                case 1534: // Huyện Nhà Bè
                    additionalMinutes = 65; // 1 giờ 5 phút = 65 phút
                    break;
                case 1533: // Huyện Bình Chánh
                    additionalMinutes = 35;
                    break;
                case 1462: // Quận Bình Thạnh
                    additionalMinutes = 29;
                    break;
                case 1461: // Quận Gò Vấp
                    additionalMinutes = 17;
                    break;
                case 1460: // Huyện Củ Chi
                    additionalMinutes = 43;
                    break;
                case 1459: // Huyện Hóc Môn
                    additionalMinutes = 16;
                    break;
                case 1458: // Quận Bình Tân
                    additionalMinutes = 23;
                    break;
                case 1457: // Quận Phú Nhuận
                    additionalMinutes = 27;
                    break;
                case 1456: // Quận Tân Phú
                    additionalMinutes = 21;
                    break;
                case 1455: // Quận Tân Bình
                    additionalMinutes = 22;
                    break;
                default:
                    additionalMinutes = 60; // Giá trị mặc định là 1 giờ nếu không có thông tin
                    break;
            }

            // Tính toán thời gian giao hàng cuối cùng sau khi cộng thêm phút
            return DateTime.Now.AddMinutes(additionalMinutes);
        }

        #endregion

        #region PDF Generate
        public async Task<byte[]> CreateOrderInvoice(int orderId)
        {
            var orderRes = await _orderRepository.GetOrderByIdAsync(orderId);

            var itemsRes = await _reviewRepository.GetOrderDetailsFromOrder(orderId);

            double totalOfFood = 0;

            Order order = new Order();

            List<ReviewProductDto> items = new List<ReviewProductDto>();

            if (orderRes.IsSuccess)
            {
                order = JsonConvert.DeserializeObject<Order>(orderRes.Result.ToString());
            }

            if (itemsRes.IsSuccess)
            {
                items = JsonConvert.DeserializeObject<List<ReviewProductDto>>(itemsRes.Result.ToString());

                foreach(var item in items as List<ReviewProductDto> ?? new List<ReviewProductDto>())
                {
                    totalOfFood += item.Total;
                }
            }

            

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    /*page.Header()
                    .Text("NanaFoodStore")
                    .SemiBold().FontSize(30).FontColor(Colors.Orange.Medium);*/


                    page.Header()
                    .Row(row =>
                    {

                        row.RelativeItem().AlignLeft().AlignMiddle().Text("NanaFoodStore").SemiBold().FontSize(30).FontColor(Colors.Orange.Medium);
                        row.RelativeItem().AlignRight().AlignMiddle().Text(DateTime.Now.ToString("dd/MM/yyyy HH:MM:ss tt")).FontSize(15).FontColor(Colors.Grey.Medium);
                    });

                    page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(10);

                        x.Item().Text("Thông tin người mua").FontSize(18).Bold().FontColor(Colors.Orange.Medium);

                        // Thông tin khách hàng
                        x.Item().Text($"Tên người mua: {order.FullName}").FontSize(15);
                        x.Item().Text($"Địa chỉ giao hàng: {order.Address}").FontSize(15);
                        x.Item().Text($"Số điện thoại: {order.PhoneNumber}").FontSize(15);
                        x.Item().Text($"Email: {order.Email}").FontSize(15);

                        // Thông tin đơn hàng dưới dạng bảng
                        x.Item().Text("Thông tin đơn hàng")
                        .Bold().FontSize(18).FontColor(Colors.Orange.Medium);

                        x.Item().Table(table =>
                        {
                            // Định nghĩa các cột của bảng
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Cột Tên sản phẩm
                                columns.RelativeColumn(); // Cột Giá
                                columns.RelativeColumn(); // Cột Số lượng
                                columns.RelativeColumn(); // Cột Tổng tiền
                            });

                            // Header của bảng
                            table.Header(header =>
                            {
                                var color = Colors.Orange.Lighten2;

                                header.Cell().Background(color).Text("Tên sản phẩm").FontSize(15).SemiBold().AlignCenter();
                                header.Cell().Background(color).Text("Giá").FontSize(15).SemiBold().AlignCenter();
                                header.Cell().Background(color).Text("Số lượng").FontSize(15).SemiBold().AlignCenter();
                                header.Cell().Background(color).Text("Tổng tiền").FontSize(15).SemiBold().AlignCenter();
                            });

                            // Dữ liệu bảng

                            // Thêm dữ liệu vào bảng với kiểu "striped"
                            for (int i = 0; i < items.Count; i++)
                            {
                                var item = items[i];

                                // Tạo hiệu ứng striped
                                var backgroundColor = i % 2 == 0 ? Colors.BlueGrey.Lighten4 : Colors.White;

                                table.Cell().Background(backgroundColor).Text(item.ProductName).FontSize(15).AlignCenter();
                                table.Cell().Background(backgroundColor).Text(item.Price.ToString("#,##") + " VNĐ").FontSize(15).AlignCenter();
                                table.Cell().Background(backgroundColor).Text(item.Quantity.ToString()).FontSize(15).AlignCenter();
                                table.Cell().Background(backgroundColor).Text(item.Total.ToString("#,##") + " VNĐ").FontSize(15).AlignCenter();
                            }
                        });

                        if (!string.IsNullOrEmpty(order.Note))
                        {
                            x.Item().Text($"Ghi chú: {order.Note}");
                        }

                        x.Item().Text("Thông tin thanh toán")
                        .Bold().FontSize(18).FontColor(Colors.Orange.Medium);

                        x.Item().Text($"Phương thức thanh toán: {order.PaymentType}").FontSize(15);
                        /*x.Item().Text($"Trạng thái thanh toán: {order.PaymentStatus}").FontSize(15);*/

                        if (!string.IsNullOrEmpty(order.CouponCode))
                        {
                            x.Item().Text($"Mã giảm giá: {order.CouponCode}").FontSize(15);
                            x.Item().Text($"Mã giảm giá: {order.Discount?.ToString("#,##")} VNĐ").FontSize(15);
                        }

                        x.Item().Text($"Tổng tiền các món: {totalOfFood.ToString("#,##")}").FontSize(15);
                        x.Item().Text($"Phí vận chuyển: {order.ShipmentFee.ToString("#,##")}").FontSize(15);
                        // Tổng tiền giả lập
                        x.Item().Text($"Tổng cộng: {order.Total.ToString("#,##")} VND").FontSize(15);

                    });

                    page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                    });
                });

            });

            var pdf = document.GeneratePdf();

            return pdf;

        }

        public async Task<IActionResult> DownloadInvoice(int orderId)
        {
            try
            {
                var pdf = await CreateOrderInvoice(orderId);

                var fileName = $"OrderInvoice_{orderId}.pdf";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                System.IO.File.WriteAllBytes(filePath, pdf);

                /*var fileBytes = System.IO.File.ReadAllBytes(filePath);*/

                var base64Pdf = System.Convert.ToBase64String(pdf);

                return Json(new { Success = true, FileBytes = base64Pdf, FileName = fileName });

            }
            catch (Exception ex)
            {
                TempData["error"] = "Không thể xuất pdf cho hóa đơn này";
                return Json(new { Success = false });
            }
        }
        #endregion
    }
}
