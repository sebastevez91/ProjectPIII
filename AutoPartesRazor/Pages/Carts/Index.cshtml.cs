using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public IList<Cart> Cart { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Cart != null)
            {
                Cart = await _context.Cart
                .Include(c => c.producto).ToListAsync();
            }

            // Calcular el número total de productos en el carrito
            int total = Cart.Sum(item => item.quantity);

            // Pasar el total a la vista usando ViewBag
            TempData["Total"] = total;
        }
    }
}
