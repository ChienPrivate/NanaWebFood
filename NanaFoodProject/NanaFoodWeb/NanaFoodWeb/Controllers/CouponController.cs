using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto.ViewModels;

namespace NanaFoodWeb.Controllers
{
    [Route("Coupon")]
    public class CouponController : Controller
    {
        private readonly ICouponRepo _couponRepo;
        public CouponController(ICouponRepo couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _couponRepo.GetAll();
            if (response.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<List<Coupon>>(response.Result.ToString());
                ViewBag.lazyLoadData = resultData;
                return View();
            }
            
            return View(new List<CouponVM>());

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
