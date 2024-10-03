using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.CallAPICenter;
using StoreManagement.Data;
using StoreManagement.Model.Request;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    [Route("Categories")]

    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly ConvertHelper _covertHelper;
        private readonly CallApiCenter _callAPI;

        public CategoriesController(DataContext context)
        {
            _context = context;
            _covertHelper = new ConvertHelper();
            _callAPI = new CallApiCenter();
        }

        // GET: Categories
        [HttpGet("Index")]
        public async Task<IActionResult> Index( string search = null)
        {
            List<Categories> lstCate = new List<Categories>();
            var req = new CategoriesReq()
            {
                ModelRequest = new CategoriesModelReq()
            };
            if (!string.IsNullOrEmpty(search))
            {
                var filter = new FilterModel()
                {
                    ColumnName = "CategoryName",
                    ValueFirst = search,
                    ValueSec = string.Empty,
                    Type = FilterType.Like,
                    DataType = DataType.Text
                };
                req.ListFllter.Add(filter);
            }
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", "token ne");
            if (data.Status)
            {
                lstCate = _covertHelper.ConvertDynamicToList<Categories>(data.Data);
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(lstCate);
        }
        [HttpGet("Details/{id}")]
        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new CategoriesReq()
            {
                ModelRequest = new CategoriesModelReq()
                {
                    CategoryId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", "token ne");
            var item = new Categories();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Categories>(data.Data, out item);
            }
            
            return View(item);
        }

        // GET: Categories/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] CategoriesModelReq categories)
        {
            var req = new CategoriesReq()
            {
                ModelRequest = categories
            };
            //var req = new RequestData();
            req.FunctionCode = "C";
            //req.ModelRequest = items;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //categories.CreateBy = "Admin";
            //categories.UpdateBy = "Admin";
            //categories.CreateDate = DateTime.Now;
            //categories.UpdateDate = DateTime.Now;
            //_context.Add(categories);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            /*if (ModelState.IsValid)
            {
                _context.Add(categories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }*/
            //return View(categories);
        }

        // GET: Categories/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new CategoriesReq()
            {
                ModelRequest = new CategoriesModelReq()
                {
                    CategoryId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", "token ne");
            var item = new Categories();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Categories>(data.Data, out item);
            }

            return View(item);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] CategoriesModelReq categories)
        {
            categories.CategoryId = id;
            var req = new CategoriesReq()
            {
                ModelRequest = categories
            };
            //var req = new RequestData();
            req.FunctionCode = "U";
            //req.ModelRequest = items;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (id != categories.CategoryId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(categories);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!CategoriesExists(categories.CategoryId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(categories);
        }

        // GET: Categories/Delete/5
        [HttpGet("Delete/{id}")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

             var req = new CategoriesReq()
            {
                ModelRequest = new CategoriesModelReq() {
                    CategoryId = (int)id
                },
                FunctionCode = "D"
            };
            
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", "token ne");
            var item = new Items();
            if (data.Status)
            {
                 _covertHelper.ConvertDynamicToT<Items>(data.Data, out item);
            }

            return RedirectToAction(nameof(Index));

        }

        // POST: Categories/Delete/5
        [HttpPost("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Categories", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var categories = await _context.Categories.FindAsync(id);
            //if (categories != null)
            //{
            //    _context.Categories.Remove(categories);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool CategoriesExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
