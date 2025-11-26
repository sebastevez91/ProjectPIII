using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Suppliers;

[Authorize]
public class AssignProductsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public AssignProductsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    // Productos ya asignados al proveedor
    public List<ProductSupplier> AssignedProducts { get; set; } = new();

    // Productos disponibles para asignar (no asignados aún)
    public List<Product> AvailableProducts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int supplierId)
    {
        // Verificar que el proveedor existe
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == supplierId && !s.IsDeleted);

        if (supplier == null)
        {
            TempData["ErrorMessage"] = "Proveedor no encontrado.";
            return RedirectToPage("./Index");
        }

        SupplierId = supplierId;
        SupplierName = supplier.Name;

        // Cargar productos asignados
        AssignedProducts = await _context.ProductSuppliers
            .Include(ps => ps.Product)
            .Where(ps => ps.SupplierId == supplierId &&
                        ps.Product != null &&
                        !ps.Product.IsDeleted)
            .OrderBy(ps => ps.Product!.Name)
            .ToListAsync();

        // Cargar productos disponibles (no asignados a este proveedor)
        var assignedProductIds = AssignedProducts.Select(ps => ps.ProductId).ToList();

        AvailableProducts = await _context.Products
            .Where(p => !p.IsDeleted && !assignedProductIds.Contains(p.Id))
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAssignAsync(int supplierId, int productId, decimal supplyPrice)
    {
        // Validaciones básicas
        if (productId <= 0)
        {
            TempData["ErrorMessage"] = "Debe seleccionar un producto válido.";
            return RedirectToPage(new { supplierId });
        }

        if (supplyPrice <= 0)
        {
            TempData["ErrorMessage"] = "El precio de suministro debe ser mayor a 0.";
            return RedirectToPage(new { supplierId });
        }

        // Verificar que el producto existe y no está eliminado
        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.IsDeleted)
        {
            TempData["ErrorMessage"] = "El producto no existe.";
            return RedirectToPage(new { supplierId });
        }

        // Verificar que el proveedor existe
        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier == null || supplier.IsDeleted)
        {
            TempData["ErrorMessage"] = "El proveedor no existe.";
            return RedirectToPage("./Index");
        }

        // Verificar que no existe ya la relación
        var exists = await _context.ProductSuppliers
            .AnyAsync(ps => ps.ProductId == productId && ps.SupplierId == supplierId);

        if (exists)
        {
            TempData["ErrorMessage"] = "Este producto ya está asignado a este proveedor.";
            return RedirectToPage(new { supplierId });
        }

        // Validar que el precio de suministro sea menor al precio de venta
        if (supplyPrice >= product.Price)
        {
            TempData["ErrorMessage"] = $"El precio de suministro (${supplyPrice:N2}) debe ser menor al precio de venta (${product.Price:N2}).";
            return RedirectToPage(new { supplierId });
        }

        // Crear la relación
        var productSupplier = new ProductSupplier
        {
            ProductId = productId,
            SupplierId = supplierId,
            SupplyPrice = supplyPrice
        };

        _context.ProductSuppliers.Add(productSupplier);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Producto '{product.Name}' asignado exitosamente con precio de suministro ${supplyPrice:N2}.";
        return RedirectToPage(new { supplierId });
    }

    public async Task<IActionResult> OnPostUpdatePriceAsync(int supplierId, int productId, decimal supplyPrice)
    {
        // Validar precio
        if (supplyPrice <= 0)
        {
            TempData["ErrorMessage"] = "El precio de suministro debe ser mayor a 0.";
            return RedirectToPage(new { supplierId });
        }

        // Buscar la relación
        var productSupplier = await _context.ProductSuppliers
            .Include(ps => ps.Product)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.SupplierId == supplierId);

        if (productSupplier == null)
        {
            TempData["ErrorMessage"] = "No se encontró la asignación.";
            return RedirectToPage(new { supplierId });
        }

        // Validar que el precio de suministro sea menor al precio de venta
        if (productSupplier.Product != null && supplyPrice >= productSupplier.Product.Price)
        {
            TempData["ErrorMessage"] = $"El precio de suministro (${supplyPrice:N2}) debe ser menor al precio de venta (${productSupplier.Product.Price:N2}).";
            return RedirectToPage(new { supplierId });
        }

        // Actualizar precio
        var oldPrice = productSupplier.SupplyPrice;
        productSupplier.SupplyPrice = supplyPrice;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Precio actualizado de ${oldPrice:N2} a ${supplyPrice:N2}.";
        return RedirectToPage(new { supplierId });
    }

    public async Task<IActionResult> OnPostRemoveAsync(int supplierId, int productId)
    {
        // Buscar la relación
        var productSupplier = await _context.ProductSuppliers
            .Include(ps => ps.Product)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.SupplierId == supplierId);

        if (productSupplier == null)
        {
            TempData["ErrorMessage"] = "No se encontró la asignación.";
            return RedirectToPage(new { supplierId });
        }

        // Verificar si hay órdenes de compra pendientes o aprobadas
        var hasPendingOrders = await _context.PurchaseOrders
            .AnyAsync(po => po.ProductId == productId &&
                           po.SupplierId == supplierId &&
                           (po.Status == "Pending" || po.Status == "Approved"));

        if (hasPendingOrders)
        {
            TempData["ErrorMessage"] = "No se puede eliminar. Existen órdenes de compra pendientes o aprobadas para este producto.";
            return RedirectToPage(new { supplierId });
        }

        var productName = productSupplier.Product?.Name ?? "el producto";

        // Eliminar la relación
        _context.ProductSuppliers.Remove(productSupplier);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{productName}' fue desasignado del proveedor exitosamente.";
        return RedirectToPage(new { supplierId });
    }
}