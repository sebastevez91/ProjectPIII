using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public IndexModel(ILogger<IndexModel> logger, Data.AutoPartesRazorContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Product> products { get; set; } = default;

        public async Task OnGetAsync()
        {
            if (_context.Product != null)
            {
                products = await _context.Product
                    .Include(c => c.Category)
                    .Include(b => b.Brand)
                    .ToListAsync();
            }

        }
    }
}
