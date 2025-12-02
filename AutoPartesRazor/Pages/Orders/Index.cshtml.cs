using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;

namespace AutoPartesRazor.Pages.Orders
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        public List<Order> Order { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchCode { get; set; }

        public async Task OnGetAsync()
        {
            // Query base - excluye órdenes eliminadas
            var query = _context.Orders
                .Where(o => !o.IsDeleted)
                .AsQueryable();

            // Filtro por Estado
            if (!string.IsNullOrWhiteSpace(SearchStatus))
            {
                query = query.Where(o => o.Status == SearchStatus);
            }

            // Filtro por Código (ID)
            if (!string.IsNullOrWhiteSpace(SearchCode))
            {
                // Intenta convertir a int para buscar por ID
                if (int.TryParse(SearchCode.Trim(), out int orderId))
                {
                    query = query.Where(o => o.Id == orderId);
                }
                else
                {
                    // Si no es un número válido, también puede buscar por código de cupón
                    query = query.Where(o => o.CouponCode != null &&
                                            o.CouponCode.Contains(SearchCode));
                }
            }

            // Ordena por fecha de creación descendente (más reciente primero)
            Order = await query
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.User)
                .Include(o => o.Items)
                .ToListAsync();
        }

        // Handler para cambiar estado a "Preparando"
        public async Task<IActionResult> OnPostPrepararAsync(int id)
        {
            var pedido = await _context.Orders.FindAsync(id);

            if (pedido == null)
                return NotFound();

            // Cambiar estado a "Preparando"
            pedido.Status = "Preparando";
            pedido.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["DespachoOK"] = true;

            // Mantener los filtros actuales al recargar
            return RedirectToPage(new
            {
                searchStatus = SearchStatus,
                searchCode = SearchCode
            });
        }

        // Handler para cambiar estado a "Despachado"
        public async Task<IActionResult> OnPostDespacharAsync(int id)
        {
            var pedido = await _context.Orders.FindAsync(id);

            if (pedido == null)
                return NotFound();

            // Cambiar estado a "Despachado"
            pedido.Status = "Despachado";
            pedido.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["DespachoOK"] = true;

            // Mantener los filtros actuales al recargar
            return RedirectToPage(new
            {
                searchStatus = SearchStatus,
                searchCode = SearchCode
            });
        }
    }
}