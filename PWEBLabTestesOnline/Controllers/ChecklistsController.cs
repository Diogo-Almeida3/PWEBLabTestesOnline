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
    public class ChecklistsController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public ChecklistsController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }

        // GET: Checklists
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var applicationDbContext = _context.Checklist.Include(c => c.CreatedBy).Where(c => c.CreatedById == currentUser.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Checklists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currentUser = await userManager.GetUserAsync(User);
            var checklist = await _context.Checklist
                .Include(c => c.Procedures)
                .Where(c => c.CreatedById == currentUser.Id)
                .FirstOrDefaultAsync(m => m.ChecklistId == id);
            if (checklist == null)
            {
                return NotFound();
            }

            return View(checklist);
        }

        // GET: Checklists/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await userManager.GetUserAsync(User);
            ViewData["ck_procedures"] = new SelectList(_context.Procedure.Where(p => p.CreatedById == currentUser.Id).ToList(), "ProcedureId", "ProcedureDescription");
            return View();
        }

        // POST: Checklists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChecklistId,Name,CreatedById")] Checklist checklist)
        {
            if (ModelState.IsValid)
            {
                checklist.CreatedBy = await userManager.GetUserAsync(User);
                checklist.CreatedById = checklist.CreatedBy.Id;      
                _context.Add(checklist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(checklist);
        }

        // GET: Checklists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checklist = await _context.Checklist.Include(c => c.Procedures).Where(c => c.ChecklistId == id).FirstAsync();
            if (checklist == null)
            {
                return NotFound();
            }
            var currentUser = await userManager.GetUserAsync(User);
            List<int> selectedValues = new List<int>();
            foreach(var p in checklist.Procedures)
            {
                selectedValues.Add(p.ProcedureId);
            }

            ViewData["ck_procedures"] = new MultiSelectList(_context.Procedure.Where(p => p.CreatedById == currentUser.Id).ToList(), "ProcedureId", "ProcedureDescription", selectedValues);
            return View(checklist);
        }

        // POST: Checklists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChecklistId,Name")] Checklist checklist, List<int> Procedures)
        {
            if (id != checklist.ChecklistId)
            {
                return NotFound();
            }
            var currentUser = await userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                try
                {
                    // adicionar procedures
                    var ck = await _context.Checklist.Include(c => c.Procedures).Where(c => c.ChecklistId == checklist.ChecklistId).FirstAsync();
                    ck.Name = checklist.Name;
                    ck.Procedures.Clear();
                    foreach (var procedure in Procedures)
                    {
                        var p = await _context.Procedure.Where(p => p.ProcedureId == procedure && p.CreatedById == currentUser.Id).FirstAsync();
                        ck.Procedures.Add(p);
                    }
                    _context.Update(ck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChecklistExists(checklist.ChecklistId))
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
            ViewData["ck_procedures"] = new MultiSelectList(_context.Procedure.Where(p => p.CreatedById == currentUser.Id).ToList(), "ProcedureId", "ProcedureDescription", Procedures);
            return View(checklist);
        }

        private bool ChecklistExists(int id)
        {
            return _context.Checklist.Any(e => e.ChecklistId == id);
        }
    }
}
