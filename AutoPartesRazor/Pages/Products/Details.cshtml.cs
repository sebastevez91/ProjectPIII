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
<<<<<<< HEAD
            if (id == null || _context.Products == null)
=======
            if (id == null || _context.Product == null)
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            {
                return NotFound();
            }

            // ⬇️ MODIFICADO: Agregar .Include para cargar reseñas ⬇️
<<<<<<< HEAD
            var product = await _context.Products
=======
            var product = await _context.Product
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                .Include(b => b.Brand)
                .Include(c => c.Category)
                .Include(p => p.Reviews!)         // ⬅️ NUEVO: Cargar reseñas
                    .ThenInclude(r => r.User)     // ⬅️ NUEVO: Cargar usuarios de las reseñas
<<<<<<< HEAD
                .FirstOrDefaultAsync(m => m.Id == id);
=======
                .FirstOrDefaultAsync(m => m.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

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
<<<<<<< HEAD
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user != null)
                {
                    UserHasReviewed = await _context.ProductReviews
=======
                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user != null)
                {
                    UserHasReviewed = await _context.ProductReview
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                        .AnyAsync(r => r.ProductId == id && r.UserId == user.Id);
                }
            }

            // Contar el número de items únicos en el carrito
<<<<<<< HEAD
            var count = await _context.Carts.CountAsync();
=======
            var count = await _context.Cart.CountAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            CartCount = count;

            return Page();
        }
    }
}