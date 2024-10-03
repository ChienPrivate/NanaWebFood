using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Model.Request;
using StoreManagement.CallAPICenter;
using StoreManagement.Data;
using StoreManagement.Model.Request;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    [Route("Users")]

    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly ConvertHelper _covertHelper;
        private readonly CallApiCenter _callAPI;

        public UsersController(DataContext context)
        {
            _context = context;
            _covertHelper = new ConvertHelper();
            _callAPI = new CallApiCenter();
        }

        // GET: Users
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search = null)
        {
            List<Users> lstUser = new List<Users>();
            var req = new UserReq()
            {
                ModelRequest = new UserModelReq()
            };
            if (!string.IsNullOrEmpty(search))
            {
                var filter = new FilterModel()
                {
                    ColumnName = "UserName",
                    ValueFirst = search,
                    ValueSec = string.Empty,
                    Type = FilterType.Like,
                    DataType = DataType.Text
                };
                req.ListFllter.Add(filter);
            }
            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", "token ne");
            if (data.Status)
            {
                lstUser = _covertHelper.ConvertDynamicToList<Users>(data.Data);
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(lstUser);
        }

        [HttpGet("Details/{id}")]
        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new UserReq()
            {
                ModelRequest = new UserModelReq()
                {
                    UserName = id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", "token ne");
            var user = new Users();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Users>(data.Data, out user);
            }

            return View(user);
        }

        // GET: Users/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Create([Bind("UserId,UserName,PassWord,Role,UserLevel,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] UserModelReq users)
        {
            var req = new UserReq()
            {
                ModelRequest = users
            };
            req.FunctionCode = "C";
            //req.ModelRequest = users;
            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            
        }

        [HttpGet("Edit/{id}")]
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new UserReq()
            {
                ModelRequest = new UserModelReq()
                {
                    UserId = (int)id
                },
                FunctionCode = "G"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", "token ne");
            var user = new Users();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Users>(data.Data, out user);
            }

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,PassWord,Role,UserLevel,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] UserModelReq users)
        {
            users.UserId = id;
            var req = new UserReq()
            {
                ModelRequest = users
            };
            req.FunctionCode = "U";
            //req.ModelRequest = users;
            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            
        }

        [HttpGet("Delete/{id}")]
        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = new UserReq()
            {
                ModelRequest = new UserModelReq()
                {
                    UserId = (int)id
                },
                FunctionCode = "D"
            };

            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", "token ne");
            var user = new Users();
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Users>(data.Data, out user);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/Delete/5
        [HttpPost("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"System/Users", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var users = await _context.Users.FindAsync(id);
            //if (users != null)
            //{
            //    _context.Users.Remove(users);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
