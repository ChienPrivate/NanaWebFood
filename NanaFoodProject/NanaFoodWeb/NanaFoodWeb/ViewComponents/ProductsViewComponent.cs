using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;

namespace NanaFoodWeb.ViewComponents
{
    public class ProductsViewComponent : ViewComponent
    {
        private readonly IProductRepo _productRepository;

        public ProductsViewComponent(IProductRepo productService)
        {
            _productRepository = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId,int categoryId, int page = 1, int pageSize = 4)
        {
            var response = await _productRepository.GetByCategoryIdExcludeSameProduct(productId,categoryId,page,pageSize);
            var products = JsonConvert.DeserializeObject<ProductVM>(response.Result.ToString());
            ViewData["productId"] = productId;


            return View(products);
        }
    }
}
