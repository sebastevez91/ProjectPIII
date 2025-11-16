using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.PurchaseOrders
{
    public class CreateModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public CreateModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PurchaseOrder purchaseOrder { get; set; } = new();

        public SelectList ProductList { get; set; }
        public SelectList SupplierList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = new SelectList(await _context.Products
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync(), "id", "name");

            SupplierList = new SelectList(await _context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync(), "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLists();
                return Page();
            }

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadLists()
        {
            ProductList = new SelectList(await _context.Products.Where(p => !p.IsDeleted).ToListAsync(), "id", "name");
            SupplierList = new SelectList(await _context.Suppliers.ToListAsync(), "Id", "Name");
        }
    }
}
