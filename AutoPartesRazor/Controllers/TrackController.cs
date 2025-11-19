using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesRazor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
    private readonly AutoPartesRazorContext _context;

    public TrackController(AutoPartesRazorContext context)
    {
        _context = context;
    }

    // GET: api/track/5
    [HttpGet("{id}")]
    public IActionResult GetEstadoPedido(int id)
    {
        // Buscamos el pedido
        var pedido = _context.Orders
            .Where(o => o.Id == id)
            .Select(o => new
            {
                OrderId = o.Id,
                Customer = o.CustomerName,
                Status = o.Status,
                LastUpdate = o.UpdatedAt ?? o.CreatedAt,
                Total = o.Total
            })
            .FirstOrDefault();

        if (pedido == null)
        {
            return NotFound(new { mensaje = "Pedido no encontrado" });
        }

        // Retornamos JSON (código 200 OK)
        return Ok(pedido);
    }
}