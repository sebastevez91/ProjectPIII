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
<<<<<<< HEAD
        Supplier = await _context.Suppliers.FindAsync(id);
=======
        Supplier = await _context.Supplier.FindAsync(id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (Supplier == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

<<<<<<< HEAD
        _context.Suppliers.Update(Supplier);
=======
        _context.Supplier.Update(Supplier);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
