using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

// Asumimos que este es el namespace
namespace AutoPartesRazor.Pages.Orders
{
    public class MyOrdersModel : PageModel
    {
<<<<<<< HEAD
        private readonly AutoPartesRazorContext _context;
        public MyOrdersModel(AutoPartesRazorContext context) => _context = context;
        public IList<Order> Pedidos { get; set; } = new List<Order>(); // Propiedad correcta: Pedidos
        public async Task OnGetAsync()
        {
            var userEmail = User.Identity?.Name;
            Pedidos = await _context.Orders
                .Where(o => o.CustomerEmail == userEmail)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
=======
        var userEmail = User.Identity?.Name;
        Pedidos = await _context.Orders
            .Where(o => o.CustomerEmail == userEmail)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
>>>>>>> main
    }
}