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
    public Coupon? AppliedCoupon { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        Order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id.Value);

        if (Order == null) return NotFound();

        // Cargar el cupón si existe
        if (Order.CouponId.HasValue)
        {
            AppliedCoupon = await _context.Coupons
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == Order.CouponId.Value);
        }

        return Page();
    }
}