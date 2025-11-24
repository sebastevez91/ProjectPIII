using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class DetallePedidoModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IEmailSender _emailSender;

    // Constructor: Solo inyectamos Context e IEmailSender (Solución Estable)
    public DetallePedidoModel(AutoPartesRazorContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    [BindProperty]
    public Order Pedido { get; set; } = default!;

    // Método auxiliar simplificado: ENVÍA EMAIL y actualiza el estado.
    private async Task HandleStatusChangeAsync(Order pedido, string nuevoEstado)
    {
        string asunto;
        string mensaje;
        string fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // 1. Lógica de mensajes específicos por estado
        switch (nuevoEstado)
        {
            case "Preparando":
                asunto = $"Tu pedido #{pedido.Id} se está preparando.";
                mensaje = $"Hola {pedido.CustomerName}, ¡hemos comenzado a **preparar** tu pedido! Te notificaremos cuando salga a ruta. Fecha y hora: {fechaHora}";
                break;

            case "Despachado":
                asunto = $"Tu pedido #{pedido.Id} ha sido despachado del almacén.";
                mensaje = $"Hola {pedido.CustomerName}, tu pedido ha sido **despachado** y está listo para ser recogido. Fecha y hora: {fechaHora}";
                break;

            case "En camino":
                asunto = $"¡Tu pedido #{pedido.Id} está En camino! ??";
                mensaje = $"Hola {pedido.CustomerName}, ¡tu pedido está en **ruta hacia ti**! Debería llegar pronto. Fecha y hora: {fechaHora}";
                break;

            case "Entregado":
                asunto = $"¡Pedido #{pedido.Id} Entregado! Califica tu Experiencia.";
                mensaje = $"¡Felicidades, {pedido.CustomerName}! Tu pedido ha sido marcado como **Entregado**. Puedes calificarnos en tu sección de pedidos. Fecha y hora: {fechaHora}";
                break;

            case "Cancelado":
                asunto = $"Pedido #{pedido.Id} Cancelado.";
                mensaje = $"Tu pedido ha sido cancelado. Si tienes preguntas, contacta a soporte. Fecha y hora: {fechaHora}";
                break;

            default:
                asunto = $"Actualización de estado de pedido #{pedido.Id}.";
                mensaje = $"El estado de tu pedido ha cambiado a: <b>{nuevoEstado}</b>. Fecha y hora: {fechaHora}";
                break;
        }

        // 2. Enviar Email
        await _emailSender.SendEmailAsync(pedido.CustomerEmail, asunto, mensaje);

        // 3. Actualizar estado y guardar cambios
        pedido.Status = nuevoEstado;
        pedido.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    // ONGET (Carga el pedido para el Stepper y la vista)
    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        // CARGA COMPLETA: Incluye OrderItems y el detalle de cada Producto (Clave para estabilidad)
        Pedido = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Pedido == null) return NotFound();
        return Page();
    }

    // HANDLERS POST (Validación y Actualización)

    public async Task<IActionResult> OnPostPrepararAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || (pedido.Status != "Pendiente" && pedido.Status != "Pending"))
        {
            TempData["error"] = $"Error: Debe estar 'Pendiente'. Estado actual: {pedido?.Status}.";
            return RedirectToPage(new { id });
        }

        await HandleStatusChangeAsync(pedido, "Preparando");
        TempData["orderMessage"] = "Pedido marcado como 'Preparando'";
        return RedirectToPage(new { id }); // Recarga la página y actualiza el Stepper
    }

    public async Task<IActionResult> OnPostDespacharAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || pedido.Status != "Preparando")
        {
            TempData["error"] = $"Error: Debe estar en estado 'Preparando' antes de despachar. Estado actual: {pedido?.Status}.";
            return RedirectToPage(new { id });
        }

        await HandleStatusChangeAsync(pedido, "Despachado");
        TempData["orderMessage"] = "Pedido despachado.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEnCaminoAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || pedido.Status != "Despachado")
        {
            TempData["error"] = $"Error: Debe estar en estado 'Despachado'. Estado actual: {pedido?.Status}.";
            return RedirectToPage(new { id });
        }

        await HandleStatusChangeAsync(pedido, "En camino");
        TempData["orderMessage"] = "Pedido en camino.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEntregarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || pedido.Status != "En camino")
        {
            TempData["error"] = $"Error: Debe estar en estado 'En camino' para marcar como Entregado. Estado actual: {pedido?.Status}.";
            return RedirectToPage(new { id });
        }

        await HandleStatusChangeAsync(pedido, "Entregado");
        TempData["orderMessage"] = "Pedido entregado exitosamente.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        await HandleStatusChangeAsync(pedido, "Cancelado");
        TempData["orderMessage"] = "Pedido cancelado.";
        return RedirectToPage(new { id });
    }
}