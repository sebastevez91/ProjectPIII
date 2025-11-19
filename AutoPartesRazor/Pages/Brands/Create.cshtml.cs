using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Brands
{
    public class CreateModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public CreateModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Brand Brand { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
<<<<<<< HEAD
            if (!ModelState.IsValid || _context.Brands == null || Brand == null)
=======
            if (!ModelState.IsValid || _context.Brand == null || Brand == null)
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            {
                return Page();
            }

<<<<<<< HEAD
            _context.Brands.Add(Brand);
=======
            _context.Brand.Add(Brand);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            await _context.SaveChangesAsync();

            return RedirectToPage("/Adminitration/AdminDashboard");
        }
    }
}
