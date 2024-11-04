using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.GHNDto;
using NanaFoodWeb.Models.Dto.ViewModels;
using NanaFoodWeb.Models.Momo;
using NanaFoodWeb.Models.VNPay;
using NanaFoodWeb.Utility;
using static NanaFoodWeb.Utility.StaticDetails;

namespace NanaFoodWeb.IRepository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IBaseService _baseService;
        public OrderRepository(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> AddOrderAsync(Order order)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Data = order,
                Url = StaticDetails.APIBase + "/api/Order/orders"
            });
        }

        public async Task<ResponseDto> CancelOrderAsync(int orderId, string message)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = StaticDetails.APIBase + $"/api/Order/CancelOrders/{orderId}/{message}",
            });
        }

        public async Task<ResponseDto> CalculateShippingFees(CalculateShippingFeeRequestDto requestDto)
        {

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = ShipppingFeeCaculateEndPoint,
                Data = requestDto
            }, false, "token");
        }

        public Task<ResponseDto> CODPayment(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> GetAvailableServiceAsync(int from_district, int todistrict)
        {
            var requestBody = new
            {
                shop_id = 5406185,
                from_district = from_district,
                to_district = todistrict
            };

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = AvailableServiceEndPoint,
                Data = requestBody
            }, false, "token");

        }

        public async Task<ResponseDto> GetDistrictAsync(int provinceId)
        {
            var requestBody = new
            {
                province_id = provinceId
            };

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = DistrictEndPoint,
                Data = requestBody
            }, false, "token");
        }

        public async Task<ResponseDto> GetProvinceAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = ProvinceEndPoint,
            }, false, "token");
        }

        public async Task<ResponseDto> GetWardAsync(int districtId)
        {
            var requestBody = new
            {
                district_id = districtId
            };

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = WardEndPoint,
                Data = requestBody
            }, false, "token");
        }

        public async Task<ResponseDto> CalculateShippingTime(CalculateShippingTimeRequestDto requestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                AccessToken = GHNApiKey,
                Url = ExpectedDeliveryDateEndPoint,
                Data = requestDto
            }, false, "token");
        }

        public async Task<ResponseDto> MomoPayment(int total)
        {
            string accessKey = "M8brj9K6E22vXoDB";
            string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            MomoRequest momoRequest = new MomoRequest();
            momoRequest.partnerCode = "MOMO5RGX20191128";
            momoRequest.partnerName = "Test Momo API Payment";
            momoRequest.storeId = "Momo Test Store";
            momoRequest.orderInfo = "Payment with momo";
            momoRequest.redirectUrl = "https://localhost:51326/Order/MomoReturn";
            momoRequest.ipnUrl = "https://localhost:51326";
            momoRequest.requestType = "captureWallet";
            momoRequest.amount = Math.Round((decimal)total).ToString();
            momoRequest.orderId = Guid.NewGuid().ToString();
            momoRequest.requestId = Guid.NewGuid().ToString();
            momoRequest.extraData = "";
            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + momoRequest.amount +
                "&extraData=" + momoRequest.extraData +
                "&ipnUrl=" + momoRequest.ipnUrl +
                "&orderId=" + momoRequest.orderId +
                "&orderInfo=" + momoRequest.orderInfo +
                "&partnerCode=" + momoRequest.partnerCode +
                "&redirectUrl=" + momoRequest.redirectUrl +
                "&requestId=" + momoRequest.requestId +
                "&requestType=" + momoRequest.requestType
                ;
            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            momoRequest.signature = crypto.signSHA256(rawHash, serectkey);

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = "https://test-payment.momo.vn/v2/gateway/api/create",
                Data = momoRequest,
            },false);

        }

        public string VNPayPayment(int total)
        {
            string vnp_Returnurl = "https://localhost:51326/Order/VNPReturn"; //URL nhan ket qua tra ve 
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = "2DQGKKDA"; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = "UMTCVNGYSCWSWVEJCADPUEECDUCVHVNU"; //Secret Key

            //Get payment input
            OrderInfo order = new OrderInfo();
            order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = (long)Math.Round((decimal)total); // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            order.CreatedDate = DateTime.Now;
            //Save order to db

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ":1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }

        public async Task<ResponseDto> GetOrderByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Order/orders/{id}"
            });
        }

        public async Task<ResponseDto> GetUserOrderIdAsync(string UserId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Order/orders/{UserId}"
            });
        }

        public async Task<ResponseDto> GetOrderDetailsAsync(int OrderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Order/ordersdetails/{OrderId}"
            });
        }

        public async Task<ResponseDto> CalculateProfitAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Order/profit"
            });
        }

        public async Task<ResponseDto> UpdateOrderStatus(int OrderId, string message)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = APIBase + $"/api/Order/profit/orders/{OrderId}&{message}"
            });
        }

        public async Task<ResponseDto> GetRebuyOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = APIBase + $"/api/Order/GetRebuyOrder/{orderId}"
            });
        }

        public async Task<ResponseDto> RebuyOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = APIBase + $"/api/Order/RebuyOrder/{orderId}"
            });
        }
    }
}
