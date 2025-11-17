using AutoPartesRazor.Data;
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;
        private readonly IUserService _userService;

        public RegisterModel(AutoPartesRazorContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [BindProperty]
        public RegisterViewModels ViewModels { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || ViewModels == null)
            {
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }


            User user = new User
            {
                FullName = ViewModels.FullName,
                UserName = ViewModels.Email,
                Email = ViewModels.Email,
            };

            var result = await _userService.AddUserAsync(user, ViewModels.Password);

            if (result.Succeeded)
            {
                return RedirectToPage("./Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return Page();
            }

        }
    }
}
