using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

/// Orden de compra a proveedores
public class PurchaseOrder
{
    public int Id { get; set; }

    // Forenig key Producto
    [Required]
    public int ProductId { get; set; }

    // Forening Key Proveédor
    [Required]
    public int SupplierId { get; set; }

    [Display(Name = "Provéedor")]
    public Supplier? Supplier { get; set; }

    [Required]
    [Range(1, 9999)]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Precio unitario")]
    public decimal UnitPrice { get; set; }

    [Display(Name ="Estado")]
    public string? Status { get; set; } = "Pending";

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Total")]
    public decimal Total { get; set; }

    [Display(Name = "Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navegación
    public Product? Product { get; set; }

}