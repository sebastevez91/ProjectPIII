using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace AutoPartesRazor.Pages.Administration
{
    public class DespachoModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;
        public DespachoModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        public IList<Order> PedidosParaDespacho { get; set; } = new List<Order>();

        // GET: carga la lista de pedidos "Pending"
        public async Task OnGetAsync()
        {
            PedidosParaDespacho = await _context.Order
                .Where(o => o.Status == "Pending")
                .ToListAsync();
        }

        // POST: Cambia el pedido a "Despachado"
        public async Task<IActionResult> OnPostDespacharAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null)
                return NotFound();
            pedido.Status = "Despachado";
            await _context.SaveChangesAsync();
            TempData["DespachoOK"] = true; // <-- Notificación
            return RedirectToPage();
        }

    }
}
