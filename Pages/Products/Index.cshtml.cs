using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public int CartCount { get; set; } = 0;

        public async Task OnGetAsync()
        {
            Products = await _context.Product
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();

            Categories = await _context.Category
                .Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name })
                .ToListAsync();

            CartCount = await _context.Cart.CountAsync();
        }

        // Handler AJAX para añadir al carrito
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            if (_context.Product == null) return new JsonResult(new { success = false, message = "No hay productos disponibles" });

            var product = await _context.Product.FindAsync(productId);
            if (product == null) return new JsonResult(new { success = false, message = "Producto no encontrado" });

            if (quantity <= 0) quantity = 1;

            var cartItem = new Cart
            {
                productId = productId,
                quantity = quantity
            };

            _context.Cart.Add(cartItem);
            await _context.SaveChangesAsync();

            var cartCount = await _context.Cart.CountAsync();
            return new JsonResult(new { success = true, message = "Añadido al carrito", cartCount });
        }
    }
}