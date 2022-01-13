using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PWEBLabTestesOnline.Data;
using PWEBLabTestesOnline.Models;

namespace PWEBLabTestesOnline.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ApplicationDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._context = _context;
        }

        public string Role { get; set; }
        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);


            var AllRolesUser = await _userManager.GetRolesAsync(user);
            var roleUser = _context.Roles.Where(r => r.Name == AllRolesUser.FirstOrDefault()).FirstOrDefault();

            Email = email;
            Role = roleUser.Name;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string UpdateRoleButton)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if(UpdateRoleButton != "Save")
            {
                var AllRolesUser = await _userManager.GetRolesAsync(user);
                var roleUser = _context.Roles.Where(r => r.Name == AllRolesUser.FirstOrDefault()).FirstOrDefault();

                var newRole = _context.Roles.Where(r => r.Name == "Client").FirstOrDefault();
                if ("Client" != roleUser.Name)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleUser.Name);
                    await _userManager.AddToRoleAsync(user, newRole.Name);
                }

                var lab_of_user = _context.Laboratories.Where(l => l.Techinicians.Contains(user)).First();
                lab_of_user.Techinicians.Remove(user);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if(Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
                //var setFirstNameResult = await _userManager.set SetFirstNameAsync(user, Input.FirstName);
                //if (!setFirstNameResult.Succeeded)
                //{
                //    StatusMessage = "Unexpected error when trying to set first name.";
                //    return RedirectToPage();
                //}
            }

            if(Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
                //var setLastNameResult = await _userManager.SetLastNameAsync(user,Input.LastName);
                //if (!setLastNameResult.Succeeded)
                //{
                //    StatusMessage = "Unexpected error when trying to set last name.";
                //    return RedirectToPage();
                //}
            }
            
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
