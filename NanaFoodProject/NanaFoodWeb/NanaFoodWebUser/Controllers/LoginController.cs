using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.CallAPICenter;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using StoreManagement.Utils;
using System.Net.Http;

namespace StoreManagement.Controllers
{
    [Route("Login")]

    public class LoginController:Controller
    {
        private readonly CallApiCenter _callAPI;
        private readonly ConvertHelper _covertHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _callAPI = new CallApiCenter();
            _httpClientFactory = httpClientFactory;
            _covertHelper = new ConvertHelper();
            _httpClient = new HttpClient();
        }
        [HttpGet("Index")]
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpGet("Login")]

        public async Task<ActionResult> Login(string userName, string passWord)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return View("Vui lòng nhập Username!");
            }
            if (string.IsNullOrEmpty(passWord))
            {
                return View("Vui lòng nhập mật khẩu!");

            }
            Helper.BaseModel.LoginModel modelReq = new Helper.BaseModel.LoginModel()
            {
                Username= userName,
                Password= passWord
            };
            ResponeModel data = await _callAPI.PostMethod(modelReq, @"Auth/Login", "token ne");
            if (!data.Status)
            {
                return View(data.Message);
            }
            else
            {
                //string customerCode = "";
                //_covertHelper.TryParseDynamicToString(data.Data, out customerCode);
                var logRes = new LoginResponeModel();
                _covertHelper.ConvertDynamicToT<LoginResponeModel>(data.Data,out logRes);
                HttpContext.Session.SetString("CustomerCode", logRes.User.CustomerCd);
                HttpContext.Session.SetString("UserName", logRes.User.UserName);
                HttpContext.Session.SetString("Role", logRes.User.Role);
                HttpContext.Session.SetString("Token", logRes.Token);
                ViewBag.UserName = logRes.User.UserName;
                ViewBag.Role = logRes.User.Role;
                ViewBag.CustCode = logRes.User.CustomerCd;
                Ultils.Token = logRes.Token;
                return RedirectToAction("Index", "Home");
            }
            

        }
        [HttpPost("Register")]

        public async Task<ActionResult> Register(string userName, string passWord, string email, string usrtel,string fullname,string address, string phoneNumber, bool sex)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return View("Vui lòng nhập Username!");
            }
            if (string.IsNullOrEmpty(passWord))
            {
                return View("Vui lòng nhập mật khẩu!");

            };
            RegisterReq modelReq = new RegisterReq()
            {
                ModelRequest = new RegisterModelReq()
                {
                    UserName = userName,
                    PassWord = passWord,
                    Email = email,
                    PhoneNumber = usrtel,
                    FullName = fullname,
                    Address = address,
                    Sex = sex
                }
                
            };
            ResponeModel data = await _callAPI.PostMethod(modelReq, @"System/Register", "token ne");
            if (!data.Status)
            {
                return View(data.Message);
            }
            return RedirectToAction("Index", "Login");


        }
        [HttpGet("LoginGg")]

        public async Task<ActionResult> LoginGg() 
        {

            //ResponeModel data = await _callAPI.GetMethod(@"Auth/login-google", "token ne");
            //return RedirectToAction("Index", "Home");
            try
            {
                // Create HttpClient instance
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _httpClient.GetAsync("https://demostore.cc/your-endpoint");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                // Base URL of your MVC application where the LoginGoogle endpoint is hosted
                //var baseUrl = "https://your-mvc-app-base-url.com/";
                // Send GET request to the LoginGoogle endpoint
                var responses = await httpClient.GetAsync(@"https://demostore.cc/api/Auth/login-google");

                // Check if the request was successful (status code 200-299)
                if (responses.IsSuccessStatusCode)
                {
                    // Redirect to home index upon successful login
                    return Redirect("~/Home/Index");
                }
                else
                {
                    // Handle unsuccessful response (e.g., return an error response)
                    return StatusCode((int)responses.StatusCode);
                }
            }
            //catch (Exception ex)
            //{
            //    // Log or handle the exception
            //    return StatusCode(500, "An error occurred");
            //}
            catch (HttpRequestException ex)
            {
                // Log the exception and handle it accordingly
                Console.WriteLine($"Request error: {ex.Message}");
            }
            return View();
        }
    }
}

