using AutoPartesRazor.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

/// Reclamos a proveedores por discrepancias
public class SupplierClaim
{
    public int Id { get; set; }

    [Required]
    public int SupplierId { get; set; }

    public int? PurchaseOrderId { get; set; }

    public int? StockAdjustmentId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Asunto")]
    public string Subject { get; set; }

    [Required]
    [StringLength(1000)]
    [Display(Name = "Descripción del Reclamo")]
    public string Description { get; set; }

    [Display(Name = "Cantidad Esperada")]
    public int? ExpectedQuantity { get; set; }

    [Display(Name = "Cantidad Recibida")]
    public int? ReceivedQuantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Monto Reclamado")]
    public decimal? ClaimAmount { get; set; }

    [Display(Name = "Estado")]
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

    [Display(Name = "Fecha de Reclamo")]
    public DateTime ClaimDate { get; set; } = DateTime.Now;

    [Display(Name = "Fecha de Resolución")]
    public DateTime? ResolutionDate { get; set; }

    [StringLength(1000)]
    [Display(Name = "Resolución")]
    public string? Resolution { get; set; }

    // Navegación
    public Supplier? Supplier { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public StockAdjustment? StockAdjustment { get; set; }
}
