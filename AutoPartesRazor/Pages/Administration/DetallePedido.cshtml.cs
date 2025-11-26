using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Interfaces; // Necesario para IEmailSender
using Microsoft.EntityFrameworkCore; // Necesario para Include y DateTime

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class DetallePedidoModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IEmailSender _emailSender; // Inyección de IEmailSender

    // Constructor modificado
    public DetallePedidoModel(AutoPartesRazorContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public Order Pedido { get; set; } = default!;

    // Asumimos que CrearNotificacion existe en una capa de negocio o la definimos aquí
    private async Task CrearNotificacion(string? userId, string titulo, string mensaje)
    {
        if (string.IsNullOrEmpty(userId)) return;

        var notificacion = new Notification
        {
            UserId = userId,
            Title = titulo,
            Message = mensaje,
            CreatedAt = DateTime.Now,
            IsRead = false
        };
        _context.Notifications.Add(notificacion);
    }

    public IActionResult OnGet(int id)
    {
        // Añadir Includes necesarios para el detalle, ej. items y usuario
        Pedido = _context.Orders
            .Include(o => o.Items)!
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .FirstOrDefault(o => o.Id == id);

        if (Pedido == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostPrepararAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();
        pedido.Status = "Preparando";
        pedido.UpdatedAt = DateTime.Now;
        await CrearNotificacion(pedido.UserId, "Pedido en Preparación", $"Tu pedido #{pedido.Id} se está preparando.");
        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido marcado como 'Preparando'";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDespacharAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();
        pedido.Status = "Despachado";
        pedido.UpdatedAt = DateTime.Now;
        await CrearNotificacion(pedido.UserId, "Pedido Despachado", $"Tu pedido #{pedido.Id} ha sido despachado.");
        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido despachado.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEnCaminoAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();
        pedido.Status = "En camino";
        pedido.UpdatedAt = DateTime.Now;
        await CrearNotificacion(pedido.UserId, "Pedido En Camino", $"Tu pedido #{pedido.Id} está en camino a tu domicilio.");
        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido en camino.";
        return RedirectToPage(new { id });
    }

    // HANDLER MODIFICADO: Solo Envía Email de Aviso
    public async Task<IActionResult> OnPostEntregarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Status = "Entregado";
        pedido.UpdatedAt = DateTime.Now;
        // pedido.DeliveryDate = DateTime.Now; // La fecha real se establece al confirmar el cliente

        // 1. ELIMINADA LA NOTIFICACIÓN INTERNA

        // 2. MODIFICAR EL CONTENIDO DEL EMAIL
        var trackingUrl = Url.Page(
            "/Orders/TrackOrder",
            pageHandler: null,
            values: new { id = pedido.Id },
            protocol: Request.Scheme
        );

        // CONTENIDO DEL CORREO SIN BOTONES DE CONFIRMACIÓN
        string mensajeHtml = $@"
            <p>Estimado/a cliente,</p>
            <p>Tu pedido <strong>#{pedido.Id}</strong> ha sido marcado como <strong>Entregado</strong>.</p>
            <p>
                Para confirmar la recepción de los productos o ver el detalle completo de tu pedido, por favor visita el Ver Tracking para confirmar el pedido.
            </p>
            <p>Gracias por tu compra.</p>";

        await _emailSender.SendEmailAsync(pedido.CustomerEmail, $"Aviso de Entrega Pedido #{pedido.Id}", mensajeHtml);

        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido marcado como Entregado. Se envió correo de aviso al cliente.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();
        pedido.Status = "Cancelado";
        pedido.UpdatedAt = DateTime.Now;
        await CrearNotificacion(pedido.UserId, "Pedido Cancelado", $"Tu pedido #{pedido.Id} ha sido cancelado.");
        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido cancelado.";
        return RedirectToPage(new { id });
    }
}