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
    [Route("Customers")]

    public class CustomersController : Controller
    {
        private readonly DataContext _context;
        private readonly CallApiCenter _callAPI;
        private readonly ConvertHelper _covertHelper;

        public CustomersController(DataContext context)
        {
            _context = context;
            _callAPI = new CallApiCenter();
            _covertHelper = new ConvertHelper();
        }

        // GET: Customers
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search = null)
        {
            List<Customers> lstCus = new List<Customers>();
            var req = new CustomerReq()
            {
                ModelRequest = new CustomerModelReq()
            };
            if (!string.IsNullOrEmpty(search))
            {
                var filter = new FilterModel()
                {
                    ColumnName = "CustomerName",
                    ValueFirst = search,
                    ValueSec = string.Empty,
                    Type = FilterType.Like,
                    DataType = DataType.Text
                };
                req.ListFllter.Add(filter);
            }
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Customers", "token ne");
            if (data.Status)
            {
                lstCus = _covertHelper.ConvertDynamicToList<Customers>(data.Data);
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(lstCus);
        }

        // GET: Customers/Details/5
        [HttpGet("Details/{id}")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new CustomerReq()
            {
                ModelRequest = new CustomerModelReq()
                {
                    CustomerId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Customers", "token ne");
            var cus = new Customers();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Customers>(data.Data, out cus);
            }

            return View(cus);
        }

        // GET: Customers/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,CustomerCd,Email,Address,PhoneNumber,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] CustomerModelReq customers)
        {
            var req = new CustomerReq()
            {
                ModelRequest = customers
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
            
        }

        [HttpGet("Edit/{id}")]
        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new CustomerReq()
            {
                ModelRequest = new CustomerModelReq()
                {
                    CustomerId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Customers", "token ne");
            var cus = new Customers();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Customers>(data.Data, out cus);
            }

            return View(cus);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerName,CustomerCd,Email,Address,PhoneNumber,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] CustomerModelReq customers)
        {
            customers.CustomerId = id;
            var req = new CustomerReq()
            {
                ModelRequest = customers
            };
            //var req = new RequestData();
            req.FunctionCode = "U";
            //req.ModelRequest = items;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Customers", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            
        }

        [HttpGet("Delete/{id}")]
        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new CustomerReq()
            {
                ModelRequest = new CustomerModelReq()
                {
                    CustomerId = (int)id
                },
                FunctionCode = "D"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Customers", "token ne");
            var cus = new Customers();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Customers>(data.Data, out cus);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Customers/Delete/5
        [HttpPost("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Customers", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var customers = await _context.Customers.FindAsync(id);
            //if (customers != null)
            //{
            //    _context.Customers.Remove(customers);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
