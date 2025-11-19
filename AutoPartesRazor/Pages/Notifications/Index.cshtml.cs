using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Notifications;

<<<<<<< HEAD
[Authorize]
public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public IndexModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
=======
[Authorize] // Solo usuarios autenticados pueden ver notificaciones
public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public IndexModel(AutoPartesRazorContext context)
    {
        _context = context;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }

    public List<Notification> Notifications { get; set; } = new();
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; } = "all";

    public async Task OnGetAsync()
    {
<<<<<<< HEAD
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
=======
        var query = _context.Notification
            .Include(n => n.User)
            .AsQueryable();

        // Aplicar filtro
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
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

<<<<<<< HEAD
        // Contadores solo del usuario autenticado
        UnreadCount = await _context.Notifications
            .CountAsync(n => n.UserId == currentUser.Id && !n.IsRead);

        TotalCount = await _context.Notifications
            .CountAsync(n => n.UserId == currentUser.Id);
=======
        // Contadores
        UnreadCount = await _context.Notification.CountAsync(n => !n.IsRead);
        TotalCount = await _context.Notification.CountAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int id)
    {
<<<<<<< HEAD
        var currentUser = await _userManager.GetUserAsync(User);

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == currentUser.Id);

=======
        var notification = await _context.Notification.FindAsync(id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
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
<<<<<<< HEAD
        var currentUser = await _userManager.GetUserAsync(User);

        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == currentUser.Id && !n.IsRead)
=======
        var unreadNotifications = await _context.Notification
            .Where(n => !n.IsRead)
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
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
<<<<<<< HEAD
        var currentUser = await _userManager.GetUserAsync(User);

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == currentUser.Id);

=======
        var notification = await _context.Notification.FindAsync(id);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        if (notification == null)
        {
            return NotFound();
        }

<<<<<<< HEAD
        _context.Notifications.Remove(notification);
=======
        _context.Notification.Remove(notification);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Notificación eliminada.";
        return RedirectToPage();
    }
}