using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products;

[Authorize(Roles ="Admin")]
public class LowStockModel : PageModel
{
    private readonly ILogger<LowStockModel> _logger;
    private readonly AutoPartesRazorContext _context;

    public LowStockModel(ILogger<LowStockModel> logger, AutoPartesRazorContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IList<Product> LowStockProducts { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        if (_context.Products != null)
        {
            LowStockProducts = await _context.Products
                .Where(p => p.Stock < 5)
                .ToListAsync();
        }
        return Page();
    }
}
