using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

<<<<<<< HEAD
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

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadListsAsync();
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

        // Obtener el producto para calcular el precio
        var product = await _context.Products.FindAsync(PurchaseOrder.ProductId);
        if (product == null)
        {
            ModelState.AddModelError("", "Producto no encontrado.");
            await LoadListsAsync();
            return Page();
        }

        // Buscar el precio del proveedor en la relación ProductSupplier
        var productSupplier = await _context.ProductSuppliers
            .FirstOrDefaultAsync(ps =>
                ps.ProductId == PurchaseOrder.ProductId &&
                ps.SupplierId == PurchaseOrder.SupplierId);

        // Calcular precio unitario (precio del proveedor o 65% del precio de venta)
        PurchaseOrder.UnitPrice = productSupplier?.SupplyPrice ?? (product.Price * 0.65m);

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

    // Método para obtener el precio del proveedor vía AJAX
    public async Task<IActionResult> OnGetSupplierPriceAsync(int productId, int supplierId)
    {
        var productSupplier = await _context.ProductSuppliers
            .FirstOrDefaultAsync(ps =>
                ps.ProductId == productId &&
                ps.SupplierId == supplierId);

        if (productSupplier?.SupplyPrice != null)
        {
            return new JsonResult(new
            {
                success = true,
                price = productSupplier.SupplyPrice
            });
        }

        // Si no hay precio del proveedor, obtener precio del producto
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            return new JsonResult(new
            {
                success = true,
                price = product.Price * 0.65m
            });
        }

        return new JsonResult(new { success = false });
    }
}
=======
namespace AutoPartesRazor.Pages.PurchaseOrders
{
    public class CreateModel : PageModel
    {
        private readonly AutoPartesRazorContext _context;

        public CreateModel(AutoPartesRazorContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PurchaseOrder purchaseOrder { get; set; } = new();

        public SelectList ProductList { get; set; }
        public SelectList SupplierList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = new SelectList(await _context.Product
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.name)
                .ToListAsync(), "id", "name");

            SupplierList = new SelectList(await _context.Supplier
                .OrderBy(s => s.Name)
                .ToListAsync(), "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLists();
                return Page();
            }

            _context.PurchaseOrder.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadLists()
        {
            ProductList = new SelectList(await _context.Product.Where(p => !p.IsDeleted).ToListAsync(), "id", "name");
            SupplierList = new SelectList(await _context.Supplier.ToListAsync(), "Id", "Name");
        }
    }
}
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
