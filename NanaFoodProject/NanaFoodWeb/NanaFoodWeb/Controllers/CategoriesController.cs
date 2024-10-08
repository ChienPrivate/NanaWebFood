﻿using Azure;
using Microsoft.AspNetCore.Mvc;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.Model;
using NanaFoodWeb.CallAPICenter;
using NanaFoodWeb.Convert;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Route("Categories")]
    public class CategoriesController : Controller
    {
        private readonly CallApiCenter _callAPICenter;
        private readonly ConvertHelper _convertHelper;
        //private readonly ICategoryRepo _categoryRepo;
        public CategoriesController() 
        {
            _callAPICenter = new CallApiCenter();
            _convertHelper= new ConvertHelper();
            //_categoryRepo = categoryRepo;
        }
        [HttpGet("Index")]
        public async Task<ActionResult> Index(string searchQuery, int? page=1)
        {
            string apiName = "Category/SearchName?page=" + page + "&pageSize=10&name=" + searchQuery;
            ResponseDto respone =await _callAPICenter.GetMethod<ResponseDto>(apiName, "");
            if (respone.IsSuccess)
            {
                ViewBag.CurrentPage = page;
                
                var resultData = JsonConvert.DeserializeObject<Result<List<CategoryDto>>>(respone.Result.ToString());
                ViewBag.TotalPages = resultData.TotalPages;
                TempData["success"] = "Data loaded successfully!";
                return View(resultData.Data);
            }
            
            return View(new List<CategoryDto>());
        }
        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }
        // POST: Category/Create
        [HttpPost("Create")]
        public async Task<ActionResult> Create(CategoryDto category)
        {
            if (ModelState.IsValid)
            {
                string apiName = "Category";
                ResponseDto respone = await _callAPICenter.PostMethod<ResponseDto>(category,apiName, "");
                if (respone.IsSuccess)
                {
                    var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                    //return View(resultData.Data);
                    return RedirectToAction("Index");
                }
                //var message = Request.Query["message"];
                //if (!string.IsNullOrEmpty(message) && message == "activation-success")
                //{
                //    TempData["success"] = "Thêm danh mục thành công!";
                //}
                //else
                //{
                //   TempData["erorr"] = "Thêm không thành công";
                //}
                TempData["success"] = "Created successfully!";
                
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryDto category)
        {
            if (ModelState.IsValid)
            {
                string apiName = "Category/"+ category.CategoryId;
                ResponseDto respone = await _callAPICenter.PutMethod<ResponseDto>(category, apiName, "");
                if (respone.IsSuccess)
                {
                    var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                    return View(resultData.Data);
                }
                //_categoryRepo.Update(category);
                TempData["success"] = "Updated successfully!";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            string apiName = "Category/" + id;
            ResponseDto respone = await _callAPICenter.DeleteMethod<ResponseDto>(apiName, "");
            if (respone.IsSuccess)
            {
                TempData["success"] = "Deleted successfully!";
                return RedirectToAction("Index");
            }
            else {
                
                string message = respone.Message;
                return NotFound(message);
                
            }
            /*var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }*/

        }
        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*var category = _categoryRepo.GetById(id);
            if (category != null)
            {
                _categoryRepo.Delete(id);
                TempData["Success"] = "Xóa danh mục thành công!";
            }*/
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            string apiName = "Category/" + id;
            ResponseDto respone = await _callAPICenter.GetMethod<ResponseDto>(apiName, "");
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
            string apiName = "Category/" + id;
            ResponseDto respone = await _callAPICenter.GetMethod<ResponseDto>(apiName, "");
            if (respone.IsSuccess)
            {
                var resultData = JsonConvert.DeserializeObject<Result<CategoryDto>>(respone.Result.ToString());
                return View(resultData.Data);
            }
            return View(new CategoryDto());
        }
    }
}
