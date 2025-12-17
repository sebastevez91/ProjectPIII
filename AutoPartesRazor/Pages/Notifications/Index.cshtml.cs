using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Notifications;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public IndexModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Notification> Notifications { get; set; } = new();
    public int CartCount { get; set; } = 0;
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; } = "all";

    public async Task OnGetAsync()
    {
        // Obtener el usuario autenticado
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return;
        }

        // Filtrar notificaciones solo del usuario autenticado
        var query = _context.Notifications
            .Where(n => n.UserId == currentUser.Id)
            .AsQueryable();

        // Aplicar filtro adicional
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

        // Contadores solo del usuario autenticado
        UnreadCount = await _context.Notifications
            .CountAsync(n => n.UserId == currentUser.Id && !n.IsRead);

        TotalCount = await _context.Notifications
            .CountAsync(n => n.UserId == currentUser.Id);

        // Obtener el contador del carrito
        CartCount = await ObtenerContadorCarritoAsync();

        // Actualizar ViewData
        ViewData["CartCount"] = CartCount;
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == currentUser.Id);

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
        var currentUser = await _userManager.GetUserAsync(User);

        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == currentUser.Id && !n.IsRead)
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
        var currentUser = await _userManager.GetUserAsync(User);

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == currentUser.Id);

        if (notification == null)
        {
            return NotFound();
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Notificación eliminada.";
        return RedirectToPage();
    }

    /// Método privado para obtener el contador de items en el carrito
    private async Task<int> ObtenerContadorCarritoAsync()
    {
        // Contar el número de items únicos en el carrito
        var count = await _context.Carts.CountAsync();
        return count;
    }
}