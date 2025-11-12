using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Notifications;

[Authorize] // Solo usuarios autenticados pueden ver notificaciones
public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public IndexModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public List<Notification> Notifications { get; set; } = new();
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; } = "all";

    public async Task OnGetAsync()
    {
        var query = _context.Notification
            .Include(n => n.User)
            .AsQueryable();

        // Aplicar filtro
        query = Filter switch
        {
            "unread" => query.Where(n => !n.IsRead),
            "read" => query.Where(n => n.IsRead),
            _ => query // "all"
        };

        // Obtener notificaciones ordenadas por fecha
        Notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        // Contadores
        UnreadCount = await _context.Notification.CountAsync(n => !n.IsRead);
        TotalCount = await _context.Notification.CountAsync();
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int id)
    {
        var notification = await _context.Notification.FindAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Notificación marcada como leída.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMarkAllAsReadAsync()
    {
        var unreadNotifications = await _context.Notification
            .Where(n => !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"{unreadNotifications.Count} notificaciones marcadas como leídas.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var notification = await _context.Notification.FindAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        _context.Notification.Remove(notification);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Notificación eliminada.";
        return RedirectToPage();
    }
}