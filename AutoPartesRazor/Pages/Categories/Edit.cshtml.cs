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
        if (id == null || _context.Category == null)
        {
            return NotFound();
        }

        var category = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);
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
            if (!CategoryExists(Category.Id))
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
        return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
