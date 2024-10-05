using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
