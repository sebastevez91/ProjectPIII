using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class DetallePedidoModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IEmailSender _emailSender;

    public DetallePedidoModel(AutoPartesRazorContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public Order Pedido { get; set; } = default!;

    // Método auxiliar para crear notificaciones (se mantiene)
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

    // MÉTODO NUEVO: Creación del Evento para la Línea de Tiempo
    private void CrearEvento(Order pedido, string status, string description, string? reference = null)
    {
        var nuevoEvento = new OrderEvent
        {
            OrderId = pedido.Id,
            Status = status,
            Description = description,
            Timestamp = DateTime.Now,
            Reference = reference
        };
        _context.OrderEvents.Add(nuevoEvento);
    }

    public IActionResult OnGet(int id)
    {
        // Añadimos Includes necesarios para la vista de detalle
        Pedido = _context.Orders
            .Include(o => o.Items)!
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            // IMPORTANTE: Incluir OrderEvents para mostrarlos en el detalle
            .Include(o => o.OrderEvents.OrderByDescending(e => e.Timestamp))
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

        CrearEvento(pedido, "Preparando", "El pedido ha sido recibido y está siendo preparado para el despacho.");
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

        CrearEvento(pedido, "Despachado", "El pedido ha sido embalado y entregado al servicio de correos.");
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

        CrearEvento(pedido, "En camino", "El paquete se encuentra en tránsito hacia la dirección de envío.");
        await CrearNotificacion(pedido.UserId, "Pedido En Camino", $"¡Buenas noticias! Tu pedido #{pedido.Id} está en camino a tu domicilio.");

        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido en camino.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEntregarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Status = "Entregado";
        pedido.UpdatedAt = DateTime.Now;

        // 1. OBTENER URL DE SEGUIMIENTO
        var trackingUrl = Url.Page(
            "/Orders/TrackOrder",
            pageHandler: null,
            values: new { id = pedido.Id },
            protocol: Request.Scheme
        );

        // 2. CREACIÓN DEL EVENTO
        CrearEvento(pedido, "Entregado", "El transportista marcó la entrega como completada.");

        // 3. CONTENIDO DEL CORREO (Aviso)
        string mensajeHtml = $@"
            <p>Estimado/a cliente,</p>
            <p>Tu pedido <strong>#{pedido.Id}</strong> ha sido marcado como <strong>Entregado</strong>.</p>
            <p>Para confirmar la recepción de los productos, por favor visita en la seccion de Ver Tracking en Mis Pedidos para Confirmar el Pedido</p>
            
            <p>Una vez allí, podrás confirmar la recepción y calificar tu experiencia. Gracias por tu compra.</p>";

        await _emailSender.SendEmailAsync(pedido.CustomerEmail, $"Aviso de Entrega Pedido #{pedido.Id}", mensajeHtml);

        // 4. ELIMINADA LA CREACIÓN DE NOTIFICACIÓN INTERNA

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

        CrearEvento(pedido, "Cancelado", "El pedido ha sido cancelado por el administrador.");
        await CrearNotificacion(pedido.UserId, "Pedido Cancelado", $"Tu pedido #{pedido.Id} ha sido cancelado.");

        await _context.SaveChangesAsync();
        TempData["orderMessage"] = "Pedido cancelado.";
        return RedirectToPage(new { id });
    }
}