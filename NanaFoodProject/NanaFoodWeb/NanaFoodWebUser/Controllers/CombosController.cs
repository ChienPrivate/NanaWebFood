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
    public class CombosController : Controller
    {
        private readonly CallApiCenter _callAPI;
        private readonly DataContext _context;

        public CombosController(DataContext context)
        {
            _context = context;
            _callAPI = new CallApiCenter();
        }

        // GET: Combos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Combos.ToListAsync());
        }

        // GET: Combos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combos = await _context.Combos
                .FirstOrDefaultAsync(m => m.ComboId == id);
            if (combos == null)
            {
                return NotFound();
            }

            return View(combos);
        }

        // GET: Combos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Combos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComboId,ComboName,Description,Price,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] Combos combos)
        {
            var req = new RequestData();
            req.FunctionCode = "C";
            //req.ModelRequest = combos;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Combos", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //combos.CreateBy = "Admin";
            //combos.UpdateBy = "Admin";
            //combos.CreateDate = DateTime.Now;
            //combos.UpdateDate = DateTime.Now;
            //_context.Add(combos);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            //if (ModelState.IsValid)
            //{
            //    _context.Add(combos);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(combos);
        }

        // GET: Combos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combos = await _context.Combos.FindAsync(id);
            if (combos == null)
            {
                return NotFound();
            }
            return View(combos);
        }

        // POST: Combos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComboId,ComboName,Description,Price,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] Combos combos)
        {
            var req = new RequestData();
            req.FunctionCode = "U";
            //req.ModelRequest = combos;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Combos", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //if (id != combos.ComboId)
            //{
            //    return NotFound();
            //}
            //else
            //{
            //    try
            //    {
            //        _context.Update(combos);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!CombosExists(combos.ComboId))
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
            //        _context.Update(combos);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!CombosExists(combos.ComboId))
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
            //return View(combos);
        }

        // GET: Combos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combos = await _context.Combos
                .FirstOrDefaultAsync(m => m.ComboId == id);
            if (combos == null)
            {
                return NotFound();
            }

            return View(combos);
        }

        // POST: Combos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = new RequestData();
            req.FunctionCode = "D";
            //req.ModelRequest = id;
            ResponeModel data = await _callAPI.PostMethod(req, @"MasterData/Combos", string.Empty);
            if (data.Status)
            {
                //lstItem = _covertHelper.ConvertDynamicToList<Items>(data.Data);
            }
            return RedirectToAction(nameof(Index));
            //var combos = await _context.Combos.FindAsync(id);
            //if (combos != null)
            //{
            //    _context.Combos.Remove(combos);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool CombosExists(int id)
        {
            return _context.Combos.Any(e => e.ComboId == id);
        }
    }
}
