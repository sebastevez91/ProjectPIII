<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class DeleteModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
<<<<<<< HEAD
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }
            var cart = await _context.Carts.FindAsync(id);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
=======
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }
            var cart = await _context.Cart.FindAsync(id);

            if (cart != null)
            {
                _context.Cart.Remove(cart);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Producto eliminado correctamente.";
            }

            return RedirectToPage("./Index");
        }
    }
}
