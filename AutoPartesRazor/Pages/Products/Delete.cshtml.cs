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
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = await _context.Product.FirstOrDefaultAsync(m => m.id == id);

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
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }
        var product = await _context.Product.FindAsync(id);

        if (product != null)
        {
<<<<<<< HEAD
            Product = product;
            _context.Product.Remove(Product);
=======
            product.IsDelete = true;
            Product = product;
            _context.Product.Update(product);
>>>>>>> 2d35bc8959d58325a67095aeb7346e0b6de50540
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Adminitration/AdminDashboard");
    }
}
