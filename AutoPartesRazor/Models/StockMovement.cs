using AutoPartesRazor.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

/// Movimientos de stock: entradas, salidas y ajustes
public class StockMovement
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Display(Name = "Tipo de Movimiento")]
    [Required]
    public StockMovementType MovementType { get; set; }

    [Required]
    [Range(1, 99999)]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    [Display(Name = "Stock Anterior")]
    public int PreviousStock { get; set; }

    [Display(Name = "Stock Nuevo")]
    public int NewStock { get; set; }

    [StringLength(500)]
    [Display(Name = "Motivo/Descripción")]
    public string? Reason { get; set; }

    [Display(Name = "Fecha")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Referencia opcional a orden de compra (si es entrada por compra)
    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    // Referencia opcional a ajuste de stock
    public int? StockAdjustmentId { get; set; }
    public StockAdjustment? StockAdjustment { get; set; }

    // Usuario que realizó el movimiento (opcional)
    [StringLength(100)]
    [Display(Name = "Usuario")]
    public string? UserName { get; set; }

    // Navegación
    public Product? Product { get; set; }
}