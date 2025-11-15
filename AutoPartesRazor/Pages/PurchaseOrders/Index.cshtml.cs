using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.PurchaseOrders;

public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public IndexModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<PurchaseOrder> PurchaseOrder { get; set; }

    public async Task OnGetAsync()
    {
        PurchaseOrder = await _context.PurchaseOrder
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .ToListAsync();
    }
}
