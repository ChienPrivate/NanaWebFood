using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.CallAPICenter;
using StoreManagement.Data;
using StoreManagement.Models;
using StoreManagement.Models.Request;

namespace StoreManagement.Controllers
{
    [Route("Items")]

    public class ItemsController : Controller
    {
        private readonly DataContext _context;
        private readonly ConvertHelper _covertHelper;
        private readonly CallApiCenter _callAPI;

        public ItemsController(DataContext context)
        {
            _context = context;
            _callAPI = new CallApiCenter();
            _covertHelper = new ConvertHelper();
        }

        // GET: Items
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search =null)
        {
            //return View(await _context.Items.ToListAsync());
            List<Items> lstItem = new List<Items>();
            var req = new ItemRequest()
            {
                ModelRequest = new ItemMasterReq()
            };
            if (!string.IsNullOrEmpty(search))
            {
                var filter = new FilterModel()
                {
                    ColumnName = "ItemName",
                    ValueFirst = search,
                    ValueSec = string.Empty,
                    Type = FilterType.Like,
                    DataType = DataType.Text
                };
                req.ListFllter.Add(filter);
            }
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", "token ne");
            if (data.Status)
            {
                lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            string cuscode = HttpContext.Session.GetString("CustomerCode");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ItemListPartial", lstItem);
            }
            //ViewData["CustCode"] = cuscode;
            ViewBag.CustCode = cuscode;
            return View(lstItem);
        }


        // GET: Items/Create
        [HttpGet("Create")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,Description,Price,CategoryId,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] ItemMasterReq items)
        {
            //items.CreateBy = "Admin";
            //items.UpdateBy = "Admin";
            //items.CreateDate = DateTime.Now;
            //items.UpdateDate = DateTime.Now;
            //_context.Add(items);
            //await _context.SaveChangesAsync();
            var req = new ItemRequest()
            {
                ModelRequest = items
            };
            //var req = new RequestData();
            req.FunctionCode = "C";
            //req.ModelRequest = items;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (modelstate.isvalid)
            //{
            //    _context.add(items);
            //    await _context.savechangesasync();
            //    return redirecttoaction(nameof(index));
            //}
            //return view(items);
        }
        // GET: Items/Details/5
        [HttpGet("Details/{id}")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new ItemRequest()
            {
                ModelRequest = new ItemMasterReq()
                {
                    ItemId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", "token ne");
            var item = new Items();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Items>(data.Data, out item);
            }
            //var items = await _context.Items.FindAsync(id);
            //if (items == null)
            //{
            //    return NotFound();
            //}
            return View(item);
        }
        // GET: Items/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var req = new ItemRequest()
            {
                ModelRequest = new ItemMasterReq()
                {
                    ItemId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", "token ne");
            var item = new Items();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Items>(data.Data, out item);
            }
            //var items = await _context.Items.FindAsync(id);
            //if (items == null)
            //{
            //    return NotFound();
            //}
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemName,Description,Price,CategoryId,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] ItemMasterReq items)
        {
            items.ItemId =id;
            var req = new ItemRequest()
            {
                ModelRequest = items
            };
            //var req = new RequestData();
            req.FunctionCode = "U";
            //req.ModelRequest = items;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (id != items.ItemId)
            //{
            //    return NotFound();
            //}
            //else
            //{
            //    try
            //    {
            //        _context.Update(items);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ItemsExists(items.ItemId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //}

            //return RedirectToAction(nameof(Index));

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(items);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ItemsExists(items.ItemId))
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
            //return View(items);
        }

        // GET: Items/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var req = new ItemRequest()
            {
                ModelRequest = new ItemMasterReq() {
                    ItemId = (int)id
                },
                FunctionCode = "D"
            };
            
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", "token ne");
            var item = new Items();
            if (data.Status)
            {
                 _covertHelper.ConvertDynamicToT<Items>(data.Data, out item);
            }
            
            return RedirectToAction("Index","Items");
            //var items = await _context.Items
            //    .FirstOrDefaultAsync(m => m.ItemId == id);
            //if (items == null)
            //{
            //    return NotFound();
            //}

            //return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var items = await _context.Items.FindAsync(id);
            //if (items != null)
            //{
            //    _context.Items.Remove(items);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool ItemsExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
