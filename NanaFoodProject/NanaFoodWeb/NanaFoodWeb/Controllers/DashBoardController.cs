using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminstratorInformation()
        {
            return View();
        }
    }
}
