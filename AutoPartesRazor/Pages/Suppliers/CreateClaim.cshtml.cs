using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Suppliers;

public class CreateClaimModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public CreateClaimModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public SupplierClaimInput Input { get; set; } = new SupplierClaimInput();

    public SelectList Suppliers { get; set; }
    public SelectList PurchaseOrders { get; set; }
    public SelectList StockAdjustments { get; set; }

    public async Task<IActionResult> OnGetAsync(int? supplierId = null, int? orderId = null, int? adjustmentId = null)
    {
        await LoadSelectLists();

        // Pre-cargar si vienen parámetros
        if (supplierId.HasValue)
        {
            Input.SupplierId = supplierId.Value;
        }

        if (orderId.HasValue)
        {
            Input.PurchaseOrderId = orderId;
            var order = await _context.PurchaseOrders
                .Include(o => o.Product)
                .Include(o => o.Supplier)
                .FirstOrDefaultAsync(o => o.Id == orderId.Value);

            if (order != null)
            {
                Input.SupplierId = order.SupplierId;
                Input.Subject = $"Reclamo sobre Orden #{order.Id} - {order.Product?.Name}";
                Input.ExpectedQuantity = order.Quantity;
            }
        }

        if (adjustmentId.HasValue)
        {
            Input.StockAdjustmentId = adjustmentId;
            var adjustment = await _context.StockAdjustments
                .Include(a => a.Product)
                .Include(a => a.Supplier)
                .FirstOrDefaultAsync(a => a.Id == adjustmentId.Value);

            if (adjustment != null && adjustment.SupplierId.HasValue)
            {
                Input.SupplierId = adjustment.SupplierId.Value;
                Input.Subject = $"Ajuste de Stock - {adjustment.Product?.Name}";
                Input.ExpectedQuantity = adjustment.TheoreticalStock;
                Input.ReceivedQuantity = adjustment.ActualStock;
                Input.Description = adjustment.Reason;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return Page();
        }

        var claim = new SupplierClaim
        {
            SupplierId = Input.SupplierId,
            PurchaseOrderId = Input.PurchaseOrderId,
            StockAdjustmentId = Input.StockAdjustmentId,
            Subject = Input.Subject,
            Description = Input.Description,
            ExpectedQuantity = Input.ExpectedQuantity,
            ReceivedQuantity = Input.ReceivedQuantity,
            ClaimAmount = Input.ClaimAmount,
            Status = ClaimStatus.Pending,
            ClaimDate = DateTime.Now
        };

        _context.SupplierClaims.Add(claim);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Reclamo #{claim.Id} creado exitosamente.";
        return RedirectToPage("Claims");
    }

    public async Task<JsonResult> OnGetOrderDetailsAsync(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return new JsonResult(new { success = false });

        return new JsonResult(new
        {
            success = true,
            supplierId = order.SupplierId,
            supplierName = order.Supplier?.Name,
            productName = order.Product?.Name,
            quantity = order.Quantity,
            unitPrice = order.UnitPrice,
            total = order.Total,
            status = order.Status
        });
    }

    public async Task<JsonResult> OnGetSupplierOrdersAsync(int supplierId)
    {
        var orders = await _context.PurchaseOrders
            .Where(o => o.SupplierId == supplierId)
            .Include(o => o.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Take(20)
            .Select(o => new
            {
                id = o.Id,
                text = $"Orden #{o.Id} - {o.Product.Name} ({o.Quantity} unidades) - {o.Status}"
            })
            .ToListAsync();

        return new JsonResult(orders);
    }

    public async Task<JsonResult> OnGetSupplierAdjustmentsAsync(int supplierId)
    {
        var adjustments = await _context.StockAdjustments
            .Where(a => a.SupplierId == supplierId)
            .Include(a => a.Product)
            .OrderByDescending(a => a.AdjustmentDate)
            .Take(20)
            .Select(a => new
            {
                id = a.Id,
                text = $"Ajuste #{a.Id} - {a.Product.Name} (Diff: {a.Difference})"
            })
            .ToListAsync();

        return new JsonResult(adjustments);
    }

    private async Task LoadSelectLists()
    {
        Suppliers = new SelectList(
            await _context.Suppliers.Where(s => !s.IsDeleted).ToListAsync(),
            "Id", "Name");

        PurchaseOrders = new SelectList(
            await _context.PurchaseOrders
                .Include(o => o.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(50)
                .Select(o => new
                {
                    o.Id,
                    Display = $"Orden #{o.Id} - {o.Product.Name}"
                })
                .ToListAsync(),
            "Id", "Display");

        StockAdjustments = new SelectList(
            await _context.StockAdjustments
                .Include(a => a.Product)
                .OrderByDescending(a => a.AdjustmentDate)
                .Take(50)
                .Select(a => new
                {
                    a.Id,
                    Display = $"Ajuste #{a.Id} - {a.Product.Name}"
                })
                .ToListAsync(),
            "Id", "Display");
    }
}

public class SupplierClaimInput
{
    [Required(ErrorMessage = "Debe seleccionar un proveedor")]
    [Display(Name = "Proveedor")]
    public int SupplierId { get; set; }

    [Display(Name = "Orden de Compra")]
    public int? PurchaseOrderId { get; set; }

    [Display(Name = "Ajuste de Stock")]
    public int? StockAdjustmentId { get; set; }

    [Required(ErrorMessage = "El asunto es obligatorio")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El asunto debe tener entre 5 y 200 caracteres")]
    [Display(Name = "Asunto")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "La descripción debe tener entre 20 y 1000 caracteres")]
    [Display(Name = "Descripción del Reclamo")]
    public string Description { get; set; }

    [Range(0, 99999)]
    [Display(Name = "Cantidad Esperada")]
    public int? ExpectedQuantity { get; set; }

    [Range(0, 99999)]
    [Display(Name = "Cantidad Recibida")]
    public int? ReceivedQuantity { get; set; }

    [Range(0, 999999.99)]
    [Display(Name = "Monto Reclamado")]
    public decimal? ClaimAmount { get; set; }
}