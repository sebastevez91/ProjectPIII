using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public DetailsModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public Product Product { get; set; } = default!;
        public int CartCount { get; set; } = 0;

        // ⬇️ NUEVA PROPIEDAD ⬇️
        public bool UserHasReviewed { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            // ⬇️ MODIFICADO: Agregar .Include para cargar reseñas ⬇️
            var product = await _context.Product
                .Include(b => b.Brand)
                .Include(c => c.Category)
                .Include(p => p.Reviews!)         // ⬅️ NUEVO: Cargar reseñas
                    .ThenInclude(r => r.User)     // ⬅️ NUEVO: Cargar usuarios de las reseñas
                .FirstOrDefaultAsync(m => m.id == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
            }

            // ⬇️ NUEVO: Verificar si el usuario ya reseñó este producto ⬇️
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user != null)
                {
                    UserHasReviewed = await _context.ProductReview
                        .AnyAsync(r => r.ProductId == id && r.UserId == user.Id);
                }
            }

            // Contar el número de items únicos en el carrito
            var count = await _context.Cart.CountAsync();
            CartCount = count;

            return Page();
        }
    }
}