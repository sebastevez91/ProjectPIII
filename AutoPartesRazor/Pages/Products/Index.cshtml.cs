using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models;

namespace AutoPartesRazor.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public IndexModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public int CartCount { get; set; } = 0;

        public async Task OnGetAsync()
        {
            // Cargar productos con sus relaciones
            Products = await _context.Product
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();

            // Cargar categorías para el filtro
            Categories = await _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.name
                })
                .ToListAsync();

            // Obtener el contador del carrito
            CartCount = await ObtenerContadorCarritoAsync();

            // Actualizar ViewData
            ViewData["CartCount"] = CartCount;
        }

        /// Handler AJAX para añadir productos al carrito
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== Iniciando AddToCart - ProductId: {productId}, Quantity: {quantity} ===");

                // Validación: verificar que exista el contexto de productos
                if (_context.Product == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Contexto de productos es null");
                    return new JsonResult(new
                    {
                        success = false,
                        message = "No hay productos disponibles"
                    });
                }

                // Buscar el producto
                var product = await _context.Product.FindAsync(productId);
                if (product == null)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Producto {productId} no encontrado");
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Producto no encontrado"
                    });
                }

                System.Diagnostics.Debug.WriteLine($"Producto encontrado: {product.name}");

                // Validar cantidad
                if (quantity <= 0)
                {
                    quantity = 1;
                }

                // Validar stock disponible
                if (product.stock < quantity)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Stock insuficiente. Solicitado: {quantity}, Disponible: {product.stock}");
                    return new JsonResult(new
                    {
                        success = false,
                        message = $"Stock insuficiente. Disponible: {product.stock}"
                    });
                }

                // Verificar si el producto ya existe en el carrito
                var existingCartItem = await _context.Cart
                    .FirstOrDefaultAsync(c => c.productId == productId);

                if (existingCartItem != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Producto ya existe en carrito. Cantidad anterior: {existingCartItem.quantity}");
                    // Si existe, actualizar la cantidad
                    existingCartItem.quantity += quantity;
                    _context.Cart.Update(existingCartItem);
                    System.Diagnostics.Debug.WriteLine($"Nueva cantidad: {existingCartItem.quantity}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Creando nuevo item en carrito");
                    // Si no existe, crear nuevo item
                    var cartItem = new Cart
                    {
                        productId = productId,
                        quantity = quantity
                    };
                    _context.Cart.Add(cartItem);
                }

                // Guardar cambios en la base de datos
                System.Diagnostics.Debug.WriteLine("Guardando cambios en BD...");
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine("Cambios guardados exitosamente");

                // Obtener el contador actualizado del carrito
                System.Diagnostics.Debug.WriteLine("Obteniendo contador del carrito...");
                var cartCount = await ObtenerContadorCarritoAsync();
                System.Diagnostics.Debug.WriteLine($"Contador del carrito: {cartCount}");

                System.Diagnostics.Debug.WriteLine("=== AddToCart completado exitosamente ===");

                // Retornar respuesta exitosa
                return new JsonResult(new
                {
                    success = true,
                    cartCount = cartCount,
                    message = "Producto agregado al carrito exitosamente"
                });
            }
            catch (DbUpdateException dbEx)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR DE BASE DE DATOS: {dbEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {dbEx.StackTrace}");

                return new JsonResult(new
                {
                    success = false,
                    message = $"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}"
                });
            }
            catch (InvalidOperationException invEx)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR DE OPERACIÓN INVÁLIDA: {invEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {invEx.StackTrace}");

                return new JsonResult(new
                {
                    success = false,
                    message = $"Error de operación: {invEx.Message}"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR GENERAL: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new JsonResult(new
                {
                    success = false,
                    message = $"Error al agregar al carrito: {ex.Message}"
                });
            }
        }

        /// Método para obtener el contador de items en el carrito
        private async Task<int> ObtenerContadorCarritoAsync()
        {
            // Contar el número de items únicos en el carrito
            var count = await _context.Cart.CountAsync();
            return count;
        }
    }
}