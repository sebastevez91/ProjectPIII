using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Suppliers;

public class EditModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public EditModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Supplier Supplier { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Supplier = await _context.Suppliers.FindAsync(id);

        if (Supplier == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _context.Suppliers.Update(Supplier);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
