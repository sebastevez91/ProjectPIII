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
    }
}
