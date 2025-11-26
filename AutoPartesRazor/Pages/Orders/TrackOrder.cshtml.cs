using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoPartesRazor.Pages.Orders;

[Authorize]
public class TrackOrderModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public TrackOrderModel(AutoPartesRazorContext context) => _context = context;

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public Order? Pedido { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var query = _context.Orders
            .Include(o => o.Items)!
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            // IMPORTANTE: Incluir OrderEvents y ordenarlos para el Timeline
            .Include(o => o.OrderEvents.OrderByDescending(e => e.Timestamp))
            .Where(m => m.Id == Id);

        if (!isAdmin)
        {
            if (userId == null) return Forbid();
            query = query.Where(o => o.UserId == userId);
        }

        Pedido = await query.FirstOrDefaultAsync();

        if (Pedido == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Lógica de Calificación
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var pedido = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == Id);

        if (pedido == null || pedido.UserId != userId || pedido.Status != "Entregado")
        {
            StatusMessage = "Error: El pedido no puede ser calificado en este momento.";
            return RedirectToPage(new { id = Id });
        }

        if (pedido.ClientConfirmed != true)
        {
            StatusMessage = "Error: Primero debes confirmar la recepción del pedido para calificarlo.";
            return RedirectToPage(new { id = Id });
        }

        if (Request.Form.TryGetValue("Calificacion", out var calif) && int.TryParse(calif, out int nota) && nota >= 1 && nota <= 5)
        {
            pedido.Calificacion = nota;
            await _context.SaveChangesAsync();
            StatusMessage = $"¡Gracias! Has calificado el pedido #{Id} con {nota} estrellas. ?";
        }
        else
        {
            StatusMessage = "Error: Calificación no válida.";
        }

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostConfirmReceptionAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == Id && o.UserId == userId);

        if (order == null)
        {
            StatusMessage = "Error: Pedido no encontrado o no autorizado.";
            return RedirectToPage(new { id = Id });
        }

        if (order.Status == "Entregado" && !order.ClientConfirmed)
        {
            order.ClientConfirmed = true;
            order.UpdatedAt = DateTime.Now;
            order.DeliveryDate = DateTime.Now;

            await _context.SaveChangesAsync();

            StatusMessage = $"¡Gracias, {order.User.FullName ?? "Cliente"}! Has confirmado la recepción del pedido #{order.Id} exitosamente. ? Ahora puedes calificar tu experiencia.";
        }
        else if (order.ClientConfirmed)
        {
            StatusMessage = $"El pedido #{order.Id} ya había sido confirmado previamente. ??";
        }
        else
        {
            StatusMessage = "El pedido no está en el estado 'Entregado' o no puede ser confirmado. ??";
        }

        return RedirectToPage(new { id = Id });
    }
}