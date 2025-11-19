using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Categories;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public EditModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
<<<<<<< HEAD
        if (id == null || _context.Categories == null)
=======
        if (id == null || _context.Category == null)
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        {
            return NotFound();
        }

<<<<<<< HEAD
        var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
=======
        var category = await _context.Category.FirstOrDefaultAsync(m => m.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        if (category == null)
        {
            return NotFound();
        }
        Category = category;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
<<<<<<< HEAD
            if (!CategoryExists(Category.Id))
=======
            if (!CategoryExists(Category.id))
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Adminitration/AdminDashboard");
    }

    private bool CategoryExists(int id)
    {
<<<<<<< HEAD
        return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
=======
        return (_context.Category?.Any(e => e.id == id)).GetValueOrDefault();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}
