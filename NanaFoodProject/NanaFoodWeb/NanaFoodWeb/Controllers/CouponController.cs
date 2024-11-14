using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto.ViewModels;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Mvc.Diagnostics;

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
            var result = await _couponRepo.GetById(code); 
            if (result != null && result.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(result.Result.ToString());
                return View(coupon);
            }
            else
            {
                return RedirectToAction("Index", new { error = "Không tìm thấy sản phẩm để chỉnh sửa." });
            }
           
        }
        [HttpPost("Edit/{code}")]
        public async Task<IActionResult>Edit(CouponDto coupon, string code)
        {
            var result = await  _couponRepo.Update(coupon);
            if(result != null && result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View(coupon);
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

        [HttpGet("delete/{code}")]
        public async Task<IActionResult> Delete(string code)
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

        [HttpPost("DeleteConfirm")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var response = await _couponRepo.DeleteById(id);
            if (response.IsSuccess)
            {
                TempData["success"] = "Xóa danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                string message = response.Message;
                return NotFound(message);
            }
        }

        [HttpPost("deDelete/{id}")]
        public async Task<IActionResult> deDelete(string id)
        {
            var response = await _couponRepo.ModifyStatus(id);
            if (response.IsSuccess)
            {
                TempData["success"] = "Xoá thành công";
                return RedirectToAction("Index");
            }
            else
            {
                string message = response.Message;
                return NotFound(message);
            }
        }
    }
}
