using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Orders;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public DetailsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        Order = await _context.Order
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.id == id.Value);

        if (Order == null) return NotFound();

        return Page();
    }
}
