using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NanaFoodWeb.ViewComponents
{
    public class CategoryCountViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryCountViewComponent(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _categoryRepo.CategoryCount();
            var categories = JsonConvert.DeserializeObject<List<CategoryCount>>(response.Result.ToString());
            return View(categories);
        }
    }
}
