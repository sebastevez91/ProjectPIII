
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
    [Column("Price", TypeName = "decimal(18, 2)")]
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


    /// <summary>
    /// Colección de todas las reseñas asociadas a este producto.
    /// Se carga usando: .Include(p => p.Reviews)
    /// </summary>
    public ICollection<ProductReview>? Reviews { get; set; }

    /// <summary>
    /// Promedio de calificación del producto (de 0.0 a 5.0).
    /// Se calcula automáticamente desde las reseñas.
    /// Ejemplo: Si tiene reseñas de 5, 4, 3 → retorna 4.0
    /// Si no tiene reseñas → retorna 0.0
    /// </summary>
    [NotMapped]
    public double AverageRating => Reviews?.Any() == true
        ? Math.Round(Reviews.Average(r => r.Rating), 1)
        : 0;

    /// <summary>
    /// Número total de reseñas del producto.
    /// Se calcula automáticamente contando las reseñas.
    /// Si no tiene reseñas → retorna 0
    /// </summary>
    [NotMapped]
    public int TotalReviews => Reviews?.Count ?? 0;

    /// <summary>
    /// Porcentaje de reseñas positivas (4 o 5 estrellas).
    /// Ejemplo: Si tiene 10 reseñas y 8 son de 4-5 estrellas → retorna 80
    /// Si no tiene reseñas → retorna 0
    /// </summary>
    [NotMapped]
    public int PositiveReviewsPercentage => TotalReviews > 0
        ? (int)((Reviews!.Count(r => r.Rating >= 4) / (double)TotalReviews) * 100)
        : 0;

    // Navigation Properties
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public ICollection<ProductSupplier>? ProductSuppliers { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
    //public ICollection<CartItem>? CartItems { get; set; }
    public ICollection<PurchaseOrder>? PurchaseOrders { get; set; }
}
