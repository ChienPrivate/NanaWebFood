using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NanaFoodWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register() 
        {
            return View();
        }

        
        public IActionResult Notification()
        {
            if (TempData["response"] != null)
            {
                var res = JsonConvert.DeserializeObject<ResponseDto>(TempData["response"].ToString());
                ViewData["notify"] = "Đăng ký tài khoản thành công";
                return View(res);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var response = await _authRepo.RegisterAsync(registerDto);

            // Gán trực tiếp nếu response.Message là chuỗi đơn giản
            string message = response.Message?.ToString() ?? "Có lỗi xảy ra";

            if (response != null && response.IsSuccess == true)
            {
                TempData["success"] = message;
                TempData["response"] = JsonConvert.SerializeObject(response);
                return RedirectToAction(nameof(Notification));
            }

            TempData["error"] = message;
            return View();
        }

        [HttpPost]
        public IActionResult GitHubLogin()
        {
            return Redirect("https://localhost:7094/github");
        }
    }
}
