using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Suppliers;

public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public IndexModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<Supplier> Suppliers { get; set; }

    public async Task OnGetAsync()
    {
<<<<<<< HEAD
        Suppliers = await _context.Suppliers.ToListAsync();
=======
        Suppliers = await _context.Supplier.ToListAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}
