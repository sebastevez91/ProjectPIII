using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Reclamos;

[Authorize]
public class CrearModel : PageModel
{
    private readonly IClaimService _reclamoService;
    private readonly UserManager<User> _userManager;

    public CrearModel(IClaimService reclamoService, UserManager<User> userManager)
    {
        _reclamoService = reclamoService;
        _userManager = userManager;
    }

    [BindProperty]
    public CrearReclamoViewModel Input { get; set; } = new CrearReclamoViewModel();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            var reclamo = await _reclamoService.CrearReclamoAsync(
                usuario.Id,
                Input.Asunto,
                Input.Descripcion,
                Input.NivelUrgencia
            );

            TempData["MensajeExito"] = $"Reclamo creado exitosamente. Número de ticket: {reclamo.NumeroTicket}";
            return RedirectToPage("./Detalle", new { id = reclamo.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error al crear el reclamo: {ex.Message}");
            return Page();
        }
    }
}
