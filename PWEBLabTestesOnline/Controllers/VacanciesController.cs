﻿using System;
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

namespace PWEBLabTestesOnline.Data.Migrations
{
    [Authorize(Roles = ("Manager"))]
    public class VacanciesController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public VacanciesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }

        // GET: Vacancies
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vacancies.Include(v => v.Laboratory).Include(v => v.Type).Where(l => l.Laboratory.ManagerId == userManager.GetUserId(User));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vacancies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancies = await _context.Vacancies
                .Include(v => v.Laboratory)
                .Include(v => v.Type)
                .Where(l => l.Laboratory.ManagerId == userManager.GetUserId(User))
                .FirstOrDefaultAsync(m => m.VacanciesId == id);
            if (vacancies == null)
            {
                return NotFound();
            }

            return View(vacancies);
        }

        // GET: Vacancies/Create
        public IActionResult Create()
        {
            ViewData["LaboratoryId"] = new SelectList(_context.Laboratories.Where(l => l.ManagerId == userManager.GetUserId(User)).ToList(), "LaboratoriesId", "LaboratoriesName");
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.TypeAnalysisTests.Where(a => a.CreatedById == userManager.GetUserId(User)).ToList(), "TypeAnalysisTestsId", "Name");
            return View();
        }

        // POST: Vacancies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VacanciesId,LaboratoryId,TypeAnalysisTestsId,DailyLimit,Opening,Enclosure")] Vacancies vacancies)
        {
            if (vacancies.Laboratory.ManagerId == userManager.GetUserId(User)) ModelState.AddModelError("Laboratory", "This laboratory is not run by this user");
            if (vacancies.Type.CreatedById == userManager.GetUserId(User)) ModelState.AddModelError("Type", "This type of test does not belong to this user");

            if (ModelState.IsValid)
            {
                _context.Add(vacancies);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LaboratoryId"] = new SelectList(_context.Laboratories.Where(l => l.ManagerId == userManager.GetUserId(User)).ToList(), "LaboratoriesId", "LaboratoriesName");
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.TypeAnalysisTests.Where(a => a.CreatedById == userManager.GetUserId(User)).ToList(), "TypeAnalysisTestsId", "Name");
            return View(vacancies);
        }

        // GET: Vacancies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancies = await _context.Vacancies.FindAsync(id);
            if (vacancies == null)
            {
                return NotFound();
            }
            ViewData["LaboratoryId"] = new SelectList(_context.Laboratories, "LaboratoriesId", "LaboratoriesName", vacancies.LaboratoryId);
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.TypeAnalysisTests, "TypeAnalysisTestsId", "Name", vacancies.TypeAnalysisTestsId);
            return View(vacancies);
        }

        // POST: Vacancies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VacanciesId,LaboratoryId,TypeAnalysisTestsId,DailyLimit,Opening,Enclosure")] Vacancies vacancies)
        {
            if (id != vacancies.VacanciesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacancies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacanciesExists(vacancies.VacanciesId))
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
            ViewData["LaboratoryId"] = new SelectList(_context.Laboratories, "LaboratoriesId", "LaboratoriesName", vacancies.LaboratoryId);
            ViewData["TypeAnalysisTestsId"] = new SelectList(_context.TypeAnalysisTests, "TypeAnalysisTestsId", "Name", vacancies.TypeAnalysisTestsId);
            return View(vacancies);
        }

        // GET: Vacancies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancies = await _context.Vacancies
                .Include(v => v.Laboratory)
                .Include(v => v.Type)
                .FirstOrDefaultAsync(m => m.VacanciesId == id);
            if (vacancies == null)
            {
                return NotFound();
            }

            return View(vacancies);
        }

        // POST: Vacancies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacancies = await _context.Vacancies.FindAsync(id);
            _context.Vacancies.Remove(vacancies);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacanciesExists(int id)
        {
            return _context.Vacancies.Any(e => e.VacanciesId == id);
        }
    }
}
