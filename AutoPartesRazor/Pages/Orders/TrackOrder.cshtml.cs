using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
=======
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

public class TrackOrderModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public TrackOrderModel(AutoPartesRazorContext context) => _context = context;
    public Order Pedido { get; set; } = default!;
    public IActionResult OnGet(int id)
    {
<<<<<<< HEAD
        Pedido = _context.Orders.FirstOrDefault(o => o.Id == id);
=======
        Pedido = _context.Order.FirstOrDefault(o => o.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        if (Pedido == null) return NotFound();
        // Seguridad: permite solo al dueño del pedido o al admin
        if (User.IsInRole("Admin") || Pedido.CustomerEmail == User.Identity?.Name)
            return Page();
        return Forbid();
    }
    public async Task<IActionResult> OnPostAsync(int id)
    {
<<<<<<< HEAD
        var pedido = await _context.Orders.FindAsync(id);
=======
        var pedido = await _context.Order.FindAsync(id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        if (pedido == null || pedido.Status != "Entregado")
            return NotFound();

        if (Request.Form.TryGetValue("Calificacion", out var calif) && int.TryParse(calif, out int nota))
            pedido.Calificacion = nota;

        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });
    }



}
