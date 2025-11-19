using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Interfaces; // Importar IEmailSender
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class TrackOrderModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IEmailSender _emailSender; // Inyectamos EmailSender

    public TrackOrderModel(AutoPartesRazorContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public Order Pedido { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        Pedido = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (Pedido == null) return NotFound();

        if (User.IsInRole("Admin") || Pedido.CustomerEmail == User.Identity?.Name)
            return Page();

        return Forbid();
    }

    // Handler para cuando el CLIENTE confirma la entrega
    public async Task<IActionResult> OnPostConfirmarEntregaAsync(int id)
    {
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null || pedido.Status != "En camino") return NotFound();

        // 1. Actualizar estado del pedido
        pedido.Status = "Entregado";
        pedido.UpdatedAt = DateTime.Now;

        // 2. Enviar Email al Administrador
        string adminEmail = "admin@tutienda.com"; // Pon aquí el email real del admin
        string asunto = $"¡Entrega Confirmada! - Pedido #{pedido.Id}";
        string mensaje = $"El cliente {pedido.CustomerName} ha confirmado la recepción del pedido #{pedido.Id}.<br/>Fecha: {DateTime.Now}";

        await _emailSender.SendEmailAsync(adminEmail, asunto, mensaje);

        // 3. Crear Notificación para el Admin (CORREGIDO)
        // Buscamos al usuario Admin (asumiendo que hay uno, o usamos un ID fijo si lo conoces)
        var adminUser = _context.Users.FirstOrDefault(u => u.UserName == "admin@admin.com" || u.Email == "admin@admin.com");

        if (adminUser != null) // Solo creamos la notificación si encontramos al admin
        {
            var notificacion = new Notification
            {
                UserId = adminUser.Id, // Usamos el ID real del admin
                Title = "Entrega Confirmada", // <--- Faltaba esto
                Message = $"El cliente {pedido.CustomerName} confirmó la recepción del pedido #{pedido.Id}.",
                CreatedAt = DateTime.Now,     // <--- Corregido: Date -> CreatedAt
                IsRead = false
            };
            _context.Notifications.Add(notificacion);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    // Handler de Calificación (ya lo tenías, solo lo dejo igual)
    public async Task<IActionResult> OnPostAsync(int id)
    {
        // ... tu código existente de calificación ...
        var pedido = await _context.Orders.FindAsync(id);
        if (pedido == null) return NotFound();

        if (Request.Form.TryGetValue("Calificacion", out var calif) && int.TryParse(calif, out int nota))
        {
            pedido.Calificacion = nota;
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }
}