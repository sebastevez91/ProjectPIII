using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Administration;

[Authorize(Roles = "Admin")]
public class ReviewsDashboardModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public ReviewsDashboardModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

<<<<<<< HEAD
    // Estadísticas generales
=======
    // EstadÃ­sticas generales
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public int ProductsWithReviews { get; set; }
    public int TotalProducts { get; set; }
    public int CriticalProducts { get; set; }

    // Listas de productos
    public List<Product> TopProducts { get; set; } = new();
    public List<Product> BottomProducts { get; set; } = new();
    public List<Product> DiscountedProducts { get; set; } = new();

<<<<<<< HEAD
    // Estadísticas por proveedor
=======
    // EstadÃ­sticas por proveedor
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    public List<SupplierStatistics> SupplierStats { get; set; } = new();

    public async Task OnGetAsync()
    {
<<<<<<< HEAD
        // Cargar productos con sus reseñas
        var products = await _context.Products
=======
        // Cargar productos con sus reseÃ±as
        var products = await _context.Product
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
            .Include(p => p.Reviews)
            .Include(p => p.Brand)
            .Include(p => p.ProductSuppliers!)
                .ThenInclude(ps => ps.Supplier)
            .Where(p => !p.IsDeleted)
            .ToListAsync();

<<<<<<< HEAD
        // Estadísticas generales
=======
        // EstadÃ­sticas generales
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        TotalProducts = products.Count;
        var productsWithReviews = products.Where(p => p.TotalReviews > 0).ToList();
        ProductsWithReviews = productsWithReviews.Count;
        TotalReviews = productsWithReviews.Sum(p => p.TotalReviews);
        AverageRating = productsWithReviews.Any()
            ? Math.Round(productsWithReviews.Average(p => p.AverageRating), 1)
            : 0;
        CriticalProducts = productsWithReviews.Count(p => p.AverageRating < 3.0);

<<<<<<< HEAD
        // TOP 10 productos - LOS 10 PORCENTAJES MÁS ALTOS (sin importar estrellas ni cantidad de reseñas)
        TopProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 1) // Mínimo 1 reseña para aparecer
            .OrderByDescending(p => p.PositiveReviewsPercentage) // Ordenar por % positivo
            .ThenByDescending(p => p.TotalReviews) // Desempate: más reseñas es mejor
            .Take(10)
            .ToList();

        // Productos críticos - Mínimo 1 reseña para detectar problemas rápido
        BottomProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 1 && p.AverageRating < 3.5)
            .OrderBy(p => p.AverageRating)
            .ThenBy(p => p.TotalReviews)
            .ToList();

        // PRODUCTOS CON DESCUENTO - TODOS los productos sin filtro de calificación
        // Mostrar todos los productos que tengan reseñas (descuento se aplica en vista)
        DiscountedProducts = productsWithReviews
            .OrderBy(p => p.AverageRating) // Ordenar por peor calificación primero
            .ThenByDescending(p => p.TotalReviews) // Luego por más reseñas
            .ToList();

        // Estadísticas por proveedor
        var suppliers = await _context.Suppliers.ToListAsync();
=======
        // TOP 10 productos (mÃ­nimo 5 reseÃ±as)
        TopProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 5)
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.TotalReviews)
            .Take(10)
            .ToList();

        // Productos crÃ­ticos (calificaciÃ³n < 3.5 y mÃ­nimo 5 reseÃ±as)
        BottomProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 5 && p.AverageRating < 3.5)
            .OrderBy(p => p.AverageRating)
            .ToList();

        // Productos con descuento automÃ¡tico (calificaciÃ³n < 3.5 y mÃ­nimo 5 reseÃ±as)
        DiscountedProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 5 && p.AverageRating < 3.5)
            .OrderBy(p => p.AverageRating)
            .ToList();

        // EstadÃ­sticas por proveedor
        var suppliers = await _context.Supplier.ToListAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        foreach (var supplier in suppliers)
        {
            var supplierProducts = products
                .Where(p => p.ProductSuppliers != null &&
                           p.ProductSuppliers.Any(ps => ps.SupplierId == supplier.Id) &&
                           p.TotalReviews > 0)
                .ToList();

            if (supplierProducts.Any())
            {
                var stat = new SupplierStatistics
                {
                    SupplierName = supplier.Name,
                    AverageRating = Math.Round(supplierProducts.Average(p => p.AverageRating), 1),
                    TotalProducts = supplierProducts.Count,
                    GoodProducts = supplierProducts.Count(p => p.AverageRating >= 4.0),
                    RegularProducts = supplierProducts.Count(p => p.AverageRating >= 3.0 && p.AverageRating < 4.0),
                    CriticalProducts = supplierProducts.Count(p => p.AverageRating < 3.0),
                    SatisfactionIndex = supplierProducts.Any()
                        ? (int)((supplierProducts.Count(p => p.AverageRating >= 4.0) / (double)supplierProducts.Count) * 100)
                        : 0
                };

                SupplierStats.Add(stat);
            }
        }

        SupplierStats = SupplierStats.OrderByDescending(s => s.AverageRating).ToList();
    }
}

<<<<<<< HEAD
// Clase auxiliar para estadísticas de proveedores
=======
// Clase auxiliar para estadÃ­sticas de proveedores
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
public class SupplierStatistics
{
    public string SupplierName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalProducts { get; set; }
    public int GoodProducts { get; set; }
    public int RegularProducts { get; set; }
    public int CriticalProducts { get; set; }
    public int SatisfactionIndex { get; set; }
}