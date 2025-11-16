using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public IndexModel(ILogger<IndexModel> logger, Data.AutoPartesRazorContext context)
    {
        _logger = logger;
        _context = context;
    }

    public int CartCount { get; set; } = 0;

    public async Task OnGetAsync()
    {
        // Contar el número de items únicos en el carrito
        var count = await _context.Carts.CountAsync();
        CartCount = count;
    }
}
