using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages;

public class ContactModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public ContactModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Notification Notification { get; set; } = default!;

    public int CartCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Obtener contador del carrito
        CartCount = await ObtenerContadorCarritoAsync();
        ViewData["CartCount"] = CartCount;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Verificar que el usuario esté autenticado
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            TempData["ErrorMessage"] = "Debes iniciar sesión para enviar un mensaje.";
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Por favor, corrige los errores en el formulario.";
            return Page();
        }

        try
        {
            // Obtener el usuario actual
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "No se pudo identificar al usuario.";
                return RedirectToPage("/Account/Login");
            }

            // Crear la notificación
            var notification = new Notification
            {
                Title = Notification.Title.Trim(),
                Message = Notification.Message.Trim(),
                UserId = currentUser.Id,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Tu mensaje ha sido enviado exitosamente! Te responderemos pronto.";

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ocurrió un error al enviar el mensaje: {ex.Message}";
            return Page();
        }
    }

    private async Task<int> ObtenerContadorCarritoAsync()
    {
        return await _context.Carts.CountAsync();
    }
}
