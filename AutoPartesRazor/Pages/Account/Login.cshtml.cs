using AutoPartesRazor.Interfaces;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;

    public LoginModel(IUserService service)
    {
        _userService = service;
    }

    [BindProperty]
    public LoginViewModel LoginInput { get; set; }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await _userService.LoginAsync(LoginInput);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }
        }
        return Page();
    }
    public async Task<IActionResult> OnGetLogout()
    {
        await _userService.LogoutAsync();
        return RedirectToPage("/Index");
    }

}
