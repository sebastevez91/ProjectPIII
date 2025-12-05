using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Orders;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AutoPartesRazorContext context, UserManager<User> userManager, ILogger<CreateModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public List<Cart> CartItems { get; set; } = new();
    public decimal Subtotal { get; set; }

    public class OrderInputModel
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = "Efectivo";
    }

    [BindProperty]
    public OrderInputModel Input { get; set; } = new();

    public User UserSesion { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        UserSesion = user ?? new User();

        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.Product?.Price ?? 0m) * c.Quantity);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.Product?.Price ?? 0m) * c.Quantity);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("OnPostAsync: ModelState inválido.");
            return Page();
        }

        if (!CartItems.Any())
        {
            ModelState.AddModelError(string.Empty, "El carrito está vacío.");
            return Page();
        }

        // Validar stock
        foreach (var item in CartItems)
        {
            if (item.Product == null)
            {
                ModelState.AddModelError(string.Empty, $"Producto {item.ProductId} no encontrado.");
                return Page();
            }
            if (item.Quantity > item.Product.Stock)
            {
                ModelState.AddModelError(string.Empty, $"No hay suficiente stock para {item.Product.Name} (disponible: {item.Product.Stock}).");
                return Page();
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // ✅ OBTENER EL USUARIO ACTUAL
            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                UserId = user?.Id,  // ✅ LÍNEA AGREGADA
                CustomerName = Input.CustomerName,
                CustomerEmail = Input.CustomerEmail,
                ShippingAddress = Input.ShippingAddress,
                PaymentMethod = Input.PaymentMethod,
                Total = Subtotal,
                CreatedAt = DateTime.UtcNow
            };

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
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al procesar el pedido en OnPostAsync");
            // Mostrar mensaje útil en la UI para depuración inicial
            ModelState.AddModelError(string.Empty, "Error al procesar el pedido. " + ex.Message + (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : ""));
            return Page();
        }
    }
}