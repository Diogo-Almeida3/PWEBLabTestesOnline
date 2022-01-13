using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PWEBLabTestesOnline.Controllers
{
    [Authorize(Roles = ("Admin"))]
    public class ClientsController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public ClientsController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }


        public async Task<IActionResult> Index()
        {
            var RoleClient = roleManager.Roles.Where(r => r.Name == "Client").FirstOrDefault();
            if (RoleClient == null) 
                return NotFound();

            var clientes = await userManager.GetUsersInRoleAsync(RoleClient.Name);

            return View(clientes.ToList());
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string IdUser)
        {
            var user = await _context.Users.FindAsync(IdUser);
            user.EmailConfirmed = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string IdUser)
        {
            var user = await _context.Users.FindAsync(IdUser);
            user.EmailConfirmed = false;
            _context.Users.Update(user);
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
    }
}
