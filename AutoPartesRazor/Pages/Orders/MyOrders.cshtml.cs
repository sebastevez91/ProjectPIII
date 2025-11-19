<<<<<<< HEAD
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
=======
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

public class MyOrdersModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    public MyOrdersModel(AutoPartesRazorContext context) => _context = context;
    public IList<Order> Pedidos { get; set; } = new List<Order>();
    public async Task OnGetAsync()
    {
        var userEmail = User.Identity?.Name;
<<<<<<< HEAD
        Pedidos = await _context.Orders
=======
        Pedidos = await _context.Order
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            .Where(o => o.CustomerEmail == userEmail)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}
