using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;

namespace PWEBLabTestesOnline.Controllers
{
    [Authorize(Roles = ("Manager"))]
    public class TypeTestsController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public TypeTestsController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }

        // GET: TypeTests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TypeAnalysisTests.Where(t => t.CreatedById == userManager.GetUserId(User));
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: TypeTests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeTests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeAnalysisTestsId,Name,CreatedById")] TypeAnalysisTests typeAnalysisTests)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.GetUserAsync(User);
                typeAnalysisTests.CreatedById = currentUser.Id;
                typeAnalysisTests.CreatedBy = currentUser;
                _context.Add(typeAnalysisTests);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeAnalysisTests);
        }


        // GET: TypeTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeAnalysisTests = await _context.TypeAnalysisTests
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.TypeAnalysisTestsId == id && m.CreatedById == userManager.GetUserId(User));
            if (typeAnalysisTests == null)
            {
                return NotFound();
            }

            return View(typeAnalysisTests);
        }

        // POST: TypeTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeAnalysisTests = await _context.TypeAnalysisTests.FindAsync(id);
            if (typeAnalysisTests.CreatedById != userManager.GetUserId(User))
                return NotFound();
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
