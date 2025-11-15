using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoPartesRazor.Pages.Products
{
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
            Categories = _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.name
                })
                .ToList();

            //Llena la lista de marcas
            Brands = _context.Brand
                .Select(b => new SelectListItem
                {
                    Value = b.id.ToString(),
                    Text = b.name
                })
                .ToList();

            return Page();
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Product == null || Product == null)
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

            _context.Product.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Administration/AdminDashboard");
        }
    }
}
