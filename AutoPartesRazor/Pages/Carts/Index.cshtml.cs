<<<<<<< HEAD
﻿using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

namespace AutoPartesRazor.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        public IList<Cart> Cart { get; set; } = default!;
=======
        public IList<Cart> Cart { get;set; } = default!;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        public int CartCount { get; set; } = 0;

        public async Task OnGetAsync()
        {
            // Contar el número de items únicos en el carrito
<<<<<<< HEAD
            var count = await _context.Carts.CountAsync();
            CartCount = count;

            if (_context.Carts != null)
            {
                Cart = await _context.Carts
                .Include(c => c.Product).ToListAsync();
            }

            // Calcular el número total de productos en el carrito
            int total = Cart.Sum(item => item.Quantity);
=======
            var count = await _context.Cart.CountAsync();
            CartCount = count;

            if (_context.Cart != null)
            {
                Cart = await _context.Cart
                .Include(c => c.producto).ToListAsync();
            }

            // Calcular el número total de productos en el carrito
            int total = Cart.Sum(item => item.quantity);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        }
    }
}
