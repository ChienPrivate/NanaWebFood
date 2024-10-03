using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper.BaseModel;
using Helper.Convert;
using Helper.Provider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using StoreManagement.CallAPICenter;
using StoreManagement.Data;
using StoreManagement.Models;
using StoreManagement.Models.Request;
//using StoreManagement.Viewver;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace StoreManagement.Controllers
{
    [Route("Carts")]
    public class CartsController : Controller
    {
        private readonly DataContext _context;
        private readonly ConvertHelper _covertHelper;
        private readonly CallApiCenter _callAPI;
        private readonly ProviderHelper _provHelper;
        //private readonly IViewRenderer _viewRenderer;
        //private readonly IActionContextAccessor _actionContextAccessor;
        //private readonly ICompositeViewEngine _viewEngine;
        public CartsController(DataContext context)
        {
            //, IActionContextAccessor actionContextAccessor, ICompositeViewEngine viewEngine
            //_viewRenderer = viewRenderer;
            //_actionContextAccessor = actionContextAccessor;
            _context = context;
            _callAPI = new CallApiCenter();
            _covertHelper = new ConvertHelper();
            _provHelper = new ProviderHelper();
            //_viewEngine = viewEngine;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            string customerCode = string.Empty;
            if (HttpContext.Session.GetString("CustomerCode") != null)
            {
                ViewBag.CustomerCode = HttpContext.Session.GetString("CustomerCode");
                customerCode = HttpContext.Session.GetString("CustomerCode");
            }
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                ViewBag.CustomerId = HttpContext.Session.GetString("CustomerId");
            }
            if (HttpContext.Session.GetString("Role") != null)
            {
                ViewBag.Role = HttpContext.Session.GetString("Role");
            }

            var req = new GetCartForCustReq();
            req.ModelRequest = customerCode;
            var cart = new Carts();
            var carDtl = new List<CartDetails>();
            ResponeModel data = await _callAPI.PostMethod(req, @"PurchaseData/GetCartForCust", string.Empty);
            if (data.Status && data.Data != null)
            {
                _covertHelper.ConvertDynamicToT<Carts>(data.Data, out cart);
                ViewData["Carts"] = cart;
                var reqDtl = new GetCartDetailByIdReq();
                reqDtl.ModelRequest = cart.CartId;
               
                ResponeModel datadtl = await _callAPI.PostMethod(reqDtl, @"PurchaseData/GetCartDetailForCust", string.Empty);
                if (datadtl.Status && datadtl.Data != null)
                {
                    carDtl = _covertHelper.ConvertDynamicToList<CartDetails>(datadtl.Data);
                }
                ViewData["CartDetails"] = carDtl;
            }
            else
            {
                ViewData["Carts"] = cart;
                ViewData["CartDetails"] = carDtl;
            }
            if (customerCode.Contains("CusTemp"))
            {
                ViewData["Customer"] = new Customers() { 
                    CustomerCd = customerCode,
                    CustomerId=0,
                    Guest_YN=true
                };

            }
            else{
                var cusReq = new GetCusInoReq()
                {
                    ModelRequest = customerCode
                };
                var cus = new Customers();
                ResponeModel datadtl = await _callAPI.PostMethod(cusReq, @"PurchaseData/GetCustInfo", string.Empty);
                if (datadtl.Status && datadtl.Data != null)
                {
                     _covertHelper.ConvertDynamicToT<Customers>(datadtl.Data, out cus);
                }
                ViewData["Customer"] = cus;
            }

            //Carts cartMst = await _iMasterDataFactory.GetCartsByCustomerCode(customerCode);

            //return View(await _context.Carts.ToListAsync());
            return View();
        }

        [HttpGet("Details")]
        public async Task<IActionResult> Details(string search =null)
        {
            string customerCode = string.Empty;
            if (HttpContext.Session.GetString("CustomerCode") != null)
            {
                ViewBag.CustomerCode = HttpContext.Session.GetString("CustomerCode");
                customerCode = HttpContext.Session.GetString("CustomerCode");
            }
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                ViewBag.CustomerId = HttpContext.Session.GetString("CustomerId");
            }
            if (HttpContext.Session.GetString("Role") != null)
            {
                ViewBag.Role = HttpContext.Session.GetString("Role");
            }

            var req = new GetCartForCustReq();
            req.ModelRequest = string.IsNullOrEmpty(search)? customerCode : search;
            var cart = new Carts();
            var carDtl = new List<CartDetails>();
            ResponeModel data = await _callAPI.PostMethod(req, @"PurchaseData/GetCartByCartCd", string.Empty);
            if (data.Status && data.Data != null)
            {
                _covertHelper.ConvertDynamicToT<Carts>(data.Data, out cart);
                ViewData["Carts"] = cart;
                var reqDtl = new GetCartDetailByIdReq();
                reqDtl.ModelRequest = cart.CartId;

                ResponeModel datadtl = await _callAPI.PostMethod(reqDtl, @"PurchaseData/GetCartDetailByIdForDetail", string.Empty);
                if (datadtl.Status && datadtl.Data != null)
                {
                    carDtl = _covertHelper.ConvertDynamicToList<CartDetails>(datadtl.Data);
                }
                ViewData["CartDetails"] = carDtl;
                cart.CartDetails = carDtl;
            }
            else
            {
                ViewData["Carts"] = cart;
                ViewData["CartDetails"] = carDtl;
            }
            if (customerCode.Contains("CusTemp"))
            {
                ViewData["Customer"] = new Customers()
                {
                    CustomerCd = customerCode,
                    CustomerId = 0,
                    Guest_YN = true
                };

            }
            else
            {
                var cusReq = new GetCusInoReq()
                {
                    ModelRequest = customerCode
                };
                var cus = new Customers();
                ResponeModel datadtl = await _callAPI.PostMethod(cusReq, @"PurchaseData/GetCustInfo", string.Empty);
                if (datadtl.Status && datadtl.Data != null)
                {
                    _covertHelper.ConvertDynamicToT<Customers>(datadtl.Data, out cus);
                }
                ViewData["Customer"] = cus;
            }
            // Check if the request is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //var tableHtml = await RenderPartialViewToString("_CartTablePartial", cart);
                //var cartHtml = await RenderPartialViewToString("_CartSummaryPartial", cart);

                //return Json(new { tableHtml = tableHtml, cartHtml = cartHtml });
            }

            //Carts cartMst = await _iMasterDataFactory.GetCartsByCustomerCode(customerCode);

            //return View(await _context.Carts.ToListAsync());
            return View(cart);
        }
        // Helper method to render a view to a string
        // Helper method to render partial views to string
        //private async Task<string> RenderPartialViewToString(string viewName, object model)
        //{
        //    var viewResult = _viewEngine.FindView(new ActionContext { HttpContext = HttpContext }, viewName, false);
        //    if (!viewResult.Success)
        //    {
        //        throw new ArgumentException($"View {viewName} not found.");
        //    }

        //    using (var sw = new StringWriter())
        //    {
        //        var viewContext = new ViewContext
        //        {
        //            HttpContext = HttpContext,
        //            RouteData = RouteData,
        //            ActionDescriptor = new ActionDescriptor(),
        //            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        //            {
        //                Model = model
        //            },
        //            Writer = sw
        //        };

        //        await viewResult.View.RenderAsync(viewContext);
        //        return sw.ToString();
        //    }
        //}
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        public async Task<IActionResult> Create([Bind("CartId,GuestId,CustomerId,CustomerCd,CustomerName,PhoneNumber,Address,TotalMoney,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] Carts carts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carts);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carts = await _context.Carts.FindAsync(id);
            if (carts == null)
            {
                return NotFound();
            }
            return View(carts);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit")]
        
        public async Task<IActionResult> Edit(int id, [Bind("CartId,GuestId,CustomerId,CustomerCd,CustomerName,PhoneNumber,Address,TotalMoney,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] Carts carts)
        {
            if (id != carts.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartsExists(carts.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carts);
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carts = await _context.Carts
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (carts == null)
            {
                return NotFound();
            }

            return View(carts);
        }

        // POST: Carts/Delete/5
        [HttpPost("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carts = await _context.Carts.FindAsync(id);
            if (carts != null)
            {
                _context.Carts.Remove(carts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartsExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }

        [HttpGet("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity(int dtl, int quantity)
        {
            var res = new ResponeModel();
            try
            {
                string customerCode = HttpContext.Session.GetString("CustomerCode");
                var req = new UpdateQuantiyReq()
                {
                    Quantity = quantity,
                    DtlId = dtl,
                    CustomerCode = customerCode
                };
                res = await _callAPI.PostMethod(req , @"PurchaseData/UpdateQuantity", string.Empty);
                if (res.Status)
                {
                    return RedirectToAction("Index", "Carts");
                }
                else
                {
                    string errorMessage = res.Message;
                    return RedirectToAction("Error", "Home", new { errorMessage });
                }
            }
            catch (Exception ex)
            {

                string errorMessage = _provHelper.GetMessage(ex);
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }

        [HttpGet("DeleteCartDetail")]
        public async Task<IActionResult> DeleteCartDetail(int id, int quantity)
        {
            var res = new ResponeModel();
            try
            {
                string customerCode = HttpContext.Session.GetString("CustomerCode");
                var req = new UpdateQuantiyReq()
                {
                    Quantity = quantity,
                    DtlId = id,
                    CustomerCode = customerCode
                };
                res= await _callAPI.PostMethod(req, @"PurchaseData/DeleteCartDetail", string.Empty);
                if (res.Status)
                {
                    return RedirectToAction("Index", "Carts");
                }
                else{
                    string errorMessage = res.Message;
                    return RedirectToAction("Error", "Home", new { errorMessage });
                }
            }
            catch (Exception ex)
            {
                string errorMessage = _provHelper.GetMessage(ex);
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Payment([Bind("CustomerName,Sex,Address,CustomerCd,CustomerId,PhoneNumber,Email,Ck_YN,Guest_YN")] CustomerPayReq cust)
        {
            var res = new ResponeModel();
            try
            {
                string customerCode = HttpContext.Session.GetString("CustomerCode");
                var req = new PaymentReq()
                {
                   ModelRequest = cust
                };
                res = await _callAPI.PostMethod(req, @"PurchaseData/Payment", string.Empty);
                if (res.Status)
                {
                    return RedirectToAction("Index", "Carts");
                }
                else
                {
                    string errorMessage = res.Message;
                    return RedirectToAction("Error", "Home", new { errorMessage });
                }
            }
            catch (Exception ex)
            {
                string errorMessage = _provHelper.GetMessage(ex);
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
    }
}
