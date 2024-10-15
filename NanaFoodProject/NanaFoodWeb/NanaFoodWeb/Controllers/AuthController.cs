using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
                var userReturn = JsonConvert.DeserializeObject<UserReturn>(response.Result.ToString());

                try
                {
                    await SignInUser(userReturn.Token); // phương thức dùng để đổi trạng thái người dùng sang IsAuthenticated
                    _tokenProvider.SetToken(userReturn.Token); // lưu token vào cookie
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message.ToString();
                    return View();
                }

                TempData["success"] = response.Message?.ToString();
                var role = _tokenProvider.ReadToken("role", userReturn.Token);

                if (role == "admin")
                {
                    return RedirectToAction("Index", "DashBoard");
                }

                var checkEmailConfirmResponse = await _authRepo.CheckEmailConfirm(_tokenProvider.ReadToken("email",userReturn.Token));
                TempData["response"] = JsonConvert.SerializeObject(response);
                if (checkEmailConfirmResponse != null && checkEmailConfirmResponse.IsSuccess)
                {
                    TempData["response"] = JsonConvert.SerializeObject(response);
                    return RedirectToAction("Index", "Home");
                }
                TempData["success"] = null;
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var response = await _authRepo.ForgotPassword(email);
            string message = response.Message?.ToString() ?? "Có lỗi xảy ra";
            if (response != null && response.IsSuccess == true)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }
                ModelState.AddModelError("Email", message);
                return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpPost]
        public IActionResult GitHubLogin()
        {
            return Redirect("https://localhost:7094/api/Auth/github");
        }

        [HttpPost]
        public IActionResult GoogleLogin()
        {
            return Redirect("https://localhost:7094/api/Auth/google");
        }

        [HttpPost]
        public IActionResult FacebookLogin()
        {
            return Redirect("https://localhost:7094/api/Auth/facebook");
        }

        public async Task<IActionResult> ExternalLoginCallback(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                // Decode the base64 data
                var jsonResponse = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data));

                if (jsonResponse != null)
                {

                    await SignInUser(jsonResponse);
                    _tokenProvider.SetToken(jsonResponse);
                    TempData["success"] = "Login Successful";
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["error"] = "Error logging in with Google.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        public async Task<IActionResult> GetInfo()
        {
            var response = await _authRepo.GetInfo();
            if(response != null && response.IsSuccess)
            {
                return View(JsonConvert.DeserializeObject<UserDto>(response.Result.ToString()));
            }
            return RedirectToAction("AccessDenied");
        }

        private async Task SignInUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,

                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == "nameid").Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.GivenName,
                jwt.Claims.FirstOrDefault(u => u.Type == "given_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == "name").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
