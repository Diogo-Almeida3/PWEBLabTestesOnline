using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;

namespace PWEBLabTestesOnline.Controllers
{
    public class AnalysisTestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalysisTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AnalysisTests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AnalysisTests.Include(a => a.Type);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AnalysisTests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisTests = await _context.AnalysisTests
                .Include(a => a.Type)
                .FirstOrDefaultAsync(m => m.AnalysisTestsId == id);
            if (analysisTests == null)
            {
                return NotFound();
            }

            return View(analysisTests);
        }

        // GET: AnalysisTests/Create
        public IActionResult Create()
        {
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>(), "TypeAnalysisTestsId", "Name");
            return View();
        }

        // POST: AnalysisTests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnalysisTestsId,Date,Location,Result,TechnicianName,TypeAnalysisTestsId")] AnalysisTests analysisTests)
        {
            if (ModelState.IsValid)
            {
                _context.Add(analysisTests);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>(), "TypeAnalysisTestsId", "Name", analysisTests.TypeAnalysisTestsId);
            return View(analysisTests);
        }

        // GET: AnalysisTests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisTests = await _context.AnalysisTests.FindAsync(id);
            if (analysisTests == null)
            {
                return NotFound();
            }
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>(), "TypeAnalysisTestsId", "Name", analysisTests.TypeAnalysisTestsId);
            return View(analysisTests);
        }

        // POST: AnalysisTests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnalysisTestsId,Date,Location,Result,TechnicianName,TypeAnalysisTestsId")] AnalysisTests analysisTests)
        {
            if (id != analysisTests.AnalysisTestsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(analysisTests);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnalysisTestsExists(analysisTests.AnalysisTestsId))
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
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>(), "TypeAnalysisTestsId", "Name", analysisTests.TypeAnalysisTestsId);
            return View(analysisTests);
        }

        // GET: AnalysisTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysisTests = await _context.AnalysisTests
                .Include(a => a.Type)
                .FirstOrDefaultAsync(m => m.AnalysisTestsId == id);
            if (analysisTests == null)
            {
                return NotFound();
            }

            return View(analysisTests);
        }

        // POST: AnalysisTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var analysisTests = await _context.AnalysisTests.FindAsync(id);
            _context.AnalysisTests.Remove(analysisTests);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnalysisTestsExists(int id)
        {
            return _context.AnalysisTests.Any(e => e.AnalysisTestsId == id);
        }
    }
}
