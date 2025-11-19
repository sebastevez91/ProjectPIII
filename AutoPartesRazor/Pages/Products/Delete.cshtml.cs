using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product Product { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
<<<<<<< HEAD
        if (id == null || _context.Products == null)
=======
        if (id == null || _context.Product == null)
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        {
            return NotFound();
        }

<<<<<<< HEAD
        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
=======
        var product = await _context.Product.FirstOrDefaultAsync(m => m.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (product == null)
        {
            return NotFound();
        }
        else
        {
            Product = product;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
<<<<<<< HEAD
        if (id == null || _context.Products == null)
        {
            return NotFound();
        }
        var product = await _context.Products.FindAsync(id);
=======
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }
        var product = await _context.Product.FindAsync(id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (product != null)
        {
            Product = product;
<<<<<<< HEAD
            _context.Products.Remove(Product);
=======
            _context.Product.Remove(Product);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Administration/AdminDashboard");
    }
}
