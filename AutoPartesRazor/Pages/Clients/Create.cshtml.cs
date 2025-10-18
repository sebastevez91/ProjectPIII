using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AutoPartesRazor.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public CreateModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        // Se usa OnGet para inicializar la página
        public IActionResult OnGet()
        {
            // Inicializa el modelo si es necesario, aunque aquí no es estrictamente necesario
            Client = new Client();
            return Page();
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        // Método que se llama al presionar el botón de "Crear"
        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Verificar si el ModelState es válido (validaciones del modelo Client)
            if (!ModelState.IsValid || _context.Client == null || Client == null)
            {
                // Si la validación falla, recarga la página para mostrar los mensajes de error.
                return Page();
            }

            // 2. Insertar el cliente y guardar
            _context.Client.Add(Client);
            await _context.SaveChangesAsync();

            // 3. Establecer el mensaje de éxito (para SweetAlert)
            TempData["SuccessMessage"] = "Cliente creado y guardado exitosamente.";

            // 4. Redirigir al listado
            return RedirectToPage("./Index");
        }
    }
}