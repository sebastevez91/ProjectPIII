using AutoPartesRazor.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

/// Orden de compra a proveedores
public class PurchaseOrder
{
    public int Id { get; set; }

    // Forenig key Producto
    [Required]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    // Forening Key Proveédor
    [Required]
    public int SupplierId { get; set; }

    [Display(Name = "Provéedor")]
    public Supplier? Supplier { get; set; }

    [Required]
    [Range(1, 9999)]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    [Display(Name = "Estado")]
    public StatusOrder? Status { get; set; } = StatusOrder.Pendiente;

    [Display(Name = "Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}