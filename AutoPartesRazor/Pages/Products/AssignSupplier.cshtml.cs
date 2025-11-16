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
        Product = await _context.Product
            .Include(p => p.ProductSuppliers)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Product == null)
            return NotFound();

        AllSuppliers = await _context.Supplier.ToListAsync();
        AssignedSuppliers = Product.ProductSuppliers.Select(ps => ps.SupplierId).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, int[] selectedSuppliers)
    {
        var product = await _context.Product
            .Include(p => p.ProductSuppliers)
            .FirstOrDefaultAsync(p => p.Id == id);

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
