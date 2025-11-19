using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Users;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public DetailsModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public User User { get; set; }
    public int TotalOrders { get; set; }
    public int TotalNotifications { get; set; }
    public List<Order> RecentOrders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        User = await _userManager.FindByIdAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        // Obtener estadísticas
        TotalOrders = await _context.Orders
            .Where(o => o.UserId == id)
            .CountAsync();

        TotalNotifications = await _context.Notifications
            .Where(n => n.UserId == id)
            .CountAsync();

        // Obtener últimas 5 órdenes
        RecentOrders = await _context.Orders
            .Where(o => o.UserId == id)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .ToListAsync();

        return Page();
    }
}