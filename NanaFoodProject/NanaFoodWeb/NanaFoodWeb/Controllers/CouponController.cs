using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NanaFoodWeb.Controllers
{
    [Route("Coupon")]
    public class CouponController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet("Edit")]
        public IActionResult Edit()
        {
            return View();
        }
        [HttpGet("Details")]
        public IActionResult Details()
        {
            return View();
        }
    }
}
