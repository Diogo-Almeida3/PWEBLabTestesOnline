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
    [Authorize(Roles=("Admin,Manager"))]
    public class LaboratoriesController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public LaboratoriesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._context = _context;
        }

        // GET: Laboratories
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.Laboratories.Include(p => p.Manager);
                return View(await applicationDbContext.ToListAsync());
            } else
            {
                var applicationDbContext = _context.Laboratories.Where(lab => lab.ManagerId == userManager.GetUserId(User)).Include(p => p.Manager);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: Laboratories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            if (User.IsInRole("Admin"))
            {
                var laboratories = await _context.Laboratories
                .Include(p => p.Manager)
                .Include(p => p.Techinicians)
                .FirstOrDefaultAsync(m => m.LaboratoriesId == id);
                return View(laboratories);
            } 
            else
            {
                var laboratories = await _context.Laboratories
                .Where(lab => lab.ManagerId == userManager.GetUserId(User))
                .Include(p => p.Manager)
                .FirstOrDefaultAsync(m => m.LaboratoriesId == id);
                if(laboratories == null)
                    return NotFound("Could not find or do not have permissions to see details this lab");
                return View(laboratories);
            }

        }

        // GET: Laboratories/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Admin"))
            {
                var RoleManager = roleManager.Roles.Where(r => r.Name == "Manager").FirstOrDefault();
                if (RoleManager == null)
                    return NotFound();

                var managers = await userManager.GetUsersInRoleAsync(RoleManager.Name);
                ViewData["ManagerId"] = new SelectList(managers, "Id", "Email");
            }
            else
            {
                List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
                var currentUser = await userManager.GetUserAsync(User);
                applicationUsers.Add(currentUser);
                ViewData["ManagerId"] = new SelectList(applicationUsers, "Id", "Email");
            }
            return View();
        }

        // POST: Laboratories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LaboratoriesId,LaboratoriesName,Location,PhoneNumber,ManagerId")] Laboratories laboratories)
        {
            if (User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    _context.Add(laboratories);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                var RoleManager = roleManager.Roles.Where(r => r.Name == "Manager").FirstOrDefault();
                if (RoleManager == null)
                    return NotFound();
                var managers = await userManager.GetUsersInRoleAsync(RoleManager.Name);
                ViewData["ManagerId"] = new SelectList(managers, "Id", "Email");
            }
            else
            {
                laboratories.Manager = await userManager.GetUserAsync(User);
                laboratories.ManagerId = laboratories.Manager.Id;
                if (ModelState.ErrorCount <= 1)
                {
                    _context.Add(laboratories);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(laboratories);
        }

        // GET: Laboratories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //ViewData["Techinicians"] = new MultiSelectList(AllClients, "Id", "Email");
            if (User.IsInRole("Admin"))
            {
                //Get lab
                var laboratories = await _context.Laboratories
                .Include(p => p.Manager)
                .Include(p => p.Techinicians)
                .FirstOrDefaultAsync(m => m.LaboratoriesId == id);
                if (laboratories == null)
                {
                    return NotFound();
                }

                // Get managers users
                var RoleManager = roleManager.Roles.Where(r => r.Name == "Manager").FirstOrDefault();
                if (RoleManager == null)
                    return NotFound();
                var managers = await userManager.GetUsersInRoleAsync(RoleManager.Name);
                ViewData["ManagerId"] = new SelectList(managers, "Id", "Email");
                return View(laboratories);
            }
            else
            {
                try
                {
                    var laboratories = _context.Laboratories
                                        .Where(lab => lab.ManagerId == userManager.GetUserId(User) && lab.LaboratoriesId == id)
                                        .Include(p => p.Manager)
                                        .Include(p => p.Techinicians)
                                        .First();
                    return View(laboratories);
                }
                catch (Exception ex)
                {
                    return NotFound("Could not find or do not have permissions to edit this lab");
                }

            }
            
        }

        // POST: Laboratories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LaboratoriesId,LaboratoriesName,Location,PhoneNumber,ManagerId")] Laboratories laboratories)
        {
            if (id != laboratories.LaboratoriesId)
            {
                return NotFound();
            }

            if(User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(laboratories);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LaboratoriesExists(laboratories.LaboratoriesId))
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
                var RoleManager = roleManager.Roles.Where(r => r.Name == "Manager").FirstOrDefault();
                if (RoleManager == null)
                    return NotFound();
                var managers = await userManager.GetUsersInRoleAsync(RoleManager.Name);
                ViewData["ManagerId"] = new SelectList(managers, "Id", "Email");
                return View(laboratories);
            }
            else
            {
                if (ModelState.ErrorCount<=1)
                {
                    try
                    {
                        laboratories.Manager = await userManager.GetUserAsync(User);
                        laboratories.ManagerId = laboratories.Manager.Id;
                        _context.Update(laboratories);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LaboratoriesExists(laboratories.LaboratoriesId))
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
                return View(laboratories);
            }
            
        }

        // GET: Laboratories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if(User.IsInRole("Admin"))
            {
                var laboratories = await _context.Laboratories
                                .Include(m => m.Manager)
                                .FirstOrDefaultAsync(m => m.LaboratoriesId == id);
                if (laboratories == null)
                {
                    return NotFound();
                }

                return View(laboratories);
            }
            else
            {
                var currentUser = await userManager.GetUserAsync(User);
                var laboratories = await _context.Laboratories
                                .Include(m => m.Manager)
                                .FirstOrDefaultAsync(m => m.LaboratoriesId == id && m.ManagerId == currentUser.Id);
                if (laboratories == null)
                {
                    return NotFound("Could not find this item that belongs to delete");
                }

                return View(laboratories);
            } 
        }

        // POST: Laboratories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var laboratories = await _context.Laboratories.FindAsync(id);
            _context.Laboratories.Remove(laboratories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LaboratoriesExists(int id)
        {
            return _context.Laboratories.Any(e => e.LaboratoriesId == id);
        }

        // GET
        public IActionResult Techinicians(int id)
        {
            if (!LaboratoriesExists(id))
            {
                return NotFound();
            }
            return View(id);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Techinicians(int id, string email,string action)
        {
            if (!LaboratoriesExists(id))
            {
                return NotFound();
            }

            var laboratory = _context.Laboratories.Include(m => m.Techinicians).First(lab => lab.LaboratoriesId == id);
            ApplicationUser user2action = null;
            
            try
            {
                user2action = _context.Users.First(u => u.Email == email);
            } catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Could not find a user with this email address";
                return View(id);
            }

            
            if (user2action == null)
            {
                ViewData["ErrorMessage"] = "Could not find a user with this email address";
                return View(id);
            }


            if (User.IsInRole("Admin"))
            {
                if(action == "Add") // Adiciona técnico a um laboratório
                {
                    await userManager.RemoveFromRoleAsync(user2action, "Client");
                    await userManager.AddToRoleAsync(user2action, "Techinician");
                    laboratory.Techinicians.Add(user2action);
                }
                else // Remove técnico de um laboratório
                {              
                    if(!laboratory.Techinicians.Remove(user2action))
                    {
                        ViewData["ErrorMessage"] = "Unable to remove this technician or a technician with this email address could not be found";
                        return View(id);
                    }
                    await userManager.RemoveFromRoleAsync(user2action, "Techinician");
                    await userManager.AddToRoleAsync(user2action, "Client");
                }        
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            else
            {
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser.Id == laboratory.ManagerId) // Este utilizador é o manager deste laboratório, por isso pode editar
                {
                    if (action == "Add") // Pertende adicionar um novo técnico
                    {
                        var Clients = await userManager.GetUsersInRoleAsync("Client");
                        if (Clients.First(c => c.Id == user2action.Id) != null)// Este utilizador é um cliente, por isso pode ser adicionado
                        {
                            await userManager.RemoveFromRoleAsync(user2action, "Client");
                            await userManager.AddToRoleAsync(user2action, "Techinician");
                            laboratory.Techinicians.Add(user2action);
                        }
                        else // Mensagem de erro para informar utilizador
                        {
                            ViewData["ErrorMessage"] = "It is not possible to add this user because he is already a technician in a laboratory";
                            return View(id);
                        }        
                    }
                    else // Pertende remover um técnico
                    {
                        if (!laboratory.Techinicians.Remove(user2action))
                        {
                            ViewData["ErrorMessage"] = "Unable to remove this technician or a technician with this email address could not be found";
                            return View(id);
                        }
                        await userManager.RemoveFromRoleAsync(user2action, "Techinician");
                        await userManager.AddToRoleAsync(user2action, "Client");
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Edit), new { id = id });
                }
                else
                {
                    return NotFound("You don't have permissions to do this");
                }
            }
            return RedirectToAction(nameof(Edit), new { id = id });
        }
    }
}


