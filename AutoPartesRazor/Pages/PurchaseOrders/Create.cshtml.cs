using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.PurchaseOrders;

public class CreateModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public CreateModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public PurchaseOrder PurchaseOrder { get; set; } = new();

    public SelectList ProductList { get; set; }
    public SelectList SupplierList { get; set; }

    // Método GET - Carga la página con datos pre-cargados opcionales
    public async Task<IActionResult> OnGetAsync(int? productId = null, int? supplierId = null)
    {
        await LoadListsAsync();

        // Si se recibe supplierId, cargar solo sus productos
        if (supplierId.HasValue)
        {
            PurchaseOrder.SupplierId = supplierId.Value;
            await LoadProductsForSupplierAsync(supplierId.Value);
        }

        // Si se recibe productId 
        if (productId.HasValue)
        {
            PurchaseOrder.ProductId = productId.Value;
            PurchaseOrder.Quantity = 1;

            // Si no se especificó proveedor, buscar el primero disponible para este producto
            if (!supplierId.HasValue)
            {
                var productSupplier = await _context.ProductSuppliers
                    .Include(ps => ps.Supplier)
                    .FirstOrDefaultAsync(ps => ps.ProductId == productId.Value);

                if (productSupplier != null)
                {
                    PurchaseOrder.SupplierId = productSupplier.SupplierId;

                    // Cargar solo proveedores que tengan este producto
                    await LoadSuppliersForProductAsync(productId.Value);

                    if (productSupplier.SupplyPrice != null)
                    {
                        ViewData["InitialPrice"] = productSupplier.SupplyPrice;
                    }
                    else
                    {
                        var product = await _context.Products.FindAsync(productId.Value);
                        if (product != null)
                        {
                            ViewData["InitialPrice"] = product.Price * 0.65m;
                        }
                    }
                }
            }
            else
            {
                // Validar que el proveedor tenga este producto
                var productSupplier = await _context.ProductSuppliers
                    .FirstOrDefaultAsync(ps => ps.ProductId == productId.Value &&
                                              ps.SupplierId == supplierId.Value);

                if (productSupplier != null && productSupplier.SupplyPrice != null)
                {
                    ViewData["InitialPrice"] = productSupplier.SupplyPrice;
                }
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validar que se hayan seleccionado producto y proveedor
        if (PurchaseOrder.ProductId <= 0)
        {
            ModelState.AddModelError("PurchaseOrder.ProductId", "Debe seleccionar un producto.");
        }

        if (PurchaseOrder.SupplierId <= 0)
        {
            ModelState.AddModelError("PurchaseOrder.SupplierId", "Debe seleccionar un proveedor.");
        }

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return Page();
        }

        // ✅ VALIDACIÓN CRÍTICA: Verificar que el proveedor tenga este producto asignado
        var productSupplier = await _context.ProductSuppliers
            .Include(ps => ps.Product)
            .Include(ps => ps.Supplier)
            .FirstOrDefaultAsync(ps =>
                ps.ProductId == PurchaseOrder.ProductId &&
                ps.SupplierId == PurchaseOrder.SupplierId);

        if (productSupplier == null)
        {
            var product = await _context.Products.FindAsync(PurchaseOrder.ProductId);
            var supplier = await _context.Suppliers.FindAsync(PurchaseOrder.SupplierId);

            ModelState.AddModelError("",
                $"El proveedor '{supplier?.Name}' no tiene asignado el producto '{product?.Name}'. " +
                "Por favor, seleccione un proveedor válido para este producto.");

            await LoadListsAsync();
            return Page();
        }

        // Obtener el producto
        var productData = productSupplier.Product ?? await _context.Products.FindAsync(PurchaseOrder.ProductId);
        if (productData == null)
        {
            ModelState.AddModelError("", "Producto no encontrado.");
            await LoadListsAsync();
            return Page();
        }

        // Calcular precio unitario (precio del proveedor o 65% del precio de venta)
        PurchaseOrder.UnitPrice = productSupplier.SupplyPrice ?? (productData.Price * 0.65m);

        // Calcular total
        PurchaseOrder.Total = PurchaseOrder.UnitPrice * PurchaseOrder.Quantity;

        // Establecer estado inicial y fecha
        PurchaseOrder.Status = "Pending";
        PurchaseOrder.CreatedAt = DateTime.Now;

        _context.PurchaseOrders.Add(PurchaseOrder);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Orden de compra #{PurchaseOrder.Id} creada exitosamente.";
        return RedirectToPage("./Index");
    }

    private async Task LoadListsAsync()
    {
        ProductList = new SelectList(
            await _context.Products
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync(),
            "Id",
            "Name"
        );

        SupplierList = new SelectList(
            await _context.Suppliers
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync(),
            "Id",
            "Name"
        );
    }

    // Cargar solo productos que el proveedor tiene asignados
    private async Task LoadProductsForSupplierAsync(int supplierId)
    {
        ProductList = new SelectList(
            await _context.ProductSuppliers
                .Where(ps => ps.SupplierId == supplierId &&
                            ps.Product != null &&
                            !ps.Product.IsDeleted)
                .Select(ps => new { ps.Product!.Id, ps.Product.Name })
                .OrderBy(p => p.Name)
                .ToListAsync(),
            "Id",
            "Name"
        );
    }

    // Cargar solo proveedores que tienen este producto
    private async Task LoadSuppliersForProductAsync(int productId)
    {
        SupplierList = new SelectList(
            await _context.ProductSuppliers
                .Where(ps => ps.ProductId == productId &&
                            ps.Supplier != null &&
                            !ps.Supplier.IsDeleted)
                .Select(ps => new { ps.Supplier!.Id, ps.Supplier.Name })
                .OrderBy(s => s.Name)
                .ToListAsync(),
            "Id",
            "Name"
        );
    }

    // ✅ NUEVO: Método para obtener productos de un proveedor vía AJAX
    public async Task<JsonResult> OnGetSupplierProductsAsync(int supplierId)
    {
        var products = await _context.ProductSuppliers
            .Where(ps => ps.SupplierId == supplierId &&
                        ps.Product != null &&
                        !ps.Product.IsDeleted)
            .Select(ps => new
            {
                id = ps.ProductId,
                name = ps.Product!.Name,
                stock = ps.Product.Stock,
                price = ps.SupplyPrice ?? (ps.Product.Price * 0.65m)
            })
            .OrderBy(p => p.name)
            .ToListAsync();

        return new JsonResult(new { success = true, products });
    }

    // ✅ NUEVO: Método para obtener proveedores de un producto vía AJAX
    public async Task<JsonResult> OnGetProductSuppliersAsync(int productId)
    {
        var suppliers = await _context.ProductSuppliers
            .Where(ps => ps.ProductId == productId &&
                        ps.Supplier != null &&
                        !ps.Supplier.IsDeleted)
            .Select(ps => new
            {
                id = ps.SupplierId,
                name = ps.Supplier!.Name,
                price = ps.SupplyPrice
            })
            .OrderBy(s => s.name)
            .ToListAsync();

        return new JsonResult(new { success = true, suppliers });
    }

    // Método mejorado para obtener el precio del proveedor vía AJAX
    public async Task<IActionResult> OnGetSupplierPriceAsync(int productId, int supplierId)
    {
        var productSupplier = await _context.ProductSuppliers
            .FirstOrDefaultAsync(ps =>
                ps.ProductId == productId &&
                ps.SupplierId == supplierId);

        // ✅ Validar que existe la relación
        if (productSupplier == null)
        {
            return new JsonResult(new
            {
                success = false,
                error = "Este proveedor no tiene asignado este producto"
            });
        }

        if (productSupplier.SupplyPrice != null)
        {
            return new JsonResult(new
            {
                success = true,
                price = productSupplier.SupplyPrice,
                hasRelation = true
            });
        }

        // Si no hay precio del proveedor, obtener precio del producto
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            return new JsonResult(new
            {
                success = true,
                price = product.Price * 0.65m,
                hasRelation = true
            });
        }

        return new JsonResult(new { success = false });
    }
}