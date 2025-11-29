using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class CouponsFromReviewsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public CouponsFromReviewsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public List<CouponFromReviewViewModel> EligibleReviews { get; set; } = new();
    public int TotalEligibleUsers { get; set; }
    public decimal PotentialDiscountValue { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ProductId { get; set; }

    public string? ProductName { get; set; }

    [BindProperty]
    public int SelectedReviewId { get; set; }

    [BindProperty]
    public int CustomDiscount { get; set; }

    [BindProperty]
    public int DaysValid { get; set; } = 30;

    [BindProperty]
    public bool ApplyToProduct { get; set; } = true;

    public async Task OnGetAsync()
    {
        await LoadEligibleReviewsAsync();
    }

    /// <summary>
    /// Generar cupón individual para una reseña específica
    /// </summary>
    public async Task<IActionResult> OnPostGenerateSingleAsync()
    {
        var review = await _context.ProductReviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == SelectedReviewId);

        if (review == null)
        {
            TempData["ErrorMessage"] = "Reseña no encontrada.";
            return RedirectToPage();
        }

        // Verificar si ya tiene cupón para este producto
        var existingCoupon = await _context.Coupons
            .FirstOrDefaultAsync(c =>
                c.UserId == review.UserId &&
                c.ReviewId == SelectedReviewId &&
                c.IsActive);

        if (existingCoupon != null)
        {
            TempData["ErrorMessage"] = $"El usuario ya tiene un cupón activo ({existingCoupon.Code}) para esta reseña.";
            return RedirectToPage();
        }

        // Calcular descuento si no se especificó uno personalizado
        int discount = CustomDiscount > 0 ? CustomDiscount : CalculateDiscount(review.Rating);

        // Generar código único
        string couponCode = GenerateUniqueCouponCode();

        // Crear cupón
        var coupon = new Coupon
        {
            Code = couponCode,
            DiscountPercentage = discount,
            UserId = review.UserId,
            ProductId = ApplyToProduct ? review.ProductId : null,
            ReviewId = review.Id,
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddDays(DaysValid),
            IsActive = true,
            Reason = $"Compensación por reseña de {review.Rating} estrellas en {review.Product?.Name}"
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        // ✅ ENVIAR NOTIFICACIÓN AL USUARIO
        await CreateCouponNotificationAsync(coupon, review.UserId.ToString());

        TempData["SuccessMessage"] = $"Cupón {couponCode} creado y notificado exitosamente a {review.User?.FullName} con {discount}% de descuento.";
        return RedirectToPage();
    }

    /// <summary>
    /// Generar cupones masivos para todas las reseñas seleccionadas
    /// </summary>
    public async Task<IActionResult> OnPostGenerateBulkAsync(List<int> selectedReviews, int bulkDaysValid = 30)
    {
        if (selectedReviews == null || !selectedReviews.Any())
        {
            TempData["ErrorMessage"] = "Debes seleccionar al menos una reseña.";
            return RedirectToPage();
        }

        var reviews = await _context.ProductReviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => selectedReviews.Contains(r.Id))
            .ToListAsync();

        int createdCount = 0;
        int skippedCount = 0;
        int notifiedCount = 0;

        foreach (var review in reviews)
        {
            // Verificar si ya tiene cupón
            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c =>
                    c.UserId == review.UserId &&
                    c.ReviewId == review.Id &&
                    c.IsActive);

            if (existingCoupon != null)
            {
                skippedCount++;
                continue;
            }

            int discount = CalculateDiscount(review.Rating);
            string couponCode = GenerateUniqueCouponCode();

            var coupon = new Coupon
            {
                Code = couponCode,
                DiscountPercentage = discount,
                UserId = review.UserId,
                ProductId = review.ProductId,
                ReviewId = review.Id,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(bulkDaysValid),
                IsActive = true,
                Reason = $"Compensación masiva por reseña de {review.Rating} estrellas"
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            // ✅ ENVIAR NOTIFICACIÓN AL USUARIO
            try
            {
                await CreateCouponNotificationAsync(coupon, review.UserId.ToString());
                notifiedCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notificando cupón {couponCode}: {ex.Message}");
            }

            createdCount++;
        }

        TempData["SuccessMessage"] = $"✅ Cupones generados: {createdCount} | 📧 Notificaciones enviadas: {notifiedCount} | ⏭️ Omitidos: {skippedCount}";
        return RedirectToPage();
    }

    /// <summary>
    /// ✅ MÉTODO PARA CREAR NOTIFICACIONES DE CUPONES
    /// </summary>
    private async Task CreateCouponNotificationAsync(Coupon coupon, string userId)
    {
        // Cargar información del producto si el cupón está asociado a uno
        var productName = "toda tu compra";
        if (coupon.ProductId.HasValue)
        {
            var product = await _context.Products.FindAsync(coupon.ProductId.Value);
            if (product != null)
            {
                productName = product.Name;
            }
        }

        var message = $@"¡Tienes un nuevo cupón de descuento!

🎟️ CÓDIGO: {coupon.Code}
💰 Descuento: {coupon.DiscountPercentage}% OFF
📅 Válido hasta: {coupon.ExpiresAt:dd/MM/yyyy}

{(coupon.ProductId.HasValue ? $"Aplica solo a: {productName}" : "Aplica a toda tu compra")}";

        var notification = new Notification
        {
            UserId = userId,
            Title = "🎟️ Nuevo cupón disponible",
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.Now
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    private async Task LoadEligibleReviewsAsync()
    {
        var query = _context.ProductReviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.Rating <= 3);

        if (ProductId.HasValue)
        {
            query = query.Where(r => r.ProductId == ProductId.Value);
            var product = await _context.Products.FindAsync(ProductId.Value);
            ProductName = product?.Name;
        }

        var negativeReviews = await query
            .OrderBy(r => r.Rating)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();

        var existingCoupons = await _context.Coupons
            .Where(c => c.IsActive)
            .ToListAsync();

        EligibleReviews = negativeReviews.Select(r =>
        {
            var existingCoupon = existingCoupons
                .FirstOrDefault(c => c.UserId == r.UserId && c.ReviewId == r.Id);

            return new CouponFromReviewViewModel
            {
                ReviewId = r.Id,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? "Usuario desconocido",
                UserEmail = r.User?.Email ?? "",
                ProductId = r.ProductId,
                ProductName = r.Product?.Name ?? "Producto desconocido",
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.CreatedAt,
                SuggestedDiscount = CalculateDiscount(r.Rating),
                HasExistingCoupon = existingCoupon != null,
                ExistingCouponCode = existingCoupon?.Code
            };
        }).ToList();

        TotalEligibleUsers = EligibleReviews.DistinctBy(r => r.UserId).Count();
        PotentialDiscountValue = EligibleReviews
            .Where(r => !r.HasExistingCoupon)
            .Sum(r => r.SuggestedDiscount);
    }

    private int CalculateDiscount(int rating)
    {
        return rating switch
        {
            1 => 40,
            2 => 30,
            3 => 20,
            _ => 10
        };
    }

    private string GenerateUniqueCouponCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789";
        var random = new Random();
        string code;

        do
        {
            code = "COMP-" + new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        while (_context.Coupons.Any(c => c.Code == code));

        return code;
    }
}