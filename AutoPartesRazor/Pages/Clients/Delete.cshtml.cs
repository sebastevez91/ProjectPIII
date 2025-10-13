using AutoPartesRazor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public DeleteModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
            }

            return RedirectToPage("./Index");
        }
    }
}