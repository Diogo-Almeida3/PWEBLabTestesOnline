using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;

namespace PWEBLabTestesOnline.Controllers
{
    public class ProceduresController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public ProceduresController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }

        // GET: Procedures
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var applicationDbContext = _context.Procedure.Include(p => p.typeAnalysisTests).Where(p => p.typeAnalysisTests.CreatedById == userManager.GetUserId(User));
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>().Where(t => t.CreatedById == currentUser.Id).ToList(), "TypeAnalysisTestsId", "Name");
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Procedures/Create
        public IActionResult Create()
        {
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>().Where(p => p.CreatedById == userManager.GetUserId(User)), "TypeAnalysisTestsId", "Name");
            return View();
        }

        // POST: Procedures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcedureId,ProcedureDescription,TypeAnalysisTestsId")] Procedure procedure)
        {
            var typeOfTest = _context.TypeAnalysisTests.Where(p => p.CreatedById == userManager.GetUserId(User)).FirstOrDefault(t => t.TypeAnalysisTestsId == procedure.TypeAnalysisTestsId);
            if (ModelState.IsValid && typeOfTest != null)
            {
                _context.Add(procedure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>().Where(p => p.CreatedById == userManager.GetUserId(User)), "TypeAnalysisTestsId", "Name");
            return View(procedure);
        }

        // GET: Procedures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = _context.Procedure.Where(p => p.ProcedureId == id && 
                                                            p.typeAnalysisTests.CreatedById == userManager.GetUserId(User)).FirstOrDefault();
            if (procedure == null)
            {
                return NotFound();
            }
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>().Where(p => p.CreatedById == userManager.GetUserId(User)), "TypeAnalysisTestsId", "Name");
            return View(procedure);
        }

        // POST: Procedures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProcedureId,ProcedureDescription,TypeAnalysisTestsId")] Procedure procedure)
        {
            if (id != procedure.ProcedureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(procedure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcedureExists(procedure.ProcedureId))
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
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>(), "TypeAnalysisTestsId", "Name", procedure.TypeAnalysisTestsId);
            return View(procedure);
        }

        // GET: Procedures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedure
                .Include(p => p.typeAnalysisTests)
                .FirstOrDefaultAsync(m => m.ProcedureId == id);
            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        // POST: Procedures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedure = await _context.Procedure.FindAsync(id);
            _context.Procedure.Remove(procedure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcedureExists(int id)
        {
            return _context.Procedure.Any(e => e.ProcedureId == id);
        }
        //Aqui é como se fosse produtos -> Categorias || Procedimentos -> Testes

        public async Task<IActionResult> ListByType(int TypeAnalysisTestsId)
        {
            List<Procedure> listTypes = null;
            if(TypeAnalysisTestsId == 0)
            {
                listTypes = _context.Procedure.Include(p => p.typeAnalysisTests).Where(p => p.typeAnalysisTests.CreatedById == userManager.GetUserId(User)).ToList();
            }
            else
            {
                listTypes = _context.Procedure.Include(p => p.typeAnalysisTests).Where(p => p.TypeAnalysisTestsId == TypeAnalysisTestsId).ToList();

            }
            var currentUser = await userManager.GetUserAsync(User);
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.Set<TypeAnalysisTests>().Where(t => t.CreatedById == currentUser.Id), "TypeAnalysisTestsId", "Name", TypeAnalysisTestsId);
            return View("Index", listTypes);
        }
    }
}
