using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products;

public class AssignSuppliersModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public AssignSuppliersModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public Product Product { get; set; }
    public List<Supplier> AllSuppliers { get; set; } = new();
    public List<int> AssignedSuppliers { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
<<<<<<< HEAD
        Product = await _context.Products
            .Include(p => p.ProductSuppliers)
            .FirstOrDefaultAsync(p => p.Id == id);
=======
        Product = await _context.Product
            .Include(p => p.ProductSuppliers)
            .FirstOrDefaultAsync(p => p.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (Product == null)
            return NotFound();

<<<<<<< HEAD
        AllSuppliers = await _context.Suppliers.ToListAsync();
=======
        AllSuppliers = await _context.Supplier.ToListAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        AssignedSuppliers = Product.ProductSuppliers.Select(ps => ps.SupplierId).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, int[] selectedSuppliers)
    {
<<<<<<< HEAD
        var product = await _context.Products
            .Include(p => p.ProductSuppliers)
            .FirstOrDefaultAsync(p => p.Id == id);
=======
        var product = await _context.Product
            .Include(p => p.ProductSuppliers)
            .FirstOrDefaultAsync(p => p.id == id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (product == null)
            return NotFound();

        product.ProductSuppliers.Clear();

        foreach (var supplierId in selectedSuppliers)
        {
            product.ProductSuppliers.Add(new ProductSupplier
            {
                ProductId = id,
                SupplierId = supplierId
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("/Products/Index");
    }
}
