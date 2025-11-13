using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class TrackOrderModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public TrackOrderModel(AutoPartesRazorContext context) => _context = context;
    public Order Pedido { get; set; } = default!;
    public IActionResult OnGet(int id)
    {
        Pedido = _context.Order.FirstOrDefault(o => o.id == id);
        if (Pedido == null) return NotFound();
        // Seguridad: permite solo al dueño del pedido o al admin
        if (User.IsInRole("Admin") || Pedido.CustomerEmail == User.Identity?.Name)
            return Page();
        return Forbid();
    }
}
