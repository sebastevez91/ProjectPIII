using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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

    [TempData]
    public string Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Email);

        if (user == null)
        {
            Message = "El email ingresado no esta registrado.";
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
            <h3>AutopartesRazor S.A - Recuperación de Contraseña</h3>
            <p>Has solicitado recuperar tu contraseña.</p>
            <p>Tu contraseña provisoria para ingresar es:</p>
            <h2>{newPassword}</h2>
            <p>Una vez que ingreses, puedes cambiar la contraseña desde tu perfil.</p>
        ";

        await _emailSender.SendEmailAsync(Email, subject, body);

        Message = "Se envió un email con su nueva contraseña, revisa tu correo.";

        return Redirect("./Login");
    }
}


