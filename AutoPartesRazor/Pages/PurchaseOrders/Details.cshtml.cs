using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.PurchaseOrders
{
    public class DetailsModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public DetailsModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public PurchaseOrder PurchaseOrder { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseorder = await _context.PurchaseOrders
                .Include(p => p.Product)
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseorder == null)
            {
                return NotFound();
            }
            else
            {
                PurchaseOrder = purchaseorder;
            }
            return Page();
        }
    }
}
