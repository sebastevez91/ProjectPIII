using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordModel(UserManager<User> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Email);

        if (user == null)
        {
            Message = "Si el correo está registrado, recibirá un email.";
            return Page();
        }

        // Generar contraseña temporal
        var random = new Random();
        var newPassword = random.Next(100000, 999999).ToString();

        // Resetear la contraseña correctamente con Identity
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (!resetResult.Succeeded)
        {
            foreach (var error in resetResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        // Enviar email al usuario
        var subject = "Recuperación de contraseña";
        var body = $@"
            <p>Has solicitado recuperar tu contraseña.</p>
            <p>Tu contraseña provisoria para ingresar es:</p>
            <h2>{newPassword}</h2>
            <p>Una vez que ingreses, puedes cambiar la contraseña desde tu perfil.</p>
        ";

        await _emailSender.SendEmailAsync(Email, subject, body);

        Message = "Si el correo está registrado, recibirá un email con su nueva contraseña.";

        return Page();
    }
}


