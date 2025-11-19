using AutoPartesRazor.Models;
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

        return RedirectToPage("./Index");
    }
}
