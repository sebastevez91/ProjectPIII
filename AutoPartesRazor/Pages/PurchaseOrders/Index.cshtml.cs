using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public List<SelectListItem> SelectStatus { get; set; }

    public async Task OnGetAsync()
    {
        PurchaseOrder = await _context.PurchaseOrders
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .ToListAsync();
    }

    // Handler para cambiar estado
    public async Task<IActionResult> OnPostChangeStatus()
    {
        var order = await _context.PurchaseOrders.FindAsync();
        if (order == null)
            return NotFound();

        await _context.SaveChangesAsync();

        return new JsonResult(new { ok = true });
    }

}