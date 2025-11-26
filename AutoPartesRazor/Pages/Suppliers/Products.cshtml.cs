using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Suppliers;

public class ProductsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public ProductsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public Supplier? Supplier { get; set; }
    public IList<ProductSupplierInfo> Products { get; set; } = new List<ProductSupplierInfo>();
    public SupplierStatistics Statistics { get; set; } = new SupplierStatistics();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Supplier = await _context.Suppliers
            .Include(s => s.ProductSuppliers)
                .ThenInclude(ps => ps.Product)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (Supplier == null)
        {
            TempData["ErrorMessage"] = "Proveedor no encontrado.";
            return RedirectToPage("Index");
        }

        // Obtener productos con información adicional
        Products = await _context.ProductSuppliers
            .Where(ps => ps.SupplierId == id)
            .Include(ps => ps.Product)
            .Select(ps => new ProductSupplierInfo
            {
                ProductId = ps.ProductId,
                ProductName = ps.Product.Name,
<<<<<<< HEAD
=======
             
>>>>>>> 6c968c799d1273b6064b0d47bc19199e0f4594cc
                CurrentStock = ps.Product.Stock,
                SupplyPrice = ps.SupplyPrice,
                SalePrice = ps.Product.Price,
                LastPurchaseDate = _context.PurchaseOrders
                    .Where(po => po.ProductId == ps.ProductId && po.SupplierId == id)
                    .OrderByDescending(po => po.CreatedAt)
                    .Select(po => po.CreatedAt)
                    .FirstOrDefault(),
                TotalPurchased = _context.PurchaseOrders
                    .Where(po => po.ProductId == ps.ProductId && po.SupplierId == id && po.Status == "Received")
                    .Sum(po => (int?)po.Quantity) ?? 0,
                PendingOrders = _context.PurchaseOrders
                    .Count(po => po.ProductId == ps.ProductId && po.SupplierId == id &&
                                (po.Status == "Pending" || po.Status == "Approved"))
            })
            .ToListAsync();

        // Calcular estadísticas
        var allOrders = await _context.PurchaseOrders
            .Where(po => po.SupplierId == id)
            .ToListAsync();

        Statistics = new SupplierStatistics
        {
            TotalProducts = Products.Count,
            TotalOrders = allOrders.Count,
            TotalReceived = allOrders.Count(o => o.Status == "Received"),
            TotalPending = allOrders.Count(o => o.Status == "Pending" || o.Status == "Approved"),
            TotalAmount = allOrders.Where(o => o.Status == "Received").Sum(o => o.Total),
            ActiveClaims = await _context.SupplierClaims
                .CountAsync(c => c.SupplierId == id &&
                    (c.Status == ClaimStatus.Pending || c.Status == ClaimStatus.InProgress))
        };

        return Page();
    }
}

public class ProductSupplierInfo
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public int CurrentStock { get; set; }
    public decimal? SupplyPrice { get; set; }
    public decimal SalePrice { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public int TotalPurchased { get; set; }
    public int PendingOrders { get; set; }
}

public class SupplierStatistics
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalReceived { get; set; }
    public int TotalPending { get; set; }
    public decimal TotalAmount { get; set; }
    public int ActiveClaims { get; set; }
}