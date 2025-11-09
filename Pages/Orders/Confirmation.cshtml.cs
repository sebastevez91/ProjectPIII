using System.Threading.Tasks;
using AutoPartesRazor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Orders
{
    public class ConfirmationModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public ConfirmationModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // opcional: validar existencia
            var exists = await _context.Order.FindAsync(OrderId);
            if (exists == null) return NotFound();
            return Page();
        }
    }
}