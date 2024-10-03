using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.Controllers
{
    [Route("Users")]
    public class UsersController : Controller
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
