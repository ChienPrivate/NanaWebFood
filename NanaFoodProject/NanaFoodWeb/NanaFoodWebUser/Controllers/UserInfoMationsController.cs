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
    public class UserInfoMationsController : Controller
    {
        private readonly DataContext _context;

        public UserInfoMationsController(DataContext context)
        {
            _context = context;
        }

        // GET: UserInfoMations
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserInfoMations.ToListAsync());
        }

        // GET: UserInfoMations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInfoMation = await _context.UserInfoMations
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userInfoMation == null)
            {
                return NotFound();
            }

            return View(userInfoMation);
        }

        // GET: UserInfoMations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserInfoMations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FirstName,LastName,FullName,Email,PhoneNumber,Address,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] UserInfoMation userInfoMation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userInfoMation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userInfoMation);
        }

        // GET: UserInfoMations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInfoMation = await _context.UserInfoMations.FindAsync(id);
            if (userInfoMation == null)
            {
                return NotFound();
            }
            return View(userInfoMation);
        }

        // POST: UserInfoMations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,FullName,Email,PhoneNumber,Address,CreateBy,UpdateBy,CreateDate,UpdateDate,Active")] UserInfoMation userInfoMation)
        {
            if (id != userInfoMation.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userInfoMation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserInfoMationExists(userInfoMation.UserId))
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
            return View(userInfoMation);
        }

        // GET: UserInfoMations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInfoMation = await _context.UserInfoMations
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userInfoMation == null)
            {
                return NotFound();
            }

            return View(userInfoMation);
        }

        // POST: UserInfoMations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userInfoMation = await _context.UserInfoMations.FindAsync(id);
            if (userInfoMation != null)
            {
                _context.UserInfoMations.Remove(userInfoMation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserInfoMationExists(int id)
        {
            return _context.UserInfoMations.Any(e => e.UserId == id);
        }
    }
}
