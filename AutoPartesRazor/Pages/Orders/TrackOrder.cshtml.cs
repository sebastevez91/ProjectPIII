using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Necesario para ClaimTypes
using Microsoft.AspNetCore.Routing; // Necesario para LinkGenerator
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPartesRazor.Pages.Orders // Asegúrate de que el namespace sea correcto
{
    public class TrackOrderModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly LinkGenerator _linkGenerator;

        public TrackOrderModel(AutoPartesRazorContext context, IEmailSender emailSender, IUserService userService, LinkGenerator linkGenerator)
        {
            _context = context;
            _emailSender = emailSender;
            _userService = userService;
            _linkGenerator = linkGenerator;
        }

        public Order Pedido { get; set; } = default!;

        // El OnGet regular para mostrar la página de seguimiento
        public IActionResult OnGet(int id)
        {
            Pedido = _context.Orders
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (Pedido == null) return NotFound();

            // Lógica de autorización
            if (User.IsInRole("Admin") || Pedido.CustomerEmail == User.Identity?.Name)
                return Page();

            return Forbid();
        }

        // Método auxiliar para crear Notificación para el Admin
        private async Task CrearNotificacionAdminAsync(Order pedido, string asunto, string mensaje)
        {
            var adminUser = await _userService.GetUserAsync("admin@tutienda.com"); // Reemplazar con el email real del Admin

            if (adminUser != null)
            {
                var notificacion = new Notification
                {
                    UserId = adminUser.Id,
                    Title = asunto,
                    Message = mensaje,
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    RelatedUrl = $"/Administration/DetallePedido?id={pedido.Id}"
                };
                _context.Notifications.Add(notificacion);
            }
        }

        // =======================================================================
        // HANDLER DE ACCIÓN: Confirmación de Recepción por el Cliente (Paso 3)
        // =======================================================================
        // HANDLER: Cliente Confirma Recepción
        public async Task<IActionResult> OnGetConfirmarRecepcionAsync(int id)
        {
            // 1. Lógica de verificación y obtención de pedido
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // El pedido debe existir, pertenecer al usuario y estar "En Camino"
            var pedido = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            // REDIRECCIÓN MEJORADA: Si falla, redirige a la página de seguimiento del pedido, no a la lista.
            if (pedido == null || pedido.Status != "En Camino")
            {
                // Si el pedido no se encuentra o el usuario no es el dueño, redirige a MyOrders o NotFound.
                if (pedido == null) return NotFound();

                // Si el estado no es "En Camino" (ej: ya está Entregado), redirige al detalle con un mensaje
                TempData["InfoMessage"] = $"El pedido #{id} tiene el estado '{pedido.Status}' y no se puede confirmar la recepción.";
                return RedirectToPage("./TrackOrder", new { id });
            }

            // ... (Resto de la lógica de cambio de estado a "Entregado" y envío de notificaciones de Calificación)

            // 2. Cambiar Status a Entregado 
            pedido.Status = "Entregado";
            pedido.UpdatedAt = DateTime.Now;

            // ... (Lógica para generar notificaciones de Calificación)

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"¡Entrega confirmada! Revisa tus notificaciones para calificar el pedido.";
            return RedirectToPage("./TrackOrder", new { id });
        }
    }
}