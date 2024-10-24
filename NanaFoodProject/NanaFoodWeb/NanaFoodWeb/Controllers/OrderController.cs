using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Utility;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderService;


        public OrderController(IOrderRepository orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {

            var provinceRequest = await _orderService.GetProvinceAsync();

            var provinceResponse = JsonConvert.DeserializeObject<GHNResponseDto<List<ProvinceDto>>>(provinceRequest.Result.ToString());

            if (provinceResponse.Code == 200)
            {
                var provinceList = provinceResponse.Data.ToList();

                var selectListProvinces = provinceList.Select(p => new SelectListItem
                {
                    Text = p.ProvinceName,   // Tên tỉnh làm Text
                    Value = p.ProvinceID.ToString()  // ID tỉnh làm Value
                }).ToList();

                ViewBag.ProvinceList = selectListProvinces;

                return View();
            }

            return View();

        }
        public async Task<IActionResult> OrderHistory()
        {
            return View();

        }
        public async Task<IActionResult> OrderHistoryDetail()
        {
            return View();

        }
        public async Task<JsonResult> GetDistricts(int provinceId)
        {
            // Gọi API để lấy danh sách quận/huyện dựa trên ProvinceID
            var districtRequest = await _orderService.GetDistrictAsync(provinceId);

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
            var wardRequest = await _orderService.GetWardAsync(districtId);

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
            var response = await _orderService.GetAvailableServiceAsync(fromDistrict, toDistrict);

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

            var response = await _orderService.CalculateShippingFees(requestDto);

            var shippingFeeResponse = JsonConvert.DeserializeObject<GHNResponseDto<ShippingFeeDto>>(response.Result.ToString());

            if (response.IsSuccess)
            {
                // Lấy được phí ship và trả về tổng phí
                var shippingFee = shippingFeeResponse.Data.Total;
                return Json(new { data = shippingFee });
            }

            return Json(new { message = "Lỗi không thể tính phí" });
        }
    }
}
