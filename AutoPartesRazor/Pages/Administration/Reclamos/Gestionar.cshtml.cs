using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Admin.Reclamos;

[Authorize(Roles = "Admin")]
public class GestionarModel : PageModel
{
    private readonly IClaimService _reclamoService;
    private readonly UserManager<User> _userManager;

    public GestionarModel(IClaimService reclamoService, UserManager<User> userManager)
    {
        _reclamoService = reclamoService;
        _userManager = userManager;
    }

    [BindProperty]
    public GestionarReclamoViewModel ViewModel { get; set; } = new GestionarReclamoViewModel();

    [TempData]
    public string? MensajeExito { get; set; }

    [TempData]
    public string? MensajeError { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var reclamo = await _reclamoService.ObtenerReclamoPorIdAsync(id);
        if (reclamo == null)
        {
            return NotFound();
        }

        var mensajes = await _reclamoService.ObtenerMensajesDeReclamoAsync(id);

        // Marcar mensajes del cliente como leídos
        await _reclamoService.MarcarMensajesComoLeidosAsync(id, true);

        // Obtener administradores disponibles
        var administradores = await _userManager.GetUsersInRoleAsync("Admin");

        ViewModel = new GestionarReclamoViewModel
        {
            Reclamo = reclamo,
            Mensajes = mensajes,
            NuevoEstado = reclamo.Estado,
            NuevaUrgencia = reclamo.NivelUrgencia,
            AdministradorAsignadoId = reclamo.AdministradorAsignadoId,
            AdministradoresDisponibles = administradores.ToList()
        };

        return Page();
    }

    public async Task<IActionResult> OnPostResponderAsync(int reclamoId)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.Respuesta))
        {
            ModelState.AddModelError("ViewModel.Respuesta", "La respuesta no puede estar vacía.");
            return await RecargarPaginaAsync(reclamoId);
        }

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            await _reclamoService.AgregarMensajeAsync(reclamoId, usuario.Id, ViewModel.Respuesta, true);
            MensajeExito = "Respuesta enviada correctamente.";
            return RedirectToPage(new { id = reclamoId });
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al enviar la respuesta: {ex.Message}";
            return await RecargarPaginaAsync(reclamoId);
        }
    }

    public async Task<IActionResult> OnPostActualizarEstadoAsync(int reclamoId)
    {
        try
        {
            var actualizado = await _reclamoService.ActualizarEstadoReclamoAsync(reclamoId, ViewModel.NuevoEstado);

            if (actualizado)
            {
                MensajeExito = $"Estado actualizado a '{ViewModel.NuevoEstado}' correctamente.";
            }
            else
            {
                MensajeError = "No se pudo actualizar el estado del reclamo.";
            }

            return RedirectToPage(new { id = reclamoId });
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al actualizar el estado: {ex.Message}";
            return RedirectToPage(new { id = reclamoId });
        }
    }

    public async Task<IActionResult> OnPostActualizarUrgenciaAsync(int reclamoId)
    {
        try
        {
            var actualizado = await _reclamoService.ActualizarUrgenciaReclamoAsync(reclamoId, ViewModel.NuevaUrgencia);

            if (actualizado)
            {
                MensajeExito = $"Urgencia actualizada a '{ViewModel.NuevaUrgencia}' correctamente.";
            }
            else
            {
                MensajeError = "No se pudo actualizar la urgencia del reclamo.";
            }

            return RedirectToPage(new { id = reclamoId });
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al actualizar la urgencia: {ex.Message}";
            return RedirectToPage(new { id = reclamoId });
        }
    }

    public async Task<IActionResult> OnPostAsignarAdminAsync(int reclamoId)
    {
        try
        {
            if (string.IsNullOrEmpty(ViewModel.AdministradorAsignadoId))
            {
                MensajeError = "Debe seleccionar un administrador.";
                return RedirectToPage(new { id = reclamoId });
            }

            var actualizado = await _reclamoService.AsignarAdministradorAsync(reclamoId, ViewModel.AdministradorAsignadoId);

            if (actualizado)
            {
                MensajeExito = "Administrador asignado correctamente.";
            }
            else
            {
                MensajeError = "No se pudo asignar el administrador.";
            }

            return RedirectToPage(new { id = reclamoId });
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al asignar administrador: {ex.Message}";
            return RedirectToPage(new { id = reclamoId });
        }
    }

    public async Task<IActionResult> OnPostCerrarAsync(int reclamoId)
    {
        try
        {
            var cerrado = await _reclamoService.CerrarReclamoAsync(reclamoId);

            if (cerrado)
            {
                MensajeExito = "Reclamo cerrado correctamente.";
            }
            else
            {
                MensajeError = "No se pudo cerrar el reclamo. Debe estar en estado 'Resuelto' primero.";
            }

            return RedirectToPage(new { id = reclamoId });
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cerrar el reclamo: {ex.Message}";
            return RedirectToPage(new { id = reclamoId });
        }
    }

    private async Task<IActionResult> RecargarPaginaAsync(int reclamoId)
    {
        var reclamo = await _reclamoService.ObtenerReclamoPorIdAsync(reclamoId);
        if (reclamo == null)
        {
            return NotFound();
        }

        var mensajes = await _reclamoService.ObtenerMensajesDeReclamoAsync(reclamoId);
        var administradores = await _userManager.GetUsersInRoleAsync("Admin");

        ViewModel.Reclamo = reclamo;
        ViewModel.Mensajes = mensajes;
        ViewModel.AdministradoresDisponibles = administradores.ToList();

        return Page();
    }
}
