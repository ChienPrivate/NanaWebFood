using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NanaFoodWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly ITokenProvider _tokenProvider;
        

        public AuthController(IAuthRepository authRepo, ITokenProvider tokenProvider)
        {
            _authRepo = authRepo;
            _tokenProvider = tokenProvider;
        }
        public IActionResult Login()
        {
            var message = Request.Query["message"];

            // Nếu có message và là "activation-success", đặt TempData
            if (!string.IsNullOrEmpty(message) && message == "activation-success")
            {
                TempData["success"] = "Kích hoạt tài khoản thành công!";
            }

            if (HttpContext.Session.GetString("Token") != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var response = await _authRepo.LoginAsync(login);

            // Gán trực tiếp nếu response.Message là chuỗi đơn giản
            string message = response.Message?.ToString() ?? "Có lỗi xảy ra";

            if (response != null && response.IsSuccess == true)
            {
                var checkEmailConfirmResponse = await _authRepo.CheckEmailConfirm();
                if (checkEmailConfirmResponse != null && checkEmailConfirmResponse.IsSuccess == true)
                {
                    var userReturn = JsonConvert.DeserializeObject<UserReturn>(response.Result.ToString());
                    HttpContext.Session.SetString("Token", userReturn.Token);
                    try
                    {
                        await SignInUser(userReturn); // phương thức dùng để đổi trạng thài người dùng sang IsAuthenticated
                        _tokenProvider.SetToken(userReturn.Token); // lưu token vào cookie
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = ex.Message.ToString();
                        return View();
                    }
                    
                    TempData["success"] = response.Message?.ToString();
                     var role = _tokenProvider.ReadToken("role",userReturn.Token);

                    if (role == "admin")
                    {
                        return RedirectToAction("Index", "DashBoard");
                    }

                    return RedirectToAction("Index", "Home");
                }
                TempData["response"] = JsonConvert.SerializeObject(response);
                return RedirectToAction(nameof(NotificationConfirmEmail));
            }

            TempData["error"] = message;
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString("Token", string.Empty);
            _tokenProvider.ClearToken();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult NotificationConfirmEmail()
        {
            if (TempData["response"] != null)
            {
                var res = JsonConvert.DeserializeObject<ResponseDto>(TempData["response"].ToString());
                return View(res);
            }
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

        public IActionResult AccessDenied()
        {
            return View();
        }


        private async Task SignInUser(UserReturn? model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,

                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == "nameid").Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == "given_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
