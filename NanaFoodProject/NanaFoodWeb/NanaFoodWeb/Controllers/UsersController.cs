using Microsoft.AspNetCore.Mvc;

namespace NanaFoodWeb.Controllers
{
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
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View(id);
        }
    }
}
