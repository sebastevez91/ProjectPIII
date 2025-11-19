using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Redirige a la página de logout confirmación o al Index
                return RedirectToPage("/Index");
            }
        }

        public IActionResult OnGet()
        {
            // Si el usuario accede por GET, simplemente muestra la página
            return Page();
        }
    }
}
