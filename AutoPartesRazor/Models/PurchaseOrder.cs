using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

/// Orden de compra a proveedores
public class PurchaseOrder
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    [Required]
    [Range(1, 9999)]
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}