using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class DespachoModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public DespachoModel(AutoPartesRazorContext context) => _context = context;

    public IList<Order> PedidosPendientes { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        PedidosPendientes = await _context.Orders
            .Where(o => o.Status == "Pending")
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // Método Modificado: Ahora el primer paso es "Preparando"
    public async Task<IActionResult> OnPostPrepararAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);

        if (pedido == null)
            return NotFound();

        // El nuevo estado es "Preparando"
        pedido.Status = "Preparando";

        // Aquí podrías integrar el registro en OrderEvent si sigues la idea del Timeline.
        // Por ahora, solo guardamos el cambio de estado.
        await _context.SaveChangesAsync();

        TempData["DespachoOK"] = true;

        // Recargamos la lista para que el pedido despachado desaparezca.
        await OnGetAsync();

        return Page();
    }
}