using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class ManageCouponsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public ManageCouponsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    // Propiedades para la vista
    public List<Coupon> Coupons { get; set; } = new();
    public int TotalCoupons { get; set; }
    public int ActiveCoupons { get; set; }
    public int UsedCoupons { get; set; }
    public int ExpiredCoupons { get; set; }

    // Filtros
    [BindProperty(SupportsGet = true)]
    public string StatusFilter { get; set; } = "all";

    /// <summary>
    /// Cargar cupones con filtros aplicados
    /// </summary>
    public async Task OnGetAsync()
    {
        // Query base con todas las relaciones necesarias
        var query = _context.Coupons
            .Include(c => c.User)
            .Include(c => c.Product)
            .Include(c => c.Review)
            .Include(c => c.Order)
            .AsQueryable();

        // Aplicar filtros según selección
        switch (StatusFilter.ToLower())
        {
            case "active":
                // Cupones activos, no usados y no expirados
                query = query.Where(c => c.IsActive && !c.IsUsed && c.ExpiresAt > DateTime.Now);
                break;

            case "used":
                // Cupones que ya fueron utilizados
                query = query.Where(c => c.IsUsed);
                break;

            case "expired":
                // Cupones expirados que no fueron usados
                query = query.Where(c => !c.IsUsed && c.ExpiresAt <= DateTime.Now);
                break;

            case "inactive":
                // Cupones desactivados manualmente
                query = query.Where(c => !c.IsActive);
                break;

            case "all":
            default:
                // Sin filtro, mostrar todos
                break;
        }

        // Ordenar por fecha de creación (más recientes primero)
        Coupons = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        // Calcular estadísticas generales
        await LoadStatisticsAsync();
    }

    /// <summary>
    /// Desactivar un cupón específico
    /// </summary>
    public async Task<IActionResult> OnPostDeactivateAsync(int id)
    {
        var coupon = await _context.Coupons
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (coupon == null)
        {
            TempData["ErrorMessage"] = "Cupón no encontrado.";
            return RedirectToPage();
        }

        if (coupon.IsUsed)
        {
            TempData["ErrorMessage"] = "No se puede desactivar un cupón que ya fue usado.";
            return RedirectToPage();
        }

        // Desactivar el cupón
        coupon.IsActive = false;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Cupón {coupon.Code} desactivado correctamente.";
        return RedirectToPage();
    }

    /// <summary>
    /// Eliminar un cupón (solo si no ha sido usado)
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var coupon = await _context.Coupons
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (coupon == null)
        {
            TempData["ErrorMessage"] = "Cupón no encontrado.";
            return RedirectToPage();
        }

        // Validar que no esté usado
        if (coupon.IsUsed)
        {
            TempData["ErrorMessage"] = $"No se puede eliminar el cupón {coupon.Code} porque ya fue usado por {coupon.User?.FullName}.";
            return RedirectToPage();
        }

        // Eliminar el cupón
        _context.Coupons.Remove(coupon);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Cupón {coupon.Code} eliminado correctamente.";
        return RedirectToPage();
    }

    /// <summary>
    /// Reactivar un cupón desactivado (si no está expirado ni usado)
    /// </summary>
    public async Task<IActionResult> OnPostReactivateAsync(int id)
    {
        var coupon = await _context.Coupons
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (coupon == null)
        {
            TempData["ErrorMessage"] = "Cupón no encontrado.";
            return RedirectToPage();
        }

        if (coupon.IsUsed)
        {
            TempData["ErrorMessage"] = "No se puede reactivar un cupón que ya fue usado.";
            return RedirectToPage();
        }

        if (coupon.ExpiresAt <= DateTime.Now)
        {
            TempData["ErrorMessage"] = "No se puede reactivar un cupón expirado.";
            return RedirectToPage();
        }

        // Reactivar el cupón
        coupon.IsActive = true;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Cupón {coupon.Code} reactivado correctamente.";
        return RedirectToPage();
    }

    /// <summary>
    /// Extender la fecha de expiración de un cupón
    /// </summary>
    public async Task<IActionResult> OnPostExtendExpirationAsync(int id, int additionalDays)
    {
        if (additionalDays <= 0 || additionalDays > 365)
        {
            TempData["ErrorMessage"] = "Los días adicionales deben estar entre 1 y 365.";
            return RedirectToPage();
        }

        var coupon = await _context.Coupons
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (coupon == null)
        {
            TempData["ErrorMessage"] = "Cupón no encontrado.";
            return RedirectToPage();
        }

        if (coupon.IsUsed)
        {
            TempData["ErrorMessage"] = "No se puede extender un cupón que ya fue usado.";
            return RedirectToPage();
        }

        // Extender la fecha de expiración
        var oldExpiration = coupon.ExpiresAt;
        coupon.ExpiresAt = coupon.ExpiresAt.AddDays(additionalDays);

        // Si estaba expirado, reactivarlo automáticamente
        if (oldExpiration < DateTime.Now && !coupon.IsActive)
        {
            coupon.IsActive = true;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Cupón {coupon.Code} extendido por {additionalDays} días. Nueva expiración: {coupon.ExpiresAt:dd/MM/yyyy}.";
        return RedirectToPage();
    }

    /// <summary>
    /// Cargar estadísticas generales de cupones
    /// </summary>
    private async Task LoadStatisticsAsync()
    {
        TotalCoupons = await _context.Coupons.CountAsync();

        ActiveCoupons = await _context.Coupons
            .CountAsync(c => c.IsActive && !c.IsUsed && c.ExpiresAt > DateTime.Now);

        UsedCoupons = await _context.Coupons
            .CountAsync(c => c.IsUsed);

        ExpiredCoupons = await _context.Coupons
            .CountAsync(c => !c.IsUsed && c.ExpiresAt <= DateTime.Now);
    }
}