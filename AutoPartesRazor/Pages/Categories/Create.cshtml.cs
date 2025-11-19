using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Categories
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
        public Category Category { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
<<<<<<< HEAD
            if (!ModelState.IsValid || _context.Categories == null || Category == null)
=======
            if (!ModelState.IsValid || _context.Category == null || Category == null)
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            {
                return Page();
            }

<<<<<<< HEAD
            _context.Categories.Add(Category);
=======
            _context.Category.Add(Category);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            await _context.SaveChangesAsync();

            return RedirectToPage("/Adminitration/AdminDashboard");
        }
    }
}
