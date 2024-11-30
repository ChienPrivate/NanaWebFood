using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace NanaFoodWeb.Controllers
{
    [Authorize(Roles = "admin,employee")]
    public class DashBoardController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly IHelperRepository _helperRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IDashBoardRepository _boardRepository;
        public DashBoardController(IAuthRepository authRepository, IHelperRepository helperRepository, ITokenProvider tokenProvider, IDashBoardRepository boardRepository)
        {
            _authRepo = authRepository;
            _helperRepository = helperRepository;
            _tokenProvider = tokenProvider;
            _boardRepository = boardRepository;
        }
        public async Task<IActionResult> Index()
        {
            var token = _tokenProvider.GetToken();
            var role =  _tokenProvider.ReadToken("role", token);

            var profitInDayRes = await _boardRepository.GetProfitInDay();

            ViewBag.ProfitInDay = profitInDayRes.Result ?? 0;

            var profitInWeekRes = await _boardRepository.GetProfitInWeek();

            ViewBag.ProfitInWeek = profitInWeekRes.Result ?? 0;

            var profitInMonthRes = await _boardRepository.GetProfitByMonthAsync(DateTime.Now.Month);

            ViewBag.ProfitInMonth = profitInMonthRes.Result ?? 0;

            var profitInYearRes = await _boardRepository.GetProfitByYearAsync(DateTime.Now.Year);

            ViewBag.ProfitInYear = profitInYearRes.Result ?? 0;


            /*var deliveringOrderRes = await _boardRepository.GetDeliveringOrderAsync();

            if (deliveringOrderRes.IsSuccess)
            {
                var deliveringOrders = JsonConvert.DeserializeObject<List<Order>>(deliveringOrderRes.Result.ToString());
                ViewBag.Delivering = deliveringOrders.Count;
            }

            var completeOrderRes = await _boardRepository.GetCompleteOrderInMonthAsync(DateTime.Now.Month);

            if (completeOrderRes.IsSuccess)
            {
                var completeOrders = JsonConvert.DeserializeObject<List<Order>>(completeOrderRes.Result.ToString());
                ViewBag.Complete = completeOrders.Count;
            }*/

            var getprofitEachMonthRes = await _boardRepository.GetProfitEachMonth(DateTime.Now.Year);

            if (getprofitEachMonthRes.IsSuccess)
            {
                var getprofitEachMonth = JsonConvert.DeserializeObject<List<LineChartDto>>(getprofitEachMonthRes.Result.ToString());
                double maxRevenue = getprofitEachMonth.Max(data => data.Revenue);
                double roundedMaxRevenue = Math.Ceiling(maxRevenue / 1000000) * 1000000;

                // Đặt khoảng cách Interval cho trục Y
                double interval = roundedMaxRevenue / 4;


                ViewBag.YAxisMaximum = roundedMaxRevenue;
                ViewBag.YAxisInterval = interval;
                ViewBag.LineChartData = getprofitEachMonth;
            }

            if (role == "employee")
            {
                return RedirectToAction("Index","ManageOrder");
            }
            return View();
        }

        public async Task<IActionResult> AdministratorInformation()
        {
            var response = await _authRepo.GetInfo();
            if (response != null && response.IsSuccess)
            {
                var viewmodel = new ChangePassAndUserDto()
                {
                    UserDto = JsonConvert.DeserializeObject<UserDto>(response.Result.ToString()),
                    changepass = null
                };
                return View(viewmodel);
            }
            return RedirectToAction("AccessDenied");
        }

        [HttpPost]
        public async Task<IActionResult> AdministratorInformation(ChangePassAndUserDto viewmodel, IFormFile? UploadFile)
        {
            string imageUrl = string.Empty;
            if (ModelState.IsValid)
            {
                if (UploadFile != null && UploadFile.Length > 0)
                {
                    // Gọi service để upload hình ảnh
                    var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);

                    imageUrl = uploadResponse.Result?.ToString() ?? "Tải ảnh lên thất bại";

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Gắn URL của hình ảnh vào model
                        viewmodel.UserDto.AvatarUrl = imageUrl;
                    }
                    else
                    {
                        viewmodel.UserDto.AvatarUrl = "https://placehold.co/300x300";
                    }
                }

                var respone = await _authRepo.UpdateInfo(viewmodel.UserDto);
                if (respone.IsSuccess)
                {
                    TempData["success"] = respone.Message;
                    viewmodel.UserDto = JsonConvert.DeserializeObject<UserDto>(respone.Result.ToString());
                    return View(viewmodel);
                }
            }

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var ChangePass = new ChangePasswordDto()
            {
                OldPassword = OldPassword,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };
            var responsedto = await _authRepo.ChangePasswordAsync(ChangePass);
            if (responsedto.IsSuccess)
            {
                return Json(new { success = true, message = "Cập nhật mật khẩu thành công" });
            }

            return StatusCode(400, new { success = false, message = responsedto.Message ?? "Cập nhật mật khẩu thất bại" });
        }
    }
}
