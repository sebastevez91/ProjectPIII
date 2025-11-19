using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

<<<<<<< HEAD
namespace AutoPartesRazor.Pages.Brands;

public class EditModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public EditModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Brand Brand { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Brands == null)
        {
            return NotFound();
        }

        var brand = await _context.Brands.FirstOrDefaultAsync(m => m.Id == id);
        if (brand == null)
        {
            return NotFound();
        }
        Brand = brand;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Brand).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BrandExists(Brand.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Adminitration/AdminDashboard");
    }

    private bool BrandExists(int id)
    {
        return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
=======
namespace AutoPartesRazor.Pages.Brands
{
    public class EditModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public EditModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Brand Brand { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand.FirstOrDefaultAsync(m => m.id == id);
            if (brand == null)
            {
                return NotFound();
            }
            Brand = brand;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(Brand.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Adminitration/AdminDashboard");
        }

        private bool BrandExists(int id)
        {
            return (_context.Brand?.Any(e => e.id == id)).GetValueOrDefault();
        }
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}
