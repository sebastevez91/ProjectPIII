using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Stock;

public class AdjustmentsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public AdjustmentsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    [BindProperty]
    public StockAdjustmentInput Input { get; set; } = new StockAdjustmentInput();

    public SelectList Products { get; set; }
    public SelectList AdjustmentTypes { get; set; }
    public SelectList Suppliers { get; set; }

    public async Task OnGetAsync()
    {
        StockAdjustments = await _context.StockAdjustments
            .Include(a => a.Product)
            .Include(a => a.Supplier)
            .Include(a => a.RelatedPurchaseOrder)
            .OrderByDescending(a => a.AdjustmentDate)
            .Take(50)
            .ToListAsync();

        await LoadSelectLists();
    }

    public async Task<IActionResult> OnPostCreateAdjustmentAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var product = await _context.Products.FindAsync(Input.ProductId);
        if (product == null)
        {
            TempData["ErrorMessage"] = "Producto no encontrado.";
            return RedirectToPage();
        }

        int theoreticalStock = product.Stock;
        int difference = Input.ActualStock - theoreticalStock;

        if (difference == 0)
        {
            TempData["InfoMessage"] = "No hay diferencia entre el stock teórico y el real.";
            return RedirectToPage();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Crear el ajuste
            var adjustment = new StockAdjustment
            {
                ProductId = Input.ProductId,
                TheoreticalStock = theoreticalStock,
                ActualStock = Input.ActualStock,
                Difference = difference,
                Reason = Input.Reason,
                AdjustmentType = Input.AdjustmentType,
                SupplierId = Input.SupplierId,
                RequiresClaim = Input.CreateClaim,
                ResponsibleUser = User.Identity?.Name ?? "Sistema"
            };
            _context.StockAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            // Crear movimiento de stock
            var movementType = difference > 0
                ? StockMovementType.AdjustmentIncrease
                : StockMovementType.AdjustmentDecrease;

            var movement = new StockMovement
            {
                ProductId = Input.ProductId,
                MovementType = movementType,
                Quantity = Math.Abs(difference),
                PreviousStock = theoreticalStock,
                NewStock = Input.ActualStock,
                Reason = Input.Reason,
                StockAdjustmentId = adjustment.Id,
                UserName = User.Identity?.Name ?? "Sistema"
            };
            _context.StockMovements.Add(movement);

            // Actualizar el stock del producto
            product.Stock = Input.ActualStock;
            product.ActualStock = Input.ActualStock;
            product.LastStockCheck = DateTime.Now;

            // Crear reclamo si es necesario
            if (Input.CreateClaim && Input.SupplierId.HasValue)
            {
                var supplier = await _context.Suppliers.FindAsync(Input.SupplierId.Value);
                var claim = new SupplierClaim
                {
                    SupplierId = Input.SupplierId.Value,
                    StockAdjustmentId = adjustment.Id,
                    Subject = $"Ajuste de stock - {product.Name}",
                    Description = $"Producto: {product.Name}\n" +
                                $"Stock Teórico: {theoreticalStock}\n" +
                                $"Stock Real: {Input.ActualStock}\n" +
                                $"Diferencia: {difference:+#;-#;0}\n" +
                                $"Motivo: {Input.Reason}",
                    ExpectedQuantity = theoreticalStock,
                    ReceivedQuantity = Input.ActualStock,
                    Status = ClaimStatus.Pending
                };
                _context.SupplierClaims.Add(claim);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = $"Ajuste realizado correctamente. Diferencia: {difference:+#;-#;0} unidades";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = $"Error al realizar el ajuste: {ex.Message}";
            return RedirectToPage();
        }
    }

    public async Task<JsonResult> OnGetProductStockAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return new JsonResult(new { success = false });

        return new JsonResult(new
        {
            success = true,
            stock = product.Stock,
            actualStock = product.ActualStock ?? product.Stock,
            lastCheck = product.LastStockCheck?.ToString("dd/MM/yyyy HH:mm")
        });
    }

    private async Task LoadSelectLists()
    {
        Products = new SelectList(
            await _context.Products.Where(p => !p.IsDeleted).ToListAsync(),
            "Id", "Name");

        AdjustmentTypes = new SelectList(
            Enum.GetValues(typeof(AdjustmentType))
                .Cast<AdjustmentType>()
                .Select(e => new {
                    Value = (int)e,
                    Text = GetAdjustmentType(e)
                }),
            "Value", "Text");

        Suppliers = new SelectList(
            await _context.Suppliers.Where(s => !s.IsDeleted).ToListAsync(),
            "Id", "Name");
    }

    public string GetAdjustmentType(AutoPartesRazor.Models.Enum.AdjustmentType status)
    {
        return status switch
        {
            AutoPartesRazor.Models.Enum.AdjustmentType.DamagedProduct => "Producto dañado",
            AutoPartesRazor.Models.Enum.AdjustmentType.LostProduct => "Producto perdido",
            AutoPartesRazor.Models.Enum.AdjustmentType.Other => "Otro",
            AutoPartesRazor.Models.Enum.AdjustmentType.PhysicalInventory => "Inventario",
            AutoPartesRazor.Models.Enum.AdjustmentType.ReceptionDiscrepancy => "Discrepancia",
            AutoPartesRazor.Models.Enum.AdjustmentType.SystemError => "Error de sistema",
            _ => status.ToString()
        };
    }
}


public class StockAdjustmentInput
{
    [Required(ErrorMessage = "Debe seleccionar un producto")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Debe ingresar el stock real contado")]
    [Range(0, 99999)]
    [Display(Name = "Stock Real Contado")]
    public int ActualStock { get; set; }

    [Required(ErrorMessage = "Debe especificar el motivo del ajuste")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "El motivo debe tener entre 10 y 500 caracteres")]
    [Display(Name = "Motivo del Ajuste")]
    public string Reason { get; set; }

    [Required]
    [Display(Name = "Tipo de Ajuste")]
    public AdjustmentType AdjustmentType { get; set; }

    [Display(Name = "Proveedor Relacionado")]
    public int? SupplierId { get; set; }

    [Display(Name = "Crear Reclamo al Proveedor")]
    public bool CreateClaim { get; set; }
}