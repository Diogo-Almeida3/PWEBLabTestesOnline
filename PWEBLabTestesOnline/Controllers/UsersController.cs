using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWEBLabTestesOnline.Controllers
{
    [Authorize(Roles = ("Admin"))]
    public class UsersController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }


        public async Task<IActionResult> Index()
        {
            var usersViewModel = new List<UsersViewModel>();

            var RolesManager = roleManager.Roles.Where(r => r.Name == "Manager").First();
            var RolesTechinician = roleManager.Roles.Where(r => r.Name == "Techinician").First();
            if (RolesManager == null || RolesTechinician == null)
                return View(usersViewModel);

            var managers = await userManager.GetUsersInRoleAsync(RolesManager.Name);
            var techinicians = await userManager.GetUsersInRoleAsync(RolesTechinician.Name);
            var labs = _context.Laboratories;

            foreach(var user in managers)
            {
                usersViewModel.Add(new UsersViewModel
                {
                    ApplicationUser = user,
                    Role = "Manager",
                    Laboratories = labs.Where(l => l.ManagerId == user.Id || l.Techinicians.Contains(user)).ToList()
                });
            }

            foreach (var user in techinicians)
            {
                usersViewModel.Add(new UsersViewModel
                {
                    ApplicationUser = user,
                    Role = "Techinician",
                    Laboratories = labs.Where(l => l.ManagerId == user.Id || l.Techinicians.Contains(user)).ToList()
                });
            }
            return View(usersViewModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Procedures/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roleName = await userManager.GetRolesAsync(user);
            var role = await _context.Roles.Where(r => r.Name == roleName.FirstOrDefault()).FirstOrDefaultAsync();
            ViewData["Roles"] = new SelectList(roleManager.Roles.ToList(), "Id", "Name", role.Id);
            return View(user);
        }

        // POST: Procedures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user, string Role)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {       
                    var user2update = await _context.Users.FindAsync(id);
                    user2update.FirstName = user.FirstName;
                    user2update.LastName = user.LastName;
                    user2update.PhoneNumber = user.PhoneNumber;

                    var AllRolesUser = await userManager.GetRolesAsync(user2update);
                    var roleUser = await _context.Roles.Where(r => r.Name == AllRolesUser.FirstOrDefault()).FirstOrDefaultAsync();

                    var newRole = _context.Roles.Where(r => r.Id == Role).FirstOrDefault();
                    if (Role != roleUser.Id)
                    {
                        await userManager.RemoveFromRoleAsync(user2update, roleUser.Name);
                        await userManager.AddToRoleAsync(user2update, newRole.Name);
                    }

                    _context.Update(user2update);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(user.Id))
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
            return View(user);
        }

        private bool UsersExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userViewModel = new UsersViewModel();
            var labs = _context.Laboratories;

            userViewModel.ApplicationUser = await _context.Users.FindAsync(id);
            userViewModel.Laboratories = labs.Where(l => l.ManagerId == id).ToList();

            if (userViewModel == null)
            {
                return NotFound();
            }

            return View(userViewModel);
        }
    }
}
