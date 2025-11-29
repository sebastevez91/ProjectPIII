using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.PurchaseOrders;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public IndexModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public SelectList StatusList { get; set; }
    public SelectList SupplierFilterList { get; set; }
    public SelectList StatusFilterList { get; set; }

    // Propiedades para los filtros
    public int? SelectedSupplierId { get; set; }
    public string SelectedStatus { get; set; }
    public string SearchTerm { get; set; }
    public string SupplierName { get; set; } // Para mostrar en el título

    // Estadísticas
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ApprovedOrders { get; set; }
    public int ReceivedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalAmount { get; set; }

    public async Task OnGetAsync(int? supplierId = null, string status = null, string search = null)
    {
        // Guardar los filtros actuales
        SelectedSupplierId = supplierId;
        SelectedStatus = status;
        SearchTerm = search;

        // Si hay un proveedor seleccionado, obtener su nombre
        if (supplierId.HasValue)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId.Value);
            SupplierName = supplier?.Name;
        }

        // Construir la consulta base
        var query = _context.PurchaseOrders
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .AsQueryable();

        // Aplicar filtro por proveedor
        if (supplierId.HasValue)
        {
            query = query.Where(o => o.SupplierId == supplierId.Value);
        }

        // Aplicar filtro por estado
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(o => o.Status == status);
        }

        // Aplicar búsqueda por código de orden
        if (!string.IsNullOrEmpty(search))
        {
            if (int.TryParse(search, out int orderId))
            {
                query = query.Where(o => o.Id == orderId);
            }
            else
            {
                // Buscar también en nombre de producto o proveedor
                query = query.Where(o =>
                    o.Product.Name.Contains(search) ||
                    o.Supplier.Name.Contains(search));
            }
        }

        // Obtener las órdenes
        PurchaseOrders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        // Calcular estadísticas basadas en los resultados filtrados
        TotalOrders = PurchaseOrders.Count;
        PendingOrders = PurchaseOrders.Count(o => o.Status == "Pending");
        ApprovedOrders = PurchaseOrders.Count(o => o.Status == "Approved");
        ReceivedOrders = PurchaseOrders.Count(o => o.Status == "Received");
        CancelledOrders = PurchaseOrders.Count(o => o.Status == "Cancelled");
        TotalAmount = PurchaseOrders.Sum(o => o.Total);

        // Cargar listas para los filtros
        await LoadFilterLists();

        // Lista de estados disponibles para cambio
        StatusList = new SelectList(new[]
        {
            new { Value = "Pending", Text = "Pendiente" },
            new { Value = "Approved", Text = "Aprobado" },
            new { Value = "Cancelled", Text = "Cancelado" }
        }, "Value", "Text");
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(int orderId, string status)
    {
        if (string.IsNullOrEmpty(status))
        {
            TempData["ErrorMessage"] = "Debe seleccionar un estado.";
            return RedirectToPage();
        }

        var order = await _context.PurchaseOrders.FindAsync(orderId);
        if (order == null)
        {
            TempData["ErrorMessage"] = "Orden no encontrada.";
            return RedirectToPage();
        }

        var oldStatus = order.Status;
        order.Status = status;

        // Si la orden es recibida, actualizar el stock del producto
        if (status == "Received" && order.ProductId > 0)
        {
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product != null)
            {
                product.Stock += order.Quantity;
            }
        }

        // Si se cancela una orden que ya estaba recibida, revertir el stock
        if (status == "Cancelled" && oldStatus == "Received" && order.ProductId > 0)
        {
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product != null)
            {
                product.Stock -= order.Quantity;
            }
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Orden N° #{orderId} - Estado actualizado a: {GetStatusText(status)}";
        return RedirectToPage();
    }

    private async Task LoadFilterLists()
    {
        // Lista de proveedores con órdenes
        var suppliersWithOrders = await _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Where(o => o.Supplier != null)
            .Select(o => o.Supplier)
            .Distinct()
            .OrderBy(s => s.Name)
            .ToListAsync();

        SupplierFilterList = new SelectList(suppliersWithOrders, "Id", "Name");

        // Lista de estados para filtro
        StatusFilterList = new SelectList(new[]
        {
            new { Value = "Pending", Text = "Pendiente" },
            new { Value = "Approved", Text = "Aprobado" },
            new { Value = "Received", Text = "Recibido" },
            new { Value = "Cancelled", Text = "Cancelado" }
        }, "Value", "Text");
    }

    private string GetStatusText(string status)
    {
        return status switch
        {
            "Pending" => "Pendiente",
            "Approved" => "Aprobado",
            "Received" => "Recibido",
            "Cancelled" => "Cancelado",
            _ => status
        };
    }
}