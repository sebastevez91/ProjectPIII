using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class DespachoModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IEmailSender _emailSender;

    public DespachoModel(AutoPartesRazorContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    [BindProperty]
    public IList<Order> PedidosPendientes { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        // CORRECCIÓN: Traemos todos los estados activos para que la lista no se vacíe
        PedidosPendientes = await _context.Orders
            // Incluimos Pendiente (y Pending), Preparando, Despachado y En camino
            .Where(o => (o.Status == "Pendiente" || o.Status == "Pending") || o.Status == "Preparando" || o.Status == "Despachado" || o.Status == "En camino")
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // NOTA: Los Handlers OnPost simples deben estar aquí para que los botones de la lista Despacho funcionen.
    private async Task HandleStatusChangeAndNotifyAsync(Order pedido, string nuevoEstado)
    {
        // Lógica simplificada de notificación (copia de DetallePedido para no fallar)
        string asunto = $"Actualización de estado de pedido #{pedido.Id}.";
        string mensaje = $"El estado de tu pedido ha cambiado a: <b>{nuevoEstado}</b>. Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm}";

        await _emailSender.SendEmailAsync(pedido.CustomerEmail, asunto, mensaje);

        pedido.Status = nuevoEstado;
        pedido.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<IActionResult> OnPostPrepararAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || (pedido.Status != "Pendiente" && pedido.Status != "Pending"))
        {
            TempData["DespachoOK"] = "Error: El pedido debe estar en estado 'Pendiente' para iniciar la preparación.";
            return RedirectToPage();
        }

        await HandleStatusChangeAndNotifyAsync(pedido, "Preparando");
        TempData["DespachoOK"] = $"Pedido #{id} marcado como 'Preparando'.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDespacharAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || pedido.Status != "Preparando")
        {
            TempData["DespachoOK"] = "Error: El pedido debe estar en estado 'Preparando' antes de ser despachado.";
            return RedirectToPage();
        }

        await HandleStatusChangeAndNotifyAsync(pedido, "Despachado");
        TempData["DespachoOK"] = $"Pedido #{id} actualizado a Despachado.";
        return RedirectToPage();
    }

    // ... (otros handlers simples si los necesitas en esta vista)
}