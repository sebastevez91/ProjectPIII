using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public IndexModel(ILogger<IndexModel> logger, Data.AutoPartesRazorContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Product> products { get; set; } = default;

        public async Task OnGetAsync()
        {
            if (_context.Product != null)
            {
                products = await _context.Product
                    .Include(c => c.Category)
                    .Include(b => b.Brand)
                    .ToListAsync();
            }

        }
        public async Task<IActionResult> OnPostAgregarCarrito(int productoId, int cantidad)
        {
            var product = await _context.Product.FindAsync(productoId);
            if (product == null)
            {
                TempData["Message"] = "El producto no fue encontrado.";
                return RedirectToPage("/Index");
            }

            var cartItem = _context.Cart.FirstOrDefault(p => p.productId == productoId);
            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    productId = productoId,
                    quantity = cantidad
                };
                _context.Cart.Add(cartItem);
            }
            else
            {
                cartItem.quantity += cantidad;
            }

            await _context.SaveChangesAsync();

            int total = _context.Cart.Sum(item => item.quantity);
            TempData["Total"] = total;
            TempData["Message"] = "Producto agregado al carrito correctamente.";

            return RedirectToPage("/Index");
        }


    }
}
