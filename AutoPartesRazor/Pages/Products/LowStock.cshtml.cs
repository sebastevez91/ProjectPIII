using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products
{
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
                    .Where(p => p.Stock < 5)// Assuming low stock is defined as less than 10 units
                    .ToListAsync();
            }
            return Page();
        }
    }
}
