using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StoreManagement.CallAPICenter;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using System.Diagnostics;

namespace StoreManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CallApiCenter _callAPI;
        private readonly ConvertHelper _covertHelper;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _callAPI = new CallApiCenter();
            _covertHelper= new ConvertHelper();
        }

        public async Task<IActionResult> Index(string search=null)
        {
            List<Items> lstItem = new List<Items>();
            var req = new ItemRequest() {
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
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ItemListPartial", lstItem);
            }
            //ViewData["CustCode"] = cuscode;
            ViewBag.CustCode = cuscode;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            return View(lstItem);
        }
        [HttpGet("Details")]
        public async Task<IActionResult> Details(int id)
        {
            var itemRes = new Items();  
            var lstImage = new List<ItemImage>();
            var req = new ItemClassGetByIDReq();
            req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/ItemId", "token ne");
            if(data.Status)
            {
                _covertHelper.ConvertDynamicToT<Items>(data.Data, out itemRes);
                //lstImage = _covertHelper.ConvertDynamicToList<ItemImage>(data.Data.ListImage);
            }
            ViewData["Itm"] = itemRes;
            ViewData["ListImg"] = itemRes.ItemImages;
            return View();
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int id,decimal price,int quantity)
        {
            string customerCode = string.Empty;
            if (HttpContext.Session.GetString("CustomerCode") != null)
            {
                ViewBag.CustomerCode = HttpContext.Session.GetString("CustomerCode");
                customerCode = HttpContext.Session.GetString("CustomerCode");
            }
            var modelReq = new AddToCartReq();
            modelReq.ItemId = id;
            modelReq.Quantity = quantity;
            modelReq.Price = price;
            modelReq.CustomerCd = customerCode;
            //var req = new RequestData() {
            //    ModelRequest = JsonConvert.SerializeObject(modelReq)
            //};
            //req.ModelRequest= modelReq;
            ResponeModel data = await _callAPI.PostMethod(modelReq, @"PurchaseData/AddToCart", "token ne");
            if (data.Status)
            {
                _covertHelper.TryParseDynamicToString(data.Data,out customerCode);
                HttpContext.Session.SetString("CustomerCode", customerCode);
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
                
            }
            return Json(new { success = data.Status,Data = customerCode });
            //return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
