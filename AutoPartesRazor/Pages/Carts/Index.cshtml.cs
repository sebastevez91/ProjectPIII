using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Carts;

public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public IndexModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Cart> Cart { get; set; } = default!;
    public int CartCount { get; set; } = 0;

    // Propiedades para cupones
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public Coupon? AppliedCoupon { get; set; }

    [BindProperty]
    public string CouponCode { get; set; } = string.Empty;

    [TempData]
    public string? CouponMessage { get; set; }

    [TempData]
    public string? AppliedCouponCode { get; set; }

    public async Task OnGetAsync()
    {
        // Contar items en carrito
        var count = await _context.Carts.CountAsync();
        CartCount = count;

        if (_context.Carts != null)
        {
            Cart = await _context.Carts
                .Include(c => c.Product)
                .ToListAsync();
        }

        // Calcular subtotal
        Subtotal = Cart.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);
        Total = Subtotal;

        // Verificar si hay cupón aplicado
        if (!string.IsNullOrEmpty(AppliedCouponCode))
        {
            await ApplyCouponAsync(AppliedCouponCode);
        }
    }

    /// <summary>
    /// Aplicar cupón de descuento
    /// </summary>
    public async Task<IActionResult> OnPostApplyCouponAsync()
    {
        if (string.IsNullOrWhiteSpace(CouponCode))
        {
            TempData["ErrorMessage"] = "Debes ingresar un código de cupón.";
            return RedirectToPage();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Debes iniciar sesión para usar cupones.";
            return RedirectToPage("/Account/Login");
        }

        // Buscar cupón
        var coupon = await _context.Coupons
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c =>
                c.Code == CouponCode.Trim().ToUpper() &&
                c.UserId == user.Id);

        if (coupon == null)
        {
            TempData["ErrorMessage"] = "Cupón no válido o no te pertenece.";
            return RedirectToPage();
        }

        // Validar cupón
        if (coupon.IsUsed)
        {
            TempData["ErrorMessage"] = $"Este cupón ya fue usado el {coupon.UsedAt:dd/MM/yyyy}.";
            return RedirectToPage();
        }

        if (!coupon.IsActive)
        {
            TempData["ErrorMessage"] = "Este cupón está inactivo.";
            return RedirectToPage();
        }

        if (coupon.ExpiresAt < DateTime.Now)
        {
            TempData["ErrorMessage"] = $"Este cupón expiró el {coupon.ExpiresAt:dd/MM/yyyy}.";
            return RedirectToPage();
        }

        // Si el cupón es para un producto específico, validar que esté en el carrito
        if (coupon.ProductId.HasValue)
        {
            var cart = await _context.Carts
                .Include(c => c.Product)
                .ToListAsync();

            if (!cart.Any(c => c.ProductId == coupon.ProductId.Value))
            {
                TempData["ErrorMessage"] = $"Este cupón solo aplica para el producto: {coupon.Product?.Name}";
                return RedirectToPage();
            }
        }

        // Cupón válido, guardarlo en TempData
        AppliedCouponCode = CouponCode.Trim().ToUpper();
        TempData["SuccessMessage"] = $"¡Cupón aplicado! Descuento del {coupon.DiscountPercentage}%";

        return RedirectToPage();
    }

    /// <summary>
    /// Remover cupón aplicado
    /// </summary>
    public async Task<IActionResult> OnPostRemoveCouponAsync()
    {
        AppliedCouponCode = null;
        TempData["SuccessMessage"] = "Cupón removido correctamente.";
        return RedirectToPage();
    }

    /// <summary>
    /// Aplicar cupón y calcular descuento
    /// </summary>
    private async Task ApplyCouponAsync(string couponCode)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        var coupon = await _context.Coupons
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c =>
                c.Code == couponCode &&
                c.UserId == user.Id &&
                c.IsActive &&
                !c.IsUsed &&
                c.ExpiresAt > DateTime.Now);

        if (coupon == null) return;

        AppliedCoupon = coupon;

        // Calcular descuento
        if (coupon.ProductId.HasValue)
        {
            // Descuento solo en producto específico
            var cartItem = Cart.FirstOrDefault(c => c.ProductId == coupon.ProductId.Value);
            if (cartItem != null && cartItem.Product != null)
            {
                var productTotal = cartItem.Product.Price * cartItem.Quantity;
                DiscountAmount = productTotal * (coupon.DiscountPercentage / 100m);
            }
        }
        else
        {
            // Descuento en todo el carrito
            DiscountAmount = Subtotal * (coupon.DiscountPercentage / 100m);
        }

        Total = Subtotal - DiscountAmount;
    }
}