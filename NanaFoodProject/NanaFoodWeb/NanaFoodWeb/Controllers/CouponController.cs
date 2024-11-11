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
        [HttpPost("Create")]
        public async Task <IActionResult> Create(CouponDto couponDto)
        {
            if (!ModelState.IsValid)
            {
                return View(couponDto);
            }
            var coupon = new Coupon
            {
                CouponCode = couponDto.CouponCode,
                Discount = couponDto.Discount,
                Description = couponDto.Description,
                MinAmount = couponDto.MinAmount,
                CouponStartDate = couponDto.CouponStartDate,
                EndStart = couponDto.EndStart,
                MaxUsage = couponDto.MaxUsage,
                TimesUsed = couponDto.TimesUsed,
                Status = couponDto.Status
            };
            var response = await _couponRepo.Create(coupon);
            if (response != null && response.IsSuccess)
            {
                response.Result = coupon; 
                return RedirectToAction("Index");
            }
            return View(coupon);
        }
        [HttpGet("Edit/{code}")]
        public async Task<IActionResult>Edit(string code)
        {
            var response = await _couponRepo.GetById(code);
            if(response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(response.Result.ToString()); 
                return View(coupon);
            }
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            return RedirectToAction("Index");

        }
        [HttpGet("Details/{code}")]
        public async Task <IActionResult> Details(string code)
        {
            var response = await _couponRepo.GetById(code);
            if (response?.IsSuccess == true && response.Result != null)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(response.Result.ToString());
                return View(coupon);
            }
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            return RedirectToAction("Index");
        }
    }
}
