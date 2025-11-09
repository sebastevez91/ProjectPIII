using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Adminitration;

[Authorize(Roles = "Admin")]
public class AdminDashboardModel : PageModel
{
    private readonly Data.AutoPartesRazorContext _context;

    public AdminDashboardModel(Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<User> Users { get; set; } = default!;
    public IList<Product> Products { get; set; } = default!;
    public IList<Order> Orders { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Users = _context.User != null
            ? await _context.User.ToListAsync()
            : new List<User>();

        Products = _context.Product != null
            ? await _context.Product.Include(p => p.Category).ToListAsync()
            : new List<Product>();

        Orders = _context.Order != null
            ? await _context.Order.Include(o => o.Items).ThenInclude(oi => oi.Product).ToListAsync()
            : new List<Order>();
    }
}
