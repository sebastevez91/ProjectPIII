using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Pages.Administration
{
    public class DetallePedidoModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;
        public DetallePedidoModel(AutoPartesRazorContext context) => _context = context;
        public Order Pedido { get; set; } = default!;
        public IActionResult OnGet(int id)
        {
            Pedido = _context.Order.FirstOrDefault(o => o.id == id);
            if (Pedido == null)
                return NotFound();
            return Page();
        }
        public async Task<IActionResult> OnPostPrepararAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.Status = "Preparando";
            await _context.SaveChangesAsync();
            TempData["orderMessage"] = "Pedido marcado como 'Preparando'";
            return RedirectToPage(new { id });
        }
        // Repite para los otros estados cambiando el tipo y el status
        public async Task<IActionResult> OnPostDespacharAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.Status = "Despachado";
            await _context.SaveChangesAsync();
            TempData["orderMessage"] = "Pedido despachado.";
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostEnCaminoAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.Status = "En camino";
            await _context.SaveChangesAsync();
            TempData["orderMessage"] = "Pedido en camino.";
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostEntregarAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.Status = "Entregado";
            await _context.SaveChangesAsync();
            TempData["orderMessage"] = "Pedido entregado exitosamente.";
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostCancelarAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null) return NotFound();
            pedido.Status = "Cancelado";
            await _context.SaveChangesAsync();
            TempData["orderMessage"] = "Pedido cancelado.";
            return RedirectToPage(new { id });
        }

    }
}
