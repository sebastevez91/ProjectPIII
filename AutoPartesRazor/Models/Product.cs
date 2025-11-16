using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class Product
{
    // ============================================
    // PROPIEDADES ORIGINALES (NO TOCAR)
    // ============================================

    public int id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Producto")]
    public string name { get; set; }

    [Required(ErrorMessage = "La decripción es obligatoria.")]
    [StringLength(300)]
    [Display(Name = "Descripción")]
    public string description { get; set; }

    [Display(Name = "Stock")]
    public int stock { get; set; } = 0;

    [Required]
    [Column("price", TypeName = "decimal(18, 2)")]
    [Display(Name = "Precio")]
    public decimal price { get; set; }

    public string? ImagePath { get; set; }

    // Nivel mínimo de stock (para alertas)
    [Range(0, int.MaxValue)]
    public int MinimumStock { get; set; } = 5;

    // Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Clave Foreign Key
    public int? idCategory { get; set; }

    // Navegación 
    public Category? Category { get; set; }

    // Clave Foreign Key
    public int? idBrand { get; set; }

    // Navegación 
    public Brand? Brand { get; set; }

    // Navegación 
    public ICollection<ProductSupplier>? ProductSuppliers { get; set; }

    // ============================================
    // ⬇️⬇️⬇️ AGREGAR DESDE AQUÍ ⬇️⬇️⬇️
    // NUEVAS PROPIEDADES PARA RESEÑAS
    // ============================================

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
}