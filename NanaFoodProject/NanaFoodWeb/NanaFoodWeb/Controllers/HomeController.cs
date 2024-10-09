using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.Extensions;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using System.Diagnostics;

namespace NanaFoodWeb.Controllers
{
    [ExcludeAdmin]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenProvider _tokenProvider;
        public HomeController(ILogger<HomeController> logger, ITokenProvider tokenProvider)
        {
            _logger = logger;
            _tokenProvider = tokenProvider;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var token = _tokenProvider.GetToken();

            if ( token != null )
            {
                var role = _tokenProvider.ReadToken("role", token);
                if (role == "admin")
                {
                    return RedirectToAction("Index", "DashBoard");
                }
            }

            return View();
        }


        public IActionResult About() 
        {
            return View();
        }

        public IActionResult Discount()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
