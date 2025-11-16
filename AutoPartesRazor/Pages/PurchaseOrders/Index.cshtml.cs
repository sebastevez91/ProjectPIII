using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
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
        SelectStatus = Enum.GetValues<StatusOrder>()
            .Select(s => new SelectListItem
            {
                Value = ((int)s).ToString(),
                Text = s.ToString()
            })
            .ToList();

        PurchaseOrder = await _context.PurchaseOrder
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .ToListAsync();
    }

    // Handler para obtener el nombre del enum
    public IActionResult OnGetGetEnumName(int index)
    {
        var status = (StatusOrder)index;
        return Content(status.ToString());
    }

    // Handler para cambiar estado
    public async Task<IActionResult> OnPostChangeStatus([FromBody] StatusUpdate data)
    {
        var order = await _context.PurchaseOrder.FindAsync(data.Id);
        if (order == null)
            return NotFound();

        // Convertir índice ? Enum
        order.Status = (StatusOrder)data.Index;
        await _context.SaveChangesAsync();

        return new JsonResult(new { ok = true });
    }

    public class StatusUpdate
    {
        public int Id { get; set; }
        public int Index { get; set; }
    }
}