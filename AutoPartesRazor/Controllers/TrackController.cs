using AutoPartesRazor.Data;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
    private readonly AutoPartesRazorContext _context;
    public TrackController(AutoPartesRazorContext context) => _context = context;

    [HttpGet("{id}")]
    public IActionResult GetTracking(int id)
    {
        var pedido = _context.Orders.Find(id);
        if (pedido == null) return NotFound();
        return Ok(new { id = pedido.Id, estado = pedido.Status });
    }
}
