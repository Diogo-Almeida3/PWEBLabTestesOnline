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
    public class SchedulesController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public SchedulesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }

        // GET: Schedules
        [Authorize(Roles = ("Techinician"))]
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var applicationDbContext = _context.Schedules
                .Include(s => s.Client)
                .Include(s => s.Laboratory)
                .Include(s => s.Techinician)
                .Include(s => s.TestType)
                .Where(s => s.Laboratory.Techinicians.Contains(currentUser));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Schedules/Details/5
        [Authorize(Roles = ("Techinician"))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await userManager.GetUserAsync(User);
            var schedules = await _context.Schedules
                .Include(s => s.Client)
                .Include(s => s.Laboratory)
                .Include(s => s.Techinician)
                .Include(s => s.TestType)
                .Where(s => s.Laboratory.Techinicians.Contains(currentUser))
                .FirstOrDefaultAsync(m => m.SchedulesId == id);
            if (schedules == null)
            {
                return NotFound();
            }

            return View(schedules);
        }

        // GET: Schedules/Create
        [Authorize(Roles = ("Client"))]
        public IActionResult Create()
        {
            ViewData["LaboratoryId"] = new SelectList(_context.Laboratories, "LaboratoriesId", "LaboratoriesName");
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Client"))]
        public async Task<IActionResult> Create([Bind("SchedulesId,AppointmentTime,LaboratoryId,TestTypeId")] Schedules schedules,List<string> lab_tests)
        {
            schedules.LaboratoryId = int.Parse(lab_tests[0]);
            
            if (lab_tests.Count > 1 && lab_tests[1] == "Create") // Se carregou no botão create
            {
                var vacancy = await _context.Vacancies.Where(v => v.TypeAnalysisTestsId == schedules.TestTypeId).FirstOrDefaultAsync();
                //TODO: verificar esta validação
                if (DateTime.Now > schedules.AppointmentTime)
                    ModelState.AddModelError("AppointmentTime", "You must enter a schedule date that is greater than the current date and time");

                if (schedules.AppointmentTime.TimeOfDay < vacancy.Opening.TimeOfDay || schedules.AppointmentTime.TimeOfDay > vacancy.Enclosure.TimeOfDay)
                    ModelState.AddModelError("AppointmentTime", "It is not possible to schedule a test at this time");

                var DailyLimit = await _context.Schedules.Where(s => s.AppointmentTime.Date == schedules.AppointmentTime.Date).CountAsync();
                if (DailyLimit >= vacancy.DailyLimit)
                    ModelState.AddModelError("DailyLimit", "There are no more vacancies for today");

                ModelState.MaxAllowedErrors = 1;

                if (!ModelState.HasReachedMaxErrors && ModelState.IsValid)
                {
                    // Adicionar restantes valores
                    var currentUser = await userManager.GetUserAsync(User);
                    schedules.Client = currentUser;
                    schedules.ClientId = currentUser.Id;
                    schedules.Result = null;
                    schedules.Laboratory = await _context.Laboratories.FirstAsync(l => l.LaboratoriesId == schedules.LaboratoryId);
                    _context.Add(schedules);
                    await _context.SaveChangesAsync();
                    return View("~/Views/Home/Index.cshtml");
                }
                ViewData["LaboratoryId"] = new SelectList(_context.Laboratories, "LaboratoriesId", "LaboratoriesName", schedules.LaboratoryId);
                return View("Create");
            }
            else // Se carregou na selectlist do laboratório
            {
                var lab = _context.Laboratories
                    .Include(l => l.Manager)
                    .Where(l => l.LaboratoriesId == schedules.LaboratoryId)
                    .FirstOrDefault();
                if (lab == null)
                    return NotFound();

                var Manager = lab.Manager;
                ViewData["LaboratoryId"] = new SelectList(_context.Laboratories, "LaboratoriesId", "LaboratoriesName", schedules.LaboratoryId);
                ViewData["TestTypeId"] = new SelectList(_context.Set<TypeAnalysisTests>().Where(t => t.CreatedById == Manager.Id).ToList(), "TypeAnalysisTestsId", "Name");
                return View("Create");
            }
        }

        // GET: Schedules/Edit/5
        [Authorize(Roles = ("Techinician"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await userManager.GetUserAsync(User);
            var schedules = await _context.Schedules
                .Include(s => s.Client)
                .Include(s => s.Laboratory)
                .Include(s => s.Techinician)
                .Include(s => s.TestType)
                .Where(s => s.Laboratory.Techinicians.Contains(currentUser))
                .FirstOrDefaultAsync(m => m.SchedulesId == id);

            if (schedules == null)
            {
                return NotFound();
            }

            return View(schedules);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Techinician"))]
        public async Task<IActionResult> Edit(int id, [Bind("SchedulesId,Result,TechinicianId")] Schedules schedules)
        {
            if (id != schedules.SchedulesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchedulesExists(schedules.SchedulesId))
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
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", schedules.ClientId);
            ViewData["LaboratoryId"] = new SelectList(_context.Laboratories, "LaboratoriesId", "LaboratoriesName", schedules.LaboratoryId);
            ViewData["TechinicianId"] = new SelectList(_context.Users, "Id", "Id", schedules.TechinicianId);
            ViewData["TestTypeId"] = new SelectList(_context.TypeAnalysisTests, "TypeAnalysisTestsId", "Name", schedules.TestTypeId);
            return View(schedules);
        }

        // GET: Schedules/Delete/5
        [Authorize(Roles = ("Client"))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedules = await _context.Schedules
                .Include(s => s.Client)
                .Include(s => s.Laboratory)
                .Include(s => s.Techinician)
                .Include(s => s.TestType)
                .FirstOrDefaultAsync(m => m.SchedulesId == id);
            if (schedules == null)
            {
                return NotFound();
            }

            return View(schedules);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Client"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedules = await _context.Schedules.FindAsync(id);
            _context.Schedules.Remove(schedules);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchedulesExists(int id)
        {
            return _context.Schedules.Any(e => e.SchedulesId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responsible(int SchedulesId)
        {

            var currentUser = await userManager.GetUserAsync(User);

            var test = await _context.Schedules
                .Include(s => s.Client)
                .Include(s => s.Laboratory)
                .Include(s => s.Techinician)
                .Include(s => s.TestType)
                .Where(s => s.Laboratory.Techinicians.Contains(currentUser))
                .FirstOrDefaultAsync(s => s.SchedulesId == SchedulesId);

            test.Techinician = currentUser;
            test.TechinicianId = currentUser.Id;

            _context.Schedules.Update(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
