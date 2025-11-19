using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;
    private readonly IWebHostEnvironment _env;

    public EditModel(AutoPartesRazor.Data.AutoPartesRazorContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Product Product { get; set; } = default!;
    public List<SelectListItem> Categories { get; set; }
    public List<SelectListItem> Brands { get; set; }

    // Para imagen del producto
    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    // Precio sin decimales
    [BindProperty]
    public int PriceInteger { get; set; } = 0;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Products == null)
        {
            return NotFound();
        }

        //Llena la lista de categorias
        Categories = _context.Categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();

        //Llena la lista de marcas
        Brands = _context.Brands
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            })
            .ToList();

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        Product = product;
        PriceInteger = (int)Product.Price;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Product).State = EntityState.Modified;
        Product.Price = PriceInteger;

        try
        {
            if (ImageFile != null)
            {
                // Crear carpeta si no existe y usar ruta absoluta del wwwroot
                var uploadsDir = Path.Combine(_env.WebRootPath, "img", "products");
                Directory.CreateDirectory(uploadsDir);

                var ext = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var uploadPath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                Product.ImagePath = "/img/products/" + fileName;
            }
            else
            {
                // Mantener la imagen existente
                _context.Entry(Product).Property(p => p.ImagePath).IsModified = false;
            }

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(Product.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Administration/AdminDashboard");
    }

    private bool ProductExists(int id)
    {
        return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
