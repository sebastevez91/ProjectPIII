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

    // Estadísticas generales
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public int ProductsWithReviews { get; set; }
    public int TotalProducts { get; set; }
    public int CriticalProducts { get; set; }

    // Listas de productos
    public List<Product> TopProducts { get; set; } = new();
    public List<Product> BottomProducts { get; set; } = new();
    public List<Product> DiscountedProducts { get; set; } = new();

    // Estadísticas por proveedor
    public List<SupplierStatistics> SupplierStats { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Cargar productos con sus reseñas
        var products = await _context.Products
            .Include(p => p.Reviews)
            .Include(p => p.Brand)
            .Include(p => p.ProductSuppliers!)
                .ThenInclude(ps => ps.Supplier)
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        // Estadísticas generales
        TotalProducts = products.Count;
        var productsWithReviews = products.Where(p => p.TotalReviews > 0).ToList();
        ProductsWithReviews = productsWithReviews.Count;
        TotalReviews = productsWithReviews.Sum(p => p.TotalReviews);
        AverageRating = productsWithReviews.Any()
            ? Math.Round(productsWithReviews.Average(p => p.AverageRating), 1)
            : 0;
        CriticalProducts = productsWithReviews.Count(p => p.AverageRating < 3.0);

        // TOP 10 productos - CAMBIO: Mínimo 3 reseñas en lugar de 5
        TopProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 3)
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.TotalReviews)
            .Take(10)
            .ToList();

        // Productos críticos - CAMBIO: Mínimo 1 reseña para detectar problemas rápido
        BottomProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 1 && p.AverageRating < 3.5)
            .OrderBy(p => p.AverageRating)
            .ThenBy(p => p.TotalReviews) // Priorizar los que tienen más reseñas
            .ToList();

        // Productos con descuento automático - CAMBIO: Mínimo 2 reseñas para activar descuentos
        DiscountedProducts = productsWithReviews
            .Where(p => p.TotalReviews >= 2 && p.AverageRating < 3.5)
            .OrderBy(p => p.AverageRating)
            .ThenByDescending(p => p.TotalReviews)
            .ToList();

        // Estadísticas por proveedor
        var suppliers = await _context.Suppliers.ToListAsync();
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

// Clase auxiliar para estadísticas de proveedores
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