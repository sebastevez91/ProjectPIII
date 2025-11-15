using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;

public class MyOrdersModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public MyOrdersModel(AutoPartesRazorContext context) => _context = context;
    public IList<Order> Pedidos { get; set; } = new List<Order>();
    public async Task OnGetAsync()
    {
        var userEmail = User.Identity?.Name;
        Pedidos = await _context.Order
            .Where(o => o.CustomerEmail == userEmail)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}
