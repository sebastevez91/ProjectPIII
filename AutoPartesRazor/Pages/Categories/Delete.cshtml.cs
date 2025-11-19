<<<<<<< HEAD
﻿using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Categories;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);


        if (category == null)
        {
            return NotFound();
        }
        else
        {
            Category = category;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }
        var category = await _context.Categories.FindAsync(id);

        if (category != null)
        {
            Category = category;
            _context.Categories.Remove(Category);
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

namespace AutoPartesRazor.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FirstOrDefaultAsync(m => m.id == id);

>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            if (category == null)
            {
                return NotFound();
            }
<<<<<<< HEAD
            else
=======
            else 
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            {
                Category = category;
            }
            return Page();
        }

<<<<<<< HEAD
        return RedirectToPage("./Index");
=======
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }
            var category = await _context.Category.FindAsync(id);

            if (category != null)
            {
                Category = category;
                _context.Category.Remove(Category);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}
