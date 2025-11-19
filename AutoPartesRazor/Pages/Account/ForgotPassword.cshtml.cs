<<<<<<< HEAD
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using AutoPartesRazor.Models;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

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

<<<<<<< HEAD
    [TempData]
=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    public string Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Email);

        if (user == null)
        {
<<<<<<< HEAD
            Message = "El email ingresado no esta registrado.";
=======
            Message = "Si el correo está registrado, recibirá un email.";
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
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
<<<<<<< HEAD
            <h3>AutopartesRazor S.A - Recuperación de Contraseña</h3>
=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            <p>Has solicitado recuperar tu contraseña.</p>
            <p>Tu contraseña provisoria para ingresar es:</p>
            <h2>{newPassword}</h2>
            <p>Una vez que ingreses, puedes cambiar la contraseña desde tu perfil.</p>
        ";

        await _emailSender.SendEmailAsync(Email, subject, body);

<<<<<<< HEAD
        Message = "Se envió un email con su nueva contraseña, revisa tu correo.";

        return Redirect("./Login");
=======
        Message = "Si el correo está registrado, recibirá un email con su nueva contraseña.";

        return Page();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}


