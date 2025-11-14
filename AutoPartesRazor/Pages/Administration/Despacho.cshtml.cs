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
        public DespachoModel(AutoPartesRazorContext context) => _context = context;

        public IList<Order> PedidosPendientes { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            PedidosPendientes = await _context.Order
                .Where(o => o.Status == "Pending")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDespacharAsync(int id)
        {
            var pedido = await _context.Order.FindAsync(id);
            if (pedido == null)
                return NotFound();
            pedido.Status = "Despachado";
            await _context.SaveChangesAsync();
            TempData["DespachoOK"] = true;
            return RedirectToPage();
        }
    }
}
