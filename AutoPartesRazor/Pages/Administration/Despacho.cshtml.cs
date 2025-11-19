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

    public async Task<IActionResult> OnPostDespacharAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null)
            return NotFound();
        pedido.Status = "Despachado";
        await _context.SaveChangesAsync();
        TempData["DespachoOK"] = true;
        return RedirectToPage();
    }
}
