using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
=======
using Microsoft.AspNetCore.Mvc.RazorPages;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.PurchaseOrders;

public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public IndexModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

<<<<<<< HEAD
    public IList<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public SelectList StatusList { get; set; }

    public async Task OnGetAsync()
    {
        PurchaseOrders = await _context.PurchaseOrders
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        // Lista de estados disponibles
        StatusList = new SelectList(new[]
        {
            new { Value = "Pending", Text = "Pendiente" },
            new { Value = "Approved", Text = "Aprobado" },
            new { Value = "Received", Text = "Recibido" },
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

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Estado actualizado a: {GetStatusText(status)}";
        return RedirectToPage();
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
=======
    public IList<PurchaseOrder> PurchaseOrder { get; set; }

    public async Task OnGetAsync()
    {
        PurchaseOrder = await _context.PurchaseOrder
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .ToListAsync();
    }
}
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
