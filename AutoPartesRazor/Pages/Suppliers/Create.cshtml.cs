using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Suppliers;

public class CreateModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public CreateModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Supplier Supplier { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

<<<<<<< HEAD
        _context.Suppliers.Add(Supplier);
=======
        _context.Supplier.Add(Supplier);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
