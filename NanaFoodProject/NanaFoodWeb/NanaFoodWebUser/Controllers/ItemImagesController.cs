using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.CallAPICenter;
using StoreManagement.Data;
using StoreManagement.Models;
using StoreManagement.Models.Request;

namespace StoreManagement.Controllers
{
    [Route("ItemImage")]
    public class ItemImagesController : Controller
    {
        private readonly DataContext _context;
        private readonly CallApiCenter _callAPI;
        private readonly ConvertHelper _covertHelper;

        public ItemImagesController(DataContext context)
        {
            _context = context;
            _callAPI = new CallApiCenter();
            _covertHelper = new ConvertHelper();
        }

        // GET: ItemImages
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search = null)
        {
            List<ItemImage> lstImg = new List<ItemImage>();
            var req = new ItemImageReq()
            {
                ModelRequest = new ItemImageModelReq()
            };
            if (!string.IsNullOrEmpty(search))
            {
                var filter = new FilterModel()
                {
                    ColumnName = "ImageURL",
                    ValueFirst = search,
                    ValueSec = string.Empty,
                    Type = FilterType.Like,
                    DataType = DataType.Text
                };
                req.ListFllter.Add(filter);
            }
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", "token ne");
            if (data.Status)
            {
                lstImg = _covertHelper.ConvertDynamicToList<ItemImage>(data.Data);
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(lstImg);
        }

        [HttpGet("Details/{id}")]
        // GET: ItemImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new ItemImageReq()
            {
                ModelRequest = new ItemImageModelReq()
                {
                    ImageId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", "token ne");
            var img = new ItemImage();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<ItemImage>(data.Data, out img);
            }

            return View(img);
        }

        // GET: ItemImages/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Create([Bind("ImageId,ImageURL,ImageBackground,ItemId,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] ItemImageModelReq itemImage)
        {
            var req = new ItemImageReq()
            {
                ModelRequest = itemImage
            };
            req.FunctionCode = "C";
            //req.ModelRequest = itemImage;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (ModelState.IsValid)
            //{
            //    _context.Add(itemImage);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(itemImage);
        }

        [HttpGet("Edit/{id}")]
        // GET: ItemImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new ItemImageReq()
            {
                ModelRequest = new ItemImageModelReq()
                {
                    ImageId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", "token ne");
            var img = new ItemImage();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<ItemImage>(data.Data, out img);
            }

            return View(img);
        }

        // POST: ItemImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]

        public async Task<IActionResult> Edit(int id, [Bind("ImageId,ImageURL,ImageBackground,ItemId,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] ItemImageModelReq itemImage)
        {
            itemImage.ImageId = id;
            var req = new ItemImageReq()
            {
                ModelRequest = itemImage
            };
            req.FunctionCode = "U";
            //req.ModelRequest = itemImage;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
           
        }

        [HttpGet("Delete/{id}")]
        // GET: ItemImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new ItemImageReq()
            {
                ModelRequest = new ItemImageModelReq()
                {
                    ImageId = (int)id
                },
                FunctionCode = "D"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", "token ne");
            var img = new ItemImage();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<ItemImage>(data.Data, out img);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ItemImages/Delete/5
        [HttpPost("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemImage", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var itemImage = await _context.ItemImages.FindAsync(id);
            //if (itemImage != null)
            //{
            //    _context.ItemImages.Remove(itemImage);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool ItemImageExists(int id)
        {
            return _context.ItemImages.Any(e => e.ImageId == id);
        }
    }
}
