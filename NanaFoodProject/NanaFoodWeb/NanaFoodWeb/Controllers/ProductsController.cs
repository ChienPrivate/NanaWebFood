using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
