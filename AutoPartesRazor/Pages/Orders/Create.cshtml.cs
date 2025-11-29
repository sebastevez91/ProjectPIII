using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Orders;

public class CreateModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public CreateModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Cart> CartItems { get; set; } = new List<Cart>();
    public User UserSesion { get; set; } = default!;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public Coupon? AppliedCoupon { get; set; }

    [BindProperty]
    public OrderInputModel Input { get; set; } = new();

    // CAMBIO: Usar BindProperty con SupportsGet para mantener el cupón
    [BindProperty(SupportsGet = true)]
    public string? AppliedCouponCode { get; set; }

    public class OrderInputModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre completo")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de envío es requerida")]
        [StringLength(200)]
        [Display(Name = "Dirección de envío")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un método de pago")]
        [Display(Name = "Método de pago")]
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserSesion = user;

        // Cargar items del carrito
        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        if (!CartItems.Any())
        {
            return Page();
        }

        // Calcular subtotal
        Subtotal = CartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);
        Total = Subtotal;

        // Aplicar cupón si existe
        if (!string.IsNullOrEmpty(AppliedCouponCode))
        {
            await LoadAppliedCouponAsync(AppliedCouponCode, user.Id);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserSesion = user;

        // Cargar items del carrito nuevamente
        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        if (!CartItems.Any())
        {
            ModelState.AddModelError(string.Empty, "No hay productos en el carrito.");
            return Page();
        }

        // Calcular totales
        Subtotal = CartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);
        Total = Subtotal;

        // Verificar si hay cupón aplicado
        Coupon? couponToUse = null;
        if (!string.IsNullOrEmpty(AppliedCouponCode))
        {
            couponToUse = await _context.Coupons
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c =>
                    c.Code == AppliedCouponCode &&
                    c.UserId == user.Id &&
                    c.IsActive &&
                    !c.IsUsed &&
                    c.ExpiresAt > DateTime.Now);

            if (couponToUse != null)
            {
                AppliedCoupon = couponToUse;
                DiscountAmount = CalculateDiscount(couponToUse);
                Total = Subtotal - DiscountAmount;
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Crear la orden
        var order = new Order
        {
            UserId = user.Id,
            CustomerName = Input.CustomerName,
            CustomerEmail = Input.CustomerEmail,
            ShippingAddress = Input.ShippingAddress,
            PaymentMethod = Input.PaymentMethod,
            OriginalTotal = Subtotal,
            DiscountAmount = DiscountAmount,
            Total = Total,
            Status = "Pendiente",
            CreatedAt = DateTime.Now
        };

        // Asignar datos del cupón si existe
        if (couponToUse != null)
        {
            order.CouponId = couponToUse.Id;
            order.CouponCode = couponToUse.Code;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Crear los OrderItems
        foreach (var cartItem in CartItems)
        {
            if (cartItem.Product == null) continue;

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price,
                Subtotal = cartItem.Product.Price * cartItem.Quantity
            };

            _context.OrderItems.Add(orderItem);
        }

        // Marcar el cupón como usado
        if (couponToUse != null)
        {
            couponToUse.IsUsed = true;
            couponToUse.UsedAt = DateTime.Now;
            _context.Coupons.Update(couponToUse);
        }

        // Limpiar el carrito
        _context.Carts.RemoveRange(CartItems);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"¡Pedido #{order.Id} creado exitosamente!";

        return RedirectToPage("/Orders/Details", new { id = order.Id });
    }

    /// <summary>
    /// Cargar cupón aplicado
    /// </summary>
    private async Task LoadAppliedCouponAsync(string couponCode, string userId)
    {
        var coupon = await _context.Coupons
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c =>
                c.Code == couponCode &&
                c.UserId == userId &&
                c.IsActive &&
                !c.IsUsed &&
                c.ExpiresAt > DateTime.Now);

        if (coupon != null)
        {
            AppliedCoupon = coupon;
            DiscountAmount = CalculateDiscount(coupon);
            Total = Subtotal - DiscountAmount;
        }
    }

    /// <summary>
    /// Calcular descuento según el tipo de cupón
    /// </summary>
    private decimal CalculateDiscount(Coupon coupon)
    {
        if (coupon.ProductId.HasValue)
        {
            // Descuento solo en producto específico
            var cartItem = CartItems.FirstOrDefault(c => c.ProductId == coupon.ProductId.Value);
            if (cartItem != null && cartItem.Product != null)
            {
                var productTotal = cartItem.Product.Price * cartItem.Quantity;
                return productTotal * (coupon.DiscountPercentage / 100m);
            }
            return 0;
        }
        else
        {
            // Descuento en todo el carrito
            return Subtotal * (coupon.DiscountPercentage / 100m);
        }
    }
}