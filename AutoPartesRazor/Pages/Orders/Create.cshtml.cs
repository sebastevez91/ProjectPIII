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
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Orders;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
<<<<<<< HEAD
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AutoPartesRazorContext context, UserManager<User> userManager, ILogger<CreateModel> logger)
    {
        _context = context;
        _userManager = userManager;
=======
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AutoPartesRazorContext context, ILogger<CreateModel> logger)
    {
        _context = context;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
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

<<<<<<< HEAD
    public User UserSesion { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        UserSesion = user ?? new User();

        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.Product?.Price ?? 0m) * c.Quantity);
=======
    public async Task<IActionResult> OnGetAsync()
    {
        CartItems = await _context.Cart
            .Include(c => c.producto)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.producto?.price ?? 0m) * c.quantity);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
<<<<<<< HEAD
        CartItems = await _context.Carts
            .Include(c => c.Product)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.Product?.Price ?? 0m) * c.Quantity);
=======
        CartItems = await _context.Cart
            .Include(c => c.producto)
            .ToListAsync();

        Subtotal = CartItems.Sum(c => (c.producto?.price ?? 0m) * c.quantity);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

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
<<<<<<< HEAD
            if (item.Product == null)
            {
                ModelState.AddModelError(string.Empty, $"Producto {item.ProductId} no encontrado.");
                return Page();
            }
            if (item.Quantity > item.Product.Stock)
            {
                ModelState.AddModelError(string.Empty, $"No hay suficiente stock para {item.Product.Name} (disponible: {item.Product.Stock}).");
=======
            if (item.producto == null)
            {
                ModelState.AddModelError(string.Empty, $"Producto {item.productId} no encontrado.");
                return Page();
            }
            if (item.quantity > item.producto.stock)
            {
                ModelState.AddModelError(string.Empty, $"No hay suficiente stock para {item.producto.name} (disponible: {item.producto.stock}).");
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                return Page();
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                CustomerName = Input.CustomerName,
                CustomerEmail = Input.CustomerEmail,
                ShippingAddress = Input.ShippingAddress,
                PaymentMethod = Input.PaymentMethod,
                Total = Subtotal,
                CreatedAt = DateTime.UtcNow
            };

<<<<<<< HEAD
            _context.Orders.Add(order);
=======
            _context.Order.Add(order);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            await _context.SaveChangesAsync(); // order.id se genera aquí

            foreach (var item in CartItems)
            {
                var oi = new OrderItem
                {
<<<<<<< HEAD
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product?.Price ?? 0m
                };
                _context.OrderItems.Add(oi);

                // Reducir stock
                item.Product!.Stock -= item.Quantity;
                _context.Products.Update(item.Product);
=======
                    OrderId = order.id,
                    ProductId = item.productId,
                    Quantity = item.quantity,
                    UnitPrice = item.producto?.price ?? 0m
                };
                _context.OrderItem.Add(oi);

                // Reducir stock
                item.producto!.stock -= item.quantity;
                _context.Product.Update(item.producto);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            }

            // Guardar items y actualizar stock
            await _context.SaveChangesAsync();

            // Vaciar carrito
<<<<<<< HEAD
            _context.Carts.RemoveRange(CartItems);
=======
            _context.Cart.RemoveRange(CartItems);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // usar ruta absoluta para evitar ambigüedades
<<<<<<< HEAD
            return RedirectToPage("/Orders/Confirmation", new { id = order.Id });
=======
            return RedirectToPage("/Orders/Confirmation", new { id = order.id });
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
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