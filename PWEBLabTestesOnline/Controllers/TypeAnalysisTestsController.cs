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
    public class TypeAnalysisTestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TypeAnalysisTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TypeAnalysisTests
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeAnalysisTests.ToListAsync());
        }

        // GET: TypeAnalysisTests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeAnalysisTests = await _context.TypeAnalysisTests
                .FirstOrDefaultAsync(m => m.TypeAnalysisTestsId == id);
            if (typeAnalysisTests == null)
            {
                return NotFound();
            }

            return View(typeAnalysisTests);
        }

        // GET: TypeAnalysisTests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeAnalysisTests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeAnalysisTestsId,Name")] TypeAnalysisTests typeAnalysisTests)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeAnalysisTests);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeAnalysisTests);
        }

        // GET: TypeAnalysisTests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeAnalysisTests = await _context.TypeAnalysisTests.FindAsync(id);
            if (typeAnalysisTests == null)
            {
                return NotFound();
            }
            return View(typeAnalysisTests);
        }

        // POST: TypeAnalysisTests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeAnalysisTestsId,Name")] TypeAnalysisTests typeAnalysisTests)
        {
            if (id != typeAnalysisTests.TypeAnalysisTestsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeAnalysisTests);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeAnalysisTestsExists(typeAnalysisTests.TypeAnalysisTestsId))
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
            return View(typeAnalysisTests);
        }

        // GET: TypeAnalysisTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeAnalysisTests = await _context.TypeAnalysisTests
                .FirstOrDefaultAsync(m => m.TypeAnalysisTestsId == id);
            if (typeAnalysisTests == null)
            {
                return NotFound();
            }

            return View(typeAnalysisTests);
        }

        // POST: TypeAnalysisTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeAnalysisTests = await _context.TypeAnalysisTests.FindAsync(id);
            _context.TypeAnalysisTests.Remove(typeAnalysisTests);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeAnalysisTestsExists(int id)
        {
            return _context.TypeAnalysisTests.Any(e => e.TypeAnalysisTestsId == id);
        }
    }
}
