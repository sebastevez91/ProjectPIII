using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        public IList<Client> ClientList { get; set; } = new List<Client>();

        public string CurrentFilter { get; set; } = string.Empty;

        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString ?? string.Empty;

            var query = _context.Client.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.LastName.Contains(searchString) ||
                    c.Email.Contains(searchString));
            }

            ClientList = await query.ToListAsync();
        }
    }
}