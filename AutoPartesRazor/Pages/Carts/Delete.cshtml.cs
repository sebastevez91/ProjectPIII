using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Carts
{
    public class DeleteModel : PageModel
    {
        private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

        public DeleteModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }
            var cart = await _context.Carts.FindAsync(id);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Producto eliminado correctamente.";
            }

            return RedirectToPage("./Index");
        }
    }
}
