using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public CreateModel(AutoPartesRazorContext context)
        {
            _context = context;
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

        public async Task<IActionResult> OnGetAsync()
        {
            CartItems = await _context.Cart
                .Include(c => c.producto)
                .ToListAsync();

            Subtotal = CartItems.Sum(c => (c.producto?.price ?? 0m) * c.quantity);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CartItems = await _context.Cart
                .Include(c => c.producto)
                .ToListAsync();

            Subtotal = CartItems.Sum(c => (c.producto?.price ?? 0m) * c.quantity);

            if (!ModelState.IsValid)
            {
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
                if (item.producto == null)
                {
                    ModelState.AddModelError(string.Empty, $"Producto {item.productId} no encontrado.");
                    return Page();
                }
                if (item.quantity > item.producto.stock)
                {
                    ModelState.AddModelError(string.Empty, $"No hay suficiente stock para {item.producto.name} (disponible: {item.producto.stock}).");
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

                _context.Order.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in CartItems)
                {
                    var oi = new OrderItem
                    {
                        OrderId = order.id,
                        ProductId = item.productId,
                        Quantity = item.quantity,
                        UnitPrice = item.producto?.price ?? 0m
                    };
                    _context.OrderItem.Add(oi);

                    // Reducir stock
                    item.producto!.stock -= item.quantity;
                    _context.Product.Update(item.producto);
                }

                // Guardar items y actualizar stock
                await _context.SaveChangesAsync();

                // Vaciar carrito
                _context.Cart.RemoveRange(CartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return RedirectToPage("./Confirmation", new { id = order.id });
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Error al procesar el pedido. Intenta de nuevo.");
                return Page();
            }
        }
    }
}