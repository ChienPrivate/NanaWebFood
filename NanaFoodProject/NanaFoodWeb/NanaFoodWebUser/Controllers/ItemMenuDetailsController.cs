using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Data;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class ItemMenuDetailsController : Controller
    {
        private readonly DataContext _context;

        public ItemMenuDetailsController(DataContext context)
        {
            _context = context;
        }

        // GET: ItemMenuDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.ItemMenuDtls.ToListAsync());
        }

        // GET: ItemMenuDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenuDetail = await _context.ItemMenuDtls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemMenuDetail == null)
            {
                return NotFound();
            }

            return View(itemMenuDetail);
        }

        // GET: ItemMenuDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemMenuDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemMenuId,ItemId,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] ItemMenuDetail itemMenuDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemMenuDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemMenuDetail);
        }

        // GET: ItemMenuDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenuDetail = await _context.ItemMenuDtls.FindAsync(id);
            if (itemMenuDetail == null)
            {
                return NotFound();
            }
            return View(itemMenuDetail);
        }

        // POST: ItemMenuDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemMenuId,ItemId,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] ItemMenuDetail itemMenuDetail)
        {
            if (id != itemMenuDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemMenuDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemMenuDetailExists(itemMenuDetail.Id))
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
            return View(itemMenuDetail);
        }

        // GET: ItemMenuDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenuDetail = await _context.ItemMenuDtls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemMenuDetail == null)
            {
                return NotFound();
            }

            return View(itemMenuDetail);
        }

        // POST: ItemMenuDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemMenuDetail = await _context.ItemMenuDtls.FindAsync(id);
            if (itemMenuDetail != null)
            {
                _context.ItemMenuDtls.Remove(itemMenuDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemMenuDetailExists(int id)
        {
            return _context.ItemMenuDtls.Any(e => e.Id == id);
        }
    }
}
