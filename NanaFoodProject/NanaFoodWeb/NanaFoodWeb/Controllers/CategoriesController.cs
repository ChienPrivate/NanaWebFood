using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
