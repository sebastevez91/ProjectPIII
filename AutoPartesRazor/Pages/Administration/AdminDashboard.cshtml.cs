using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Administration;

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
    public IList<Category> Categories { get; set; } = default!;
    public IList<Brand> Brands { get; set; } = default!;
    public IList<Notification> Notifications { get; set; } = default!;

    public List<Product> LowStockProducts { get; set; } = new();


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

        Categories = _context.Category != null
            ? await _context.Category.ToListAsync()
            : new List<Category>();

        Brands = _context.Brand != null
            ? await _context.Brand.ToListAsync()
            : new List<Brand>();

        Notifications = _context.Notification != null
            ? await _context.Notification.Include(n => n.User).ToListAsync()
            : new List<Notification>();

        // Obtener productos con bajo stock (por ejemplo, stock menor o igual a 5)
        LowStockProducts = await _context.Product
            .Where(p => !p.IsDeleted && p.stock <= 5)
            .OrderBy(p => p.stock)
            .ToListAsync();

    }
}

