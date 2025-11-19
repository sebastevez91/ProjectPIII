using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;
=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

namespace AutoPartesRazor.Models;

/// Orden de compra a proveedores
public class PurchaseOrder
{
    public int Id { get; set; }

<<<<<<< HEAD
    // Forenig key Producto
    [Required]
    public int ProductId { get; set; }

    // Forening Key Proveédor
    [Required]
    public int SupplierId { get; set; }

    [Display(Name = "Provéedor")]
=======
    [Required]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    public int SupplierId { get; set; }
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    public Supplier? Supplier { get; set; }

    [Required]
    [Range(1, 9999)]
<<<<<<< HEAD
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

=======
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
}