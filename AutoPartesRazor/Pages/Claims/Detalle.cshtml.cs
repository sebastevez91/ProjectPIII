using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Claims;

[Authorize]
public class DetalleModel : PageModel
{
    private readonly IClaimService _reclamoService;  // ✅ IClaimService
    private readonly UserManager<User> _userManager;

    public DetalleModel(IClaimService reclamoService, UserManager<User> userManager)  // ✅ IClaimService
    {
        _reclamoService = reclamoService;
        _userManager = userManager;
    }

    [BindProperty]
    public ReclamoDetalleViewModel ViewModel { get; set; } = new ReclamoDetalleViewModel();  // ✅ ReclamoDetalleViewModel

    [TempData]
    public string? MensajeExito { get; set; }

    [TempData]
    public string? MensajeError { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var esAdministrador = User.IsInRole("Admin");

        // Verificar permisos
        var puedeVer = await _reclamoService.UsuarioPuedeVerReclamoAsync(id, usuario.Id, esAdministrador);
        if (!puedeVer)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var reclamo = await _reclamoService.ObtenerReclamoPorIdAsync(id);
        if (reclamo == null)
        {
            return NotFound();
        }

        var mensajes = await _reclamoService.ObtenerMensajesDeReclamoAsync(id);

        // Marcar mensajes como leídos
        await _reclamoService.MarcarMensajesComoLeidosAsync(id, esAdministrador);

        ViewModel = new ReclamoDetalleViewModel
        {
            Reclamo = reclamo,
            Mensajes = mensajes,
            EsAdministrador = esAdministrador,
            PuedeResponder = reclamo.Estado != StatusClaim.Cerrado
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Obtener el ID de la ruta
        int id = 0;
        if (RouteData.Values.ContainsKey("id"))
        {
            id = Convert.ToInt32(RouteData.Values["id"]);
        }

        // LIMPIAR ERRORES DE VALIDACIÓN QUE NO NECESITAMOS
        var keysToRemove = ModelState.Keys
            .Where(k => k.StartsWith("ViewModel.Reclamo") || k.StartsWith("ViewModel.Mensajes"))
            .ToList();

        foreach (var key in keysToRemove)
        {
            ModelState.Remove(key);
        }

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var esAdministrador = User.IsInRole("Admin");

        // Verificar permisos
        var puedeVer = await _reclamoService.UsuarioPuedeVerReclamoAsync(id, usuario.Id, esAdministrador);
        if (!puedeVer)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        // Validar el mensaje
        if (string.IsNullOrWhiteSpace(ViewModel.NuevoMensaje))
        {
            ModelState.AddModelError("ViewModel.NuevoMensaje", "El mensaje no puede estar vacío.");
        }

        if (!ModelState.IsValid)
        {
            System.Diagnostics.Debug.WriteLine("ModelState NO válido, recargando página");

            // Recargar datos
            var reclamo = await _reclamoService.ObtenerReclamoPorIdAsync(id);
            var mensajes = await _reclamoService.ObtenerMensajesDeReclamoAsync(id);

            ViewModel.Reclamo = reclamo!;
            ViewModel.Mensajes = mensajes;
            ViewModel.EsAdministrador = esAdministrador;
            ViewModel.PuedeResponder = reclamo!.Estado != StatusClaim.Cerrado;

            return Page();
        }

        try
        {

            await _reclamoService.AgregarMensajeAsync(id, usuario.Id, ViewModel.NuevoMensaje, esAdministrador);

            MensajeExito = "Mensaje enviado correctamente.";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {

            MensajeError = $"Error al enviar el mensaje: {ex.Message}";

            // Recargar datos
            var reclamo = await _reclamoService.ObtenerReclamoPorIdAsync(id);
            var mensajes = await _reclamoService.ObtenerMensajesDeReclamoAsync(id);

            ViewModel.Reclamo = reclamo!;
            ViewModel.Mensajes = mensajes;
            ViewModel.EsAdministrador = esAdministrador;
            ViewModel.PuedeResponder = reclamo!.Estado != StatusClaim.Cerrado;

            return Page();
        }
    }
}
