using Microsoft.AspNetCore.Mvc;
using AutoPartesRazor.Data;

[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
    private readonly AutoPartesRazorContext _context;
    public TrackController(AutoPartesRazorContext context) => _context = context;

    [HttpGet("{id}")]
    public IActionResult GetTracking(int id)
    {
        var pedido = _context.Order.Find(id);
        if (pedido == null) return NotFound();
        return Ok(new { id = pedido.id, estado = pedido.Status });
    }
}
