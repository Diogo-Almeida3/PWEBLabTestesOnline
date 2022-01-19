using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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
        [Authorize(Roles = ("Techinician,Client"))]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Techinician"))
            {
                var currentUser = await userManager.GetUserAsync(User);
                var applicationDbContext = _context.Schedules
                    .Include(s => s.Client)
                    .Include(s => s.Laboratory)
                    .Include(s => s.Techinician)
                    .Include(s => s.TestType)
                    .Include(s => s.CurrentChecklist)
                    .Where(s => s.Laboratory.Techinicians.Contains(currentUser))
                    .OrderByDescending(s => s.AppointmentTime);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var currentUser = await userManager.GetUserAsync(User);
                var applicationDbContext = _context.Schedules
                    .Include(s => s.Client)
                    .Include(s => s.Laboratory)
                    .Include(s => s.Techinician)
                    .Include(s => s.TestType)
                    .Include(s => s.CurrentChecklist)
                    .Where(s => s.ClientId == currentUser.Id)
                    .OrderByDescending(s => s.AppointmentTime);
                return View(await applicationDbContext.ToListAsync());
            }
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
        public async Task<IActionResult> Create([Bind("SchedulesId,AppointmentTime,LaboratoryId,TestTypeId,CurrentChecklistId")] Schedules schedules, List<string> lab_tests)
        {
            schedules.LaboratoryId = int.Parse(lab_tests[0]);

            if (lab_tests.Count > 1 && lab_tests[1] == "Create") // Se carregou no botão create
            {
                var vacancy = await _context.Vacancies.Include(v => v.CurrentChecklist).Where(v => v.TypeAnalysisTestsId == schedules.TestTypeId && schedules.LaboratoryId == v.LaboratoryId).FirstOrDefaultAsync();

                if (DateTime.Now > schedules.AppointmentTime)
                    ModelState.AddModelError("AppointmentTime", "You must enter a schedule date that is greater than the current date and time");

                if (schedules.AppointmentTime.TimeOfDay < vacancy.Opening.TimeOfDay || schedules.AppointmentTime.TimeOfDay > vacancy.Enclosure.TimeOfDay)
                    ModelState.AddModelError("AppointmentTime", "It is not possible to schedule a test at this time");

                var DailyLimit = await _context.Schedules.Where(s => s.AppointmentTime.Date == schedules.AppointmentTime.Date).CountAsync();
                if (DailyLimit >= vacancy.DailyLimit)
                    ModelState.AddModelError("DailyLimit", "There are no more vacancies for today");

                if (ModelState.IsValid)
                {
                    schedules.CurrentChecklist = vacancy.CurrentChecklist;
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
                .Include(s => s.CurrentChecklist)
                .Include(s => s.CurrentChecklist.Procedures)
                .Where(s => s.Laboratory.Techinicians.Contains(currentUser) && s.TechinicianId == currentUser.Id)
                .FirstOrDefaultAsync(m => m.SchedulesId == id);

            if (schedules == null)
            {
                return NotFound();
            }
            //ViewData["checklist"] = await _context.Procedure.Where(p => p.TypeAnalysisTestsId == schedules.TestTypeId).ToListAsync();
            return View(schedules);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Techinician"))]
        public async Task<IActionResult> Edit(int id, Schedules schedules, List<int> ProcedureId)
        {
            if (id != schedules.SchedulesId)
            {
                return NotFound();
            }
            // TODO: fazer proteção de técnico responsável
            if (ModelState.IsValid)
            {
                try
                {

                    var currentUser = await userManager.GetUserAsync(User);
                    var schedulesUpdate = await _context.Schedules
                            .Include(s => s.Client)
                            .Include(s => s.Laboratory)
                            .Include(s => s.Techinician)
                            .Include(s => s.TestType)
                            .Include(s => s.CurrentChecklist)
                            .Include(s => s.CurrentChecklist.Procedures)
                            .Where(s => s.Laboratory.Techinicians.Contains(currentUser) && s.TechinicianId == currentUser.Id)
                            .FirstOrDefaultAsync(m => m.SchedulesId == id);

                    if (schedulesUpdate == null)
                    {
                        return NotFound();
                    }

                    if (ProcedureId.Count < schedulesUpdate.CurrentChecklist.Procedures.Count)
                        ModelState.AddModelError("Result", "All checkboxes must be filled in");

                    if (ModelState.IsValid)
                    {
                        if (schedules.Result == TestResult.Other)
                            schedulesUpdate.Description = schedules.Description;

                        schedulesUpdate.Result = schedules.Result;
                        _context.Update(schedulesUpdate);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return View(schedulesUpdate);
                    }


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
            var currentUser = await userManager.GetUserAsync(User);
            var schedules = await _context.Schedules
                .Include(s => s.Client)
                .Include(s => s.Laboratory)
                .Include(s => s.Techinician)
                .Include(s => s.TestType)
                .Include(s => s.CurrentChecklist)
                .Where(s => s.ClientId == currentUser.Id)
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
            var currentUser = await userManager.GetUserAsync(User);
            var schedules = await _context.Schedules.Where(s => s.ClientId == currentUser.Id && s.SchedulesId == id).FirstAsync();
            if (schedules == null)
                return NotFound();

            if (schedules.AppointmentTime.Subtract(DateTime.Now).TotalHours > 24 && schedules.Result == null)
            {
                schedules.Result = TestResult.Unrealized;
                _context.Schedules.Update(schedules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
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
                .Include(s => s.CurrentChecklist)
                .Where(s => s.Laboratory.Techinicians.Contains(currentUser))
                .FirstOrDefaultAsync(s => s.SchedulesId == SchedulesId);

            test.Techinician = currentUser;
            test.TechinicianId = currentUser.Id;

            _context.Schedules.Update(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Schedules/Statistics
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Statistics()
        {
            var stats = new StatisticsViewModel
            {
                all = await getStats(),
                FilterDay = DateTime.Now,
                OnDay = await getStats(DateTime.Now),
                OnWeek = await getStats(FirstDayOfWeek(DateTime.Now), LastDayOfWeek(DateTime.Now)),
                FilterWeekDay1 = FirstDayOfWeek(DateTime.Now),
                FilterWeekDay2 = LastDayOfWeek(DateTime.Now),
                OnMonth = await getStats(FirstDayOfMonth(DateTime.Now), LastDayOfMonth(DateTime.Now)),
                Month = DateTime.Now.Year + "-" + DateTime.Now.Month,
            };

            return View(stats);
        }

        // POST: Schedules/Statistics
        [Authorize(Roles = ("Admin"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Statistics(DateTime Date, String Week, String Month)
        {
            var weekYear = int.Parse(Week.Substring(0, 4));
            var weekNumber = int.Parse(Week.Substring(6));
            var weekFirstDate = FirstDateOfWeekISO8601(weekYear, weekNumber);

            var firstDayOfMonth = new DateTime(int.Parse(Month.Substring(0, 4)), int.Parse(Month.Substring(5)), 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var stats = new StatisticsViewModel
            {
                all = await getStats(),
                FilterDay = Date,
                OnDay = await getStats(Date),
                OnWeek = await getStats(weekFirstDate, LastDayOfWeek(weekFirstDate)),
                FilterWeekDay1 = weekFirstDate,
                FilterWeekDay2 = LastDayOfWeek(weekFirstDate),
                OnMonth = await getStats(firstDayOfMonth, lastDayOfMonth),
                Month = Month,
            };

            return View(stats);
        }

        //============================================= UTILS ========================================

        private async Task<AllTests> getStats()
        {
            return new AllTests
            {
                TotalTests = await _context.Schedules.Where(s => s.Result != null && s.Result != TestResult.Unrealized).CountAsync(),
                TotalPositiveTests = await _context.Schedules.Where(s => s.Result == TestResult.Positive).CountAsync(),
                TotalNegativeTests = await _context.Schedules.Where(s => s.Result == TestResult.Negative).CountAsync(),
                TotalInconclusiveTests = await _context.Schedules.Where(s => s.Result == TestResult.Inconclusive).CountAsync(),
            };
        }

        private async Task<AllTests> getStats(DateTime date)
        {
            return new AllTests
            {

                TotalTests = await _context.Schedules.Where(s => s.Result != null && s.Result != TestResult.Unrealized && s.AppointmentTime.Date == date.Date).CountAsync(),
                TotalPositiveTests = await _context.Schedules.Where(s => s.Result == TestResult.Positive && s.AppointmentTime.Date == date.Date).CountAsync(),
                TotalNegativeTests = await _context.Schedules.Where(s => s.Result == TestResult.Negative && s.AppointmentTime.Date == date.Date).CountAsync(),
                TotalInconclusiveTests = await _context.Schedules.Where(s => s.Result == TestResult.Inconclusive && s.AppointmentTime.Date == date.Date).CountAsync(),
            };
        }

        private async Task<AllTests> getStats(DateTime date1, DateTime date2)
        {
            return new AllTests
            {
                TotalTests = await _context.Schedules.Where(s => s.Result != null && s.Result != TestResult.Unrealized && s.AppointmentTime.Date >= date1.Date && s.AppointmentTime.Date <= date2.Date).CountAsync(),
                TotalPositiveTests = await _context.Schedules.Where(s => s.Result == TestResult.Positive && s.AppointmentTime.Date >= date1.Date && s.AppointmentTime.Date <= date2.Date).CountAsync(),
                TotalNegativeTests = await _context.Schedules.Where(s => s.Result == TestResult.Negative && s.AppointmentTime.Date >= date1.Date && s.AppointmentTime.Date <= date2.Date).CountAsync(),
                TotalInconclusiveTests = await _context.Schedules.Where(s => s.Result == TestResult.Inconclusive && s.AppointmentTime.Date >= date1.Date && s.AppointmentTime.Date <= date2.Date).CountAsync(),
            };
        }

        private DateTime FirstDayOfWeek(DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek - 1;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        private DateTime LastDayOfWeek(DateTime dt) => FirstDayOfWeek(dt).AddDays(6);

        private DateTime FirstDayOfMonth(DateTime dt) => new DateTime(dt.Year, dt.Month, 1);

        private DateTime LastDayOfMonth(DateTime dt) => FirstDayOfMonth(dt).AddMonths(1).AddDays(-1);

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }
    }
}
