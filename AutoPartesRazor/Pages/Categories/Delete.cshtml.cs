using AutoPartesRazor.Models;
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

        return RedirectToPage("./Index");
    }
}
