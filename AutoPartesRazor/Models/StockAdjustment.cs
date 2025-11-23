using AutoPartesRazor.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

/// Ajustes de stock con diferencias entre teórico y real
public class StockAdjustment
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Display(Name = "Stock Teórico")]
    public int TheoreticalStock { get; set; }

    [Required]
    [Display(Name = "Stock Real Contado")]
    public int ActualStock { get; set; }

    [Display(Name = "Diferencia")]
    public int Difference { get; set; }

    [Required]
    [StringLength(500)]
    [Display(Name = "Motivo del Ajuste")]
    public string Reason { get; set; }

    [Display(Name = "Tipo de Ajuste")]
    public AdjustmentType AdjustmentType { get; set; }

    [Display(Name = "Fecha de Ajuste")]
    public DateTime AdjustmentDate { get; set; } = DateTime.Now;

    [StringLength(100)]
    [Display(Name = "Responsable")]
    public string? ResponsibleUser { get; set; }

    // Relacionado a proveedor si es discrepancia en recepción
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? RelatedPurchaseOrder { get; set; }

    [Display(Name = "Requiere Reclamo")]
    public bool RequiresClaim { get; set; }

    // Navegación
    public Product? Product { get; set; }
    public StockMovement? StockMovement { get; set; }
    public SupplierClaim? SupplierClaim { get; set; }
}
