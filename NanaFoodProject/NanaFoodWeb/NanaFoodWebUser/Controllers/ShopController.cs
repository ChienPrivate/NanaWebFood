using Helper.BaseModel;
using Helper.Convert;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.CallAPICenter;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using System.Diagnostics;

namespace StoreManagement.Controllers
{
    [Route("[controller]")]
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        private readonly CallApiCenter _callAPI;
        private readonly ConvertHelper _covertHelper;
        public ShopController(ILogger<ShopController> logger)
        {
            _logger = logger;
            _callAPI = new CallApiCenter();
            _covertHelper = new ConvertHelper();
        }
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            List<Items> lstItem = new List<Items>();
            var req = new ItemRequest()
            {
                ModelRequest = new ItemMasterReq()
            };
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Items", "token ne");
            if (data.Status)
            {
                lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            string cuscode = HttpContext.Session.GetString("CustomerCode");
            //ViewData["CustCode"] = cuscode;
            ViewBag.CustCode = cuscode;
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
            if (data.Status)
            {
                _covertHelper.ConvertDynamicToT<Items>(data.Data, out itemRes);
                //lstImage = _covertHelper.ConvertDynamicToList<ItemImage>(data.Data.ListImage);
            }
            ViewData["Itm"] = itemRes;
            ViewData["ListImg"] = itemRes.ItemImages;
            return View();
        }

        [HttpGet("AddToCart")]
        public async Task<IActionResult> AddToCart(int id, decimal price, int quantity)
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
                _covertHelper.TryParseDynamicToString(data.Data, out customerCode);
                HttpContext.Session.SetString("CustomerCode", customerCode);
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
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
