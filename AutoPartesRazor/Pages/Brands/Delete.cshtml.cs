<<<<<<< HEAD
﻿using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Brands;

public class DeleteModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Brand Brand { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Brands == null)
        {
            return NotFound();
        }


        var brand = await _context.Brands.FirstOrDefaultAsync(m => m.Id == id);


        if (brand == null)
        {
            return NotFound();
        }
        else
        {
            Brand = brand;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Brands == null)
        {
            return NotFound();
        }
        var brand = await _context.Brands.FindAsync(id);


        if (brand != null)
        {
            Brand = brand;
            _context.Brands.Remove(Brand);
            await _context.SaveChangesAsync();
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

namespace AutoPartesRazor.Pages.Brands
{
    public class DeleteModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Brand Brand { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand.FirstOrDefaultAsync(m => m.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

            if (brand == null)
            {
                return NotFound();
            }
<<<<<<< HEAD
            else
=======
            else 
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            {
                Brand = brand;
            }
            return Page();
<<<<<<< HEAD

        }

        return RedirectToPage("./Index");
=======
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }
            var brand = await _context.Brand.FindAsync(id);

            if (brand != null)
            {
                Brand = brand;
                _context.Brand.Remove(Brand);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}
