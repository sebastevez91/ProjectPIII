using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get; set; } = default!;
        public string SearchProduct {get; set; } = string.Empty;

        public async Task OnGetAsync(string searchString)
        {
            // Buscador por nombre de producto
            SearchProduct = searchString ?? string.Empty;
            var query = _context.Product.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(p =>
                    p.name.Contains(searchString));
            }


            Product = await query
                    .Include(c => c.Category)
                    .Include(b => b.Brand)
                    .ToListAsync();
        }
    }
}
