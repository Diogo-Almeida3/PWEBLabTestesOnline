using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;
using System.Threading.Tasks;

namespace PWEBLabTestesOnline.Controllers
{
    public class ClientsController : Controller
    {

        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> userManager;

        public ClientsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> ClientList()
        {
            var ListOfUsers = await userManager.GetUsersInRoleAsync("Client");

            ClientViewModel clientViewModel = new ClientViewModel
            {
                Users = ListOfUsers
            };

            return View(clientViewModel);
        }

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
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FirstName,LastName,Email,PhoneNumber")] ApplicationUser user)
        {
            
            user.Id = id;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailExists(id).Result)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ClientList));
            }
            return View(user);
        }



        private Task<bool> EmailExists(string id)
        {
            return _context.Users.AnyAsync(e => e.Id == id);
        }
    }
    
}
