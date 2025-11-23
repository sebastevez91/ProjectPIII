using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.PurchaseOrders;

public class ReceiveModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public ReceiveModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int OrderId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Debe ingresar la cantidad recibida")]
    [Range(0, 99999, ErrorMessage = "La cantidad debe ser entre 0 y 99999")]
    [Display(Name = "Cantidad Recibida")]
    public int ReceivedQuantity { get; set; }

    [BindProperty]
    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observations { get; set; }

    [BindProperty]
    [Display(Name = "Crear Reclamo")]
    public bool CreateClaim { get; set; }

    public PurchaseOrder? PurchaseOrder { get; set; }
    public bool HasDiscrepancy { get; set; }
    public int QuantityDifference { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        OrderId = id;
        PurchaseOrder = await _context.PurchaseOrders
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (PurchaseOrder == null)
        {
            TempData["ErrorMessage"] = "Orden no encontrada.";
            return RedirectToPage("Index");
        }

        if (PurchaseOrder.Status == "Received")
        {
            TempData["ErrorMessage"] = "Esta orden ya fue recibida.";
            return RedirectToPage("Index");
        }

        // Pre-cargar la cantidad esperada
        ReceivedQuantity = PurchaseOrder.Quantity;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            PurchaseOrder = await _context.PurchaseOrders
                .Include(o => o.Product)
                .Include(o => o.Supplier)
                .FirstOrDefaultAsync(o => o.Id == OrderId);
            return Page();
        }

        PurchaseOrder = await _context.PurchaseOrders
            .Include(o => o.Product)
            .Include(o => o.Supplier)
            .FirstOrDefaultAsync(o => o.Id == OrderId);

        if (PurchaseOrder == null)
        {
            TempData["ErrorMessage"] = "Orden no encontrada.";
            return RedirectToPage("Index");
        }

        var product = await _context.Products.FindAsync(PurchaseOrder.ProductId);
        if (product == null)
        {
            TempData["ErrorMessage"] = "Producto no encontrado.";
            return RedirectToPage("Index");
        }

        int difference = ReceivedQuantity - PurchaseOrder.Quantity;
        bool hasDiscrepancy = difference != 0;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Actualizar stock del producto
            int previousStock = product.Stock;
            product.Stock += ReceivedQuantity;

            // 2. Crear movimiento de stock
            var stockMovement = new StockMovement
            {
                ProductId = product.Id,
                MovementType = StockMovementType.PurchaseEntry,
                Quantity = ReceivedQuantity,
                PreviousStock = previousStock,
                NewStock = product.Stock,
                Reason = $"Recepción de Orden #{PurchaseOrder.Id} - {PurchaseOrder.Supplier?.Name}",
                PurchaseOrderId = PurchaseOrder.Id,
                UserName = User.Identity?.Name ?? "Sistema"
            };
            _context.StockMovements.Add(stockMovement);

            // 3. Si hay discrepancia, crear ajuste de stock
            if (hasDiscrepancy)
            {
                var adjustment = new StockAdjustment
                {
                    ProductId = product.Id,
                    TheoreticalStock = previousStock + PurchaseOrder.Quantity,
                    ActualStock = product.Stock,
                    Difference = difference,
                    Reason = Observations ?? $"Diferencia en recepción de orden #{PurchaseOrder.Id}",
                    AdjustmentType = AdjustmentType.ReceptionDiscrepancy,
                    SupplierId = PurchaseOrder.SupplierId,
                    PurchaseOrderId = PurchaseOrder.Id,
                    RequiresClaim = CreateClaim,
                    ResponsibleUser = User.Identity?.Name ?? "Sistema"
                };
                _context.StockAdjustments.Add(adjustment);
                await _context.SaveChangesAsync(); // Guardar para obtener el Id

                // Actualizar la referencia en el movimiento
                stockMovement.StockAdjustmentId = adjustment.Id;

                // 4. Si se debe crear reclamo
                if (CreateClaim && Math.Abs(difference) > 0)
                {
                    var claim = new SupplierClaim
                    {
                        SupplierId = PurchaseOrder.SupplierId,
                        PurchaseOrderId = PurchaseOrder.Id,
                        StockAdjustmentId = adjustment.Id,
                        Subject = difference > 0
                            ? $"Recepción con excedente - Orden #{PurchaseOrder.Id}"
                            : $"Recepción incompleta - Orden #{PurchaseOrder.Id}",
                        Description = $"Orden #{PurchaseOrder.Id} - Producto: {product.Name}\n" +
                                    $"Cantidad esperada: {PurchaseOrder.Quantity}\n" +
                                    $"Cantidad recibida: {ReceivedQuantity}\n" +
                                    $"Diferencia: {difference:+#;-#;0}\n" +
                                    $"Observaciones: {Observations}",
                        ExpectedQuantity = PurchaseOrder.Quantity,
                        ReceivedQuantity = ReceivedQuantity,
                        ClaimAmount = Math.Abs(difference) * PurchaseOrder.UnitPrice,
                        Status = ClaimStatus.Pending
                    };
                    _context.SupplierClaims.Add(claim);
                }
            }

            // 5. Actualizar estado de la orden
            PurchaseOrder.Status = "Received";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (hasDiscrepancy)
            {
                TempData["WarningMessage"] = $"Orden recibida con diferencia de {difference:+#;-#;0} unidades. " +
                    (CreateClaim ? "Se ha creado un reclamo al proveedor." : "");
            }
            else
            {
                TempData["SuccessMessage"] = "Orden recibida correctamente sin discrepancias.";
            }

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = $"Error al procesar la recepción: {ex.Message}";
            return Page();
        }
    }
}