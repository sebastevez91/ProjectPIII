using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
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
        Users = _context.Users != null
            ? await _context.Users.ToListAsync()
            : new List<User>();

        Products = _context.Products != null
            ? await _context.Products.Include(p => p.Category).ToListAsync()
            : new List<Product>();

        Orders = _context.Orders != null
            ? await _context.Orders.Include(o => o.Items).ThenInclude(oi => oi.Product).ToListAsync()
            : new List<Order>();

        Categories = _context.Categories != null
            ? await _context.Categories.ToListAsync()
            : new List<Category>();

        Brands = _context.Brands != null
            ? await _context.Brands.ToListAsync()
            : new List<Brand>();

        Notifications = _context.Notifications != null
            ? await _context.Notifications.Include(n => n.User).ToListAsync()
            : new List<Notification>();

        // Obtener productos con bajo stock (por ejemplo, stock menor o igual a 5)
        LowStockProducts = await _context.Products
            .Where(p => !p.IsDeleted && p.Stock <= 5)
            .OrderBy(p => p.Stock)
            .ToListAsync();

    }
}

