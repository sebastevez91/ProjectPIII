using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
<<<<<<< HEAD
using Microsoft.AspNetCore.Authorization;
=======
>>>>>>> main
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
<<<<<<< HEAD
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AutoPartesRazorContext context, UserManager<User> userManager, ILogger<CreateModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
=======

    public CreateModel(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
>>>>>>> main
    }

    public IList<Cart> CartItems { get; set; } = new List<Cart>();
    public User UserSesion { get; set; } = default!;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public Coupon? AppliedCoupon { get; set; }

    [BindProperty]
    public OrderInputModel Input { get; set; } = new();

<<<<<<< HEAD
    public User UserSesion { get; set; } = new();
=======
    [TempData]
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
>>>>>>> main

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
<<<<<<< HEAD

        UserSesion = user ?? new User();

=======
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        UserSesion = user;

        // Cargar items del carrito
>>>>>>> main
        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

<<<<<<< HEAD
        Subtotal = CartItems.Sum(c => (c.Product?.Price ?? 0m) * c.Quantity);
=======
        if (!CartItems.Any())
        {
            return Page();
        }

        // Calcular subtotal
        Subtotal = CartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);
        Total = Subtotal;

        // Aplicar cupón si existe en TempData
        if (!string.IsNullOrEmpty(AppliedCouponCode))
        {
            await LoadAppliedCouponAsync(AppliedCouponCode, user.Id);
        }

>>>>>>> main
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
<<<<<<< HEAD
        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.Product?.Price ?? 0m) * c.Quantity);

        if (!ModelState.IsValid)
=======
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
>>>>>>> main
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
<<<<<<< HEAD
            if (item.Product == null)
            {
                ModelState.AddModelError(string.Empty, $"Producto {item.ProductId} no encontrado.");
                return Page();
            }
            if (item.Quantity > item.Product.Stock)
            {
                ModelState.AddModelError(string.Empty, $"No hay suficiente stock para {item.Product.Name} (disponible: {item.Product.Stock}).");
                return Page();
=======
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
>>>>>>> main
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
            Status = "Pending",
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

<<<<<<< HEAD
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // order.id se genera aquí

            foreach (var item in CartItems)
            {
                var oi = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product?.Price ?? 0m
                };
                _context.OrderItems.Add(oi);

                // Reducir stock
                item.Product!.Stock -= item.Quantity;
                _context.Products.Update(item.Product);
            }

            // Guardar items y actualizar stock
            await _context.SaveChangesAsync();

            // Vaciar carrito
            _context.Carts.RemoveRange(CartItems);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // usar ruta absoluta para evitar ambigüedades
            return RedirectToPage("/Orders/Confirmation", new { id = order.Id });
=======
            _context.OrderItems.Add(orderItem);
>>>>>>> main
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

        // Limpiar el cupón aplicado de TempData
        AppliedCouponCode = null;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"¡Pedido #{order.Id} creado exitosamente!";

        return RedirectToPage("/Orders/Details", new { id = order.Id });
    }

    /// <summary>
    /// Cargar cupón aplicado desde TempData
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