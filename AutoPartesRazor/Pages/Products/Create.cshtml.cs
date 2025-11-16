using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoPartesRazor.Pages.Products;

public class CreateModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public CreateModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product Product { get; set; } = default!;
    public List<SelectListItem> Categories { get; set; }
    public List<SelectListItem> Brands { get; set; }

    // Para imagen del producto
    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
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

        return Page();
    }


    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Products == null || Product == null)
        {
            return Page();
        }

        if (ImageFile != null)
        {
            var fileName = Path.GetFileName(ImageFile.FileName);
            var uploadPath = Path.Combine("wwwroot/img/products", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            Product.ImagePath = "/img/products/" + fileName;
        }

        _context.Products.Add(Product);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Administration/AdminDashboard");
    }
}
