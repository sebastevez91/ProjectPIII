using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
// using QRCoder; // Eliminamos la referencia a QRCoder si la había

namespace AutoPartesRazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly AutoPartesRazorContext _context;

        public TrackController(AutoPartesRazorContext context)
        {
            _context = context;
        }

        // Método para obtener el estado del pedido (necesario para el stepper del cliente)
        [HttpGet("{orderId}")]
        public async Task<ActionResult<string>> GetOrderStatus(int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == orderId)
                .Select(o => o.Status)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(order))
            {
                return NotFound("Pedido no encontrado.");
            }

            return order;
        }

        // ELIMINADO: El método que generaba el Código QR ha sido eliminado.

        // Si tienes otros métodos en TrackController.cs, deben permanecer.
        // Aquí solo se muestra el método de estado para verificar que sigue funcionando.
    }
}