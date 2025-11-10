using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        public async Task OnGetAsync()
        {
            // Query base de productos
            var query = _context.Product
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            // Aplicar búsqueda por nombre o descripción
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var searchLower = SearchQuery.ToLower();
                query = query.Where(p =>
                    p.name.ToLower().Contains(searchLower) ||
                    p.description.ToLower().Contains(searchLower) ||
                    (p.Brand != null && p.Brand.name.ToLower().Contains(searchLower))
                );
            }

            // Aplicar filtro por categoría
            if (CategoryFilter.HasValue && CategoryFilter.Value > 0)
            {
                query = query.Where(p => p.idCategory == CategoryFilter.Value);
            }

            // Aplicar ordenamiento
            query = SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.price),
                "price_desc" => query.OrderByDescending(p => p.price),
                "name_asc" => query.OrderBy(p => p.name),
                "name_desc" => query.OrderByDescending(p => p.name),
                _ => query.OrderBy(p => p.id) // Orden por defecto
            };

            // Ejecutar consulta
            Products = await query.ToListAsync();

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
                // Validación: verificar que exista el contexto de productos
                if (_context.Product == null)
                {
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
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Producto no encontrado"
                    });
                }

                // Validar cantidad
                if (quantity <= 0)
                {
                    quantity = 1;
                }

                // Validar stock disponible
                if (product.stock < quantity)
                {
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
                    // Si existe, actualizar la cantidad
                    existingCartItem.quantity += quantity;
                    _context.Cart.Update(existingCartItem);
                }
                else
                {
                    // Si no existe, crear nuevo item
                    var cartItem = new Cart
                    {
                        productId = productId,
                        quantity = quantity
                    };
                    _context.Cart.Add(cartItem);
                }

                // Guardar cambios en la base de datos
                await _context.SaveChangesAsync();

                // Obtener el contador actualizado del carrito
                var cartCount = await ObtenerContadorCarritoAsync();

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
                return new JsonResult(new
                {
                    success = false,
                    message = $"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}"
                });
            }
            catch (InvalidOperationException invEx)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = $"Error de operación: {invEx.Message}"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = $"Error al agregar al carrito: {ex.Message}"
                });
            }
        }

        /// Método privado para obtener el contador de items en el carrito
        private async Task<int> ObtenerContadorCarritoAsync()
        {
            // Contar el número de items únicos en el carrito
            var count = await _context.Cart.CountAsync();
            return count;
        }
    }
}