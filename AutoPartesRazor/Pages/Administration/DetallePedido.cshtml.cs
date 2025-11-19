using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Interfaces; // Importar IEmailSender
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class DetallePedidoModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IEmailSender _emailSender; // 1. Inyectar servicio de Email

    public DetallePedidoModel(AutoPartesRazorContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public Order Pedido { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        // NOTA: Podrías necesitar incluir Pedido.User si usas el UserId
        Pedido = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (Pedido == null) return NotFound();
        return Page();
    }

    // Método auxiliar para enviar correo y notificar al cliente
    private async Task NotificarCambioEstado(Order pedido, string nuevoEstado)
    {
        string asunto = $"Actualización de tu pedido #{pedido.Id}";
        string mensaje = $"Hola {pedido.CustomerName},<br/>" +
                         $"El estado de tu pedido ha cambiado a: <b>{nuevoEstado}</b><br/>" +
                         $"Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm}";

        await _emailSender.SendEmailAsync(pedido.CustomerEmail, asunto, mensaje);
    }

    // --- MANEJO DE ESTADOS CON VALIDACIÓN DE FLUJO ---

    public async Task<IActionResult> OnPostPrepararAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        // 2. VALIDACIÓN: Solo pasa si está "Pendiente" o "Pending" (según tu modelo)
        if (pedido.Status != "Pendiente" && pedido.Status != "Pending")
        {
            TempData["orderMessage"] = $"Error: El pedido debe estar en estado 'Pendiente' para pasar a 'Preparando'. Estado actual: {pedido.Status}.";
            return RedirectToPage(new { id });
        }

        pedido.Status = "Preparando"; // <-- ¡ESTE ES EL ESTADO CORRECTO!
        pedido.UpdatedAt = DateTime.Now;

        await NotificarCambioEstado(pedido, "Preparando"); // Enviar Email
        await _context.SaveChangesAsync();

        TempData["orderMessage"] = "Pedido marcado como 'Preparando'";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDespacharAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        // 2. VALIDACIÓN: Solo pasa si está Preparando
        if (pedido.Status != "Preparando")
        {
            TempData["orderMessage"] = $"Error: El pedido debe estar en estado 'Preparando' antes de despachar. Estado actual: {pedido.Status}.";
            return RedirectToPage(new { id });
        }

        pedido.Status = "Despachado";
        pedido.UpdatedAt = DateTime.Now;

        await NotificarCambioEstado(pedido, "Despachado");
        await _context.SaveChangesAsync();

        TempData["orderMessage"] = "Pedido despachado.";
        return RedirectToPage(new { id });
    }

    // (El resto de los handlers deben tener validación similar para asegurar la secuencia)

    public async Task<IActionResult> OnPostEnCaminoAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        if (pedido.Status != "Despachado") return RedirectToPage(new { id }); // Validacion

        pedido.Status = "En camino";
        pedido.UpdatedAt = DateTime.Now;

        await NotificarCambioEstado(pedido, "En camino");
        await _context.SaveChangesAsync();

        TempData["orderMessage"] = "Pedido en camino.";
        return RedirectToPage(new { id });
    }

    // (Mantén los handlers OnPostEntregarAsync y OnPostCancelarAsync, agregándoles la notificación por email)

    public async Task<IActionResult> OnPostEntregarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Status = "Entregado";
        pedido.UpdatedAt = DateTime.Now;

        await NotificarCambioEstado(pedido, "Entregado");
        await _context.SaveChangesAsync();

        TempData["orderMessage"] = "Pedido entregado exitosamente.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelarAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Status = "Cancelado";
        pedido.UpdatedAt = DateTime.Now;

        await NotificarCambioEstado(pedido, "Cancelado");
        await _context.SaveChangesAsync();

        TempData["orderMessage"] = "Pedido cancelado.";
        return RedirectToPage(new { id });
    }
}