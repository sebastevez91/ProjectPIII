<<<<<<< HEAD
﻿using AutoPartesRazor.Data;
using Microsoft.AspNetCore.Mvc;
=======
﻿using Microsoft.AspNetCore.Mvc;
using AutoPartesRazor.Data;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
    private readonly AutoPartesRazorContext _context;
    public TrackController(AutoPartesRazorContext context) => _context = context;

    [HttpGet("{id}")]
    public IActionResult GetTracking(int id)
    {
<<<<<<< HEAD
        var pedido = _context.Orders.Find(id);
        if (pedido == null) return NotFound();
        return Ok(new { id = pedido.Id, estado = pedido.Status });
=======
        var pedido = _context.Order.Find(id);
        if (pedido == null) return NotFound();
        return Ok(new { id = pedido.id, estado = pedido.Status });
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }
}
