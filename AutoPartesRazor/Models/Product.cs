
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nombre de producto obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre de producto")]
    public string Name { get; set; }

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string Description { get; set; } = "No se ingresó descripción.";

    [Display(Name = "Stock")]
    public int Stock { get; set; } = 0;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Precio")]
    public decimal Price { get; set; }

    [Display(Name = "Imagen del producto")]
    public string? ImagePath { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Minimo Stock")]
    public int MinimumStock { get; set; } = 5;

    // Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Foreign Keys
    [Display(Name = "Categoria")]
    public int? CategoryId { get; set; }

    [Display(Name = "Marca")]
    public int? BrandId { get; set; }

    // Navigation Properties
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public ICollection<ProductSupplier>? ProductSuppliers { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
    //public ICollection<CartItem>? CartItems { get; set; }
    public ICollection<PurchaseOrder>? PurchaseOrders { get; set; }
}
