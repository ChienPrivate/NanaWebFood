﻿using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using NanaFoodWeb.CallAPICenter;
using NanaFoodWeb.Convert;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;
using NuGet.Common;

namespace NanaFoodWeb.Controllers
{
    [Route("Categories")]
    public class CategoriesController : Controller
    {
        private readonly CallApiCenter _callAPICenter;
        private readonly ConvertHelper _convertHelper;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHelperRepository _helperRepository;
        //private readonly ICategoryRepo _categoryRepo;
        public CategoriesController(
            ITokenProvider tokenProvider,
            IHelperRepository helperRepository) 
        {
            _callAPICenter = new CallApiCenter();
            _convertHelper= new ConvertHelper();
            _tokenProvider = tokenProvider;
            _helperRepository = helperRepository;
            //_categoryRepo = categoryRepo;
        }
        [HttpGet("Index")]
        public async Task<ActionResult> Index(string searchQuery, int? page=1)
        {
            var token = _tokenProvider.GetToken();
            string apiName = "Category/SearchName?page=" + page + "&pageSize=10&name=" + searchQuery;
            ResponseDto respone =await _callAPICenter.GetMethod<ResponseDto>(apiName, token);
            if (respone.IsSuccess)
            {
                ViewBag.CurrentPage = page;
                
                var resultData = JsonConvert.DeserializeObject<Result<List<CategoryDto>>>(respone.Result.ToString());
                ViewBag.TotalPages = resultData.TotalPages;
                return View(resultData.Data);
            }
            
            return View(new List<CategoryDto>());
        }
        [HttpGet("Create")]
        public ActionResult Create()
        {
            var categories = new CategoryDto();
            return View(categories);
        }
        // POST: Category/Create
        [HttpPost("Create")]
        public async Task<ActionResult> Create(CategoryDto category, IFormFile? UploadFile)
        {
            string imageUrl = string.Empty;
            
            if (ModelState.IsValid)
            {
                var token = _tokenProvider.GetToken();
                string apiName = "Category";
                if (UploadFile != null && UploadFile.Length > 0)
                {
                    // Gọi service để upload hình ảnh
                    var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);

                    imageUrl = uploadResponse.Result?.ToString() ?? "Tải ảnh lên thất bại";

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Gắn URL của hình ảnh vào model
                        category.CategoryImage = imageUrl;
                    }
                    else
                    {
                        category.CategoryImage = "Chưa có hình ảnh";
                    }

                }
                
                ResponseDto respone = await _callAPICenter.PostMethod<ResponseDto>(category,apiName, token);
                if (respone.IsSuccess)
                {
                    var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                    //return View(resultData.Data);
                    TempData["success"] = "Tạo danh mục thành công";
                    return RedirectToAction("Index");
                }
                
                
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryDto category,IFormFile? UploadFile)
        {
            string imageUrl = string.Empty;
            var token = _tokenProvider.GetToken();
            if (ModelState.IsValid)
            {
                if (UploadFile != null && UploadFile.Length > 0)
                {
                    // Gọi service để upload hình ảnh
                    var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);

                    imageUrl = uploadResponse.Result?.ToString() ?? "Tải ảnh lên thất bại";

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Gắn URL của hình ảnh vào model
                        category.CategoryImage = imageUrl;
                    }
                    else
                    {
                        category.CategoryImage = "Chưa có hình ảnh";
                    }

                }
                string apiName = "Category/"+ category.CategoryId;
                ResponseDto respone = await _callAPICenter.PutMethod<ResponseDto>(category, apiName, token);
                if (respone.IsSuccess)
                {
                    var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                    TempData["success"] = "cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                
            }
            return View(category);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var token = _tokenProvider.GetToken();
            string apiName = "Category/" + id;
            ResponseDto respone = await _callAPICenter.DeleteMethod<ResponseDto>(apiName, token);
            if (respone.IsSuccess)
            {
                TempData["success"] = "Xóa danh mục thành công";
                return RedirectToAction("Index");
            }
            else {
                
                string message = respone.Message;
                return NotFound(message);
                
            }

        }
        // POST: Category/Delete/5
/*        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }*/

        [HttpGet("Edit/{id}")]
        public async Task<ActionResult> Edit(int id)
       {
            var token = _tokenProvider.GetToken();
            string apiName = "Category/" + id;
            ResponseDto respone = await _callAPICenter.GetMethod<ResponseDto>(apiName, token);
            if (respone.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                return View(resultData.Data);
            }

            return View(new CategoryDto());
        }
        [HttpGet("Details")]
        public async Task<ActionResult> Details(int id)
        {
            var token = _tokenProvider.GetToken();
            string apiName = "Category/" + id;
            ResponseDto respone = await _callAPICenter.GetMethod<ResponseDto>(apiName, token);
            if (respone.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                return View(resultData.Data);
            }
            return View(new CategoryDto());
        }
    }
}
