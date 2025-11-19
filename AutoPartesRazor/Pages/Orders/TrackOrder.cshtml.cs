using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class TrackOrderModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public TrackOrderModel(AutoPartesRazorContext context) => _context = context;
    public Order Pedido { get; set; } = default!;
    public IActionResult OnGet(int id)
    {
        Pedido = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (Pedido == null) return NotFound();
        // Seguridad: permite solo al dueño del pedido o al admin
        if (User.IsInRole("Admin") || Pedido.CustomerEmail == User.Identity?.Name)
            return Page();
        return Forbid();
    }
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || pedido.Status != "Entregado")
            return NotFound();

        if (Request.Form.TryGetValue("Calificacion", out var calif) && int.TryParse(calif, out int nota))
            pedido.Calificacion = nota;

        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });
    }



}
