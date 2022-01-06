using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;

namespace PWEBLabTestesOnline.Controllers
{
    public class LaboratoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LaboratoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Laboratories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Laboratories.ToListAsync());
        }

        // GET: Laboratories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratories = await _context.Laboratories
                .FirstOrDefaultAsync(m => m.LaboratoriesId == id);
            if (laboratories == null)
            {
                return NotFound();
            }

            return View(laboratories);
        }

        // GET: Laboratories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Laboratories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LaboratoriesId,LaboratoriesName,Location,PhoneNumber")] Laboratories laboratories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(laboratories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(laboratories);
        }

        // GET: Laboratories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratories = await _context.Laboratories.FindAsync(id);
            if (laboratories == null)
            {
                return NotFound();
            }
            return View(laboratories);
        }

        // POST: Laboratories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LaboratoriesId,LaboratoriesName,Location,PhoneNumber")] Laboratories laboratories)
        {
            if (id != laboratories.LaboratoriesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(laboratories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaboratoriesExists(laboratories.LaboratoriesId))
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
            return View(laboratories);
        }

        // GET: Laboratories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laboratories = await _context.Laboratories
                .FirstOrDefaultAsync(m => m.LaboratoriesId == id);
            if (laboratories == null)
            {
                return NotFound();
            }

            return View(laboratories);
        }

        // POST: Laboratories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var laboratories = await _context.Laboratories.FindAsync(id);
            _context.Laboratories.Remove(laboratories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LaboratoriesExists(int id)
        {
            return _context.Laboratories.Any(e => e.LaboratoriesId == id);
        }
    }
}
