using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper.BaseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.CallAPICenter;
using StoreManagement.Data;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class GuestsController : Controller
    {
        private readonly DataContext _context;
        private readonly CallApiCenter _callAPI;

        public GuestsController(DataContext context)
        {
            _context = context;
            _callAPI = new CallApiCenter();
        }

        // GET: Guests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Guests.ToListAsync());
        }

        // GET: Guests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.GuestId == id);
            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // GET: Guests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GuestId,GuestLastName,GuestFirsttName,Email,Address,PhoneNumber,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] Guest guest)
        {
            var req = new RequestData();
            req.FunctionCode = "C";
            //req.ModelRequest = guest;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Guests", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (ModelState.IsValid)
            //{
            //    _context.Add(guest);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(guest);
        }

        // GET: Guests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return View(guest);
        }

        // POST: Guests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GuestId,GuestLastName,GuestFirsttName,Email,Address,PhoneNumber,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] Guest guest)
        {
            var req = new RequestData();
            req.FunctionCode = "U";
            //req.ModelRequest = guest;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Guests", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (id != guest.GuestId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(guest);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!GuestExists(guest.GuestId))
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
            //return View(guest);
        }

        // GET: Guests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.GuestId == id);
            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // POST: Guests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Guests", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var guest = await _context.Guests.FindAsync(id);
            //if (guest != null)
            //{
            //    _context.Guests.Remove(guest);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool GuestExists(int id)
        {
            return _context.Guests.Any(e => e.GuestId == id);
        }
    }
}
