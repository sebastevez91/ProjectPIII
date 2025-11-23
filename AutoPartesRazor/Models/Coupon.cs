using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

/// <summary>
/// Cupón de descuento generado para usuarios con reseñas negativas
/// </summary>
public class Coupon
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Código del Cupón")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Range(1, 100)]
    [Display(Name = "Porcentaje de Descuento")]
    public int DiscountPercentage { get; set; }

    [Required]
    [Display(Name = "Fecha de Creación")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    [Display(Name = "Fecha de Expiración")]
    public DateTime ExpiresAt { get; set; }

    [Display(Name = "Usado")]
    public bool IsUsed { get; set; } = false;

    [Display(Name = "Fecha de Uso")]
    public DateTime? UsedAt { get; set; }

    [Display(Name = "Activo")]
    public bool IsActive { get; set; } = true;

    // Foreign Keys
    [Required]
    [Display(Name = "Usuario")]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Producto Específico")]
    public int? ProductId { get; set; }

    [Display(Name = "Reseña que originó el cupón")]
    public int? ReviewId { get; set; }

    [Display(Name = "Pedido donde se usó")]
    public int? OrderId { get; set; }

    [StringLength(500)]
    [Display(Name = "Motivo del Cupón")]
    public string Reason { get; set; } = string.Empty;

    // Navigation Properties
    public User? User { get; set; }
    public Product? Product { get; set; }
    public ProductReview? Review { get; set; }
    public Order? Order { get; set; }

    /// <summary>
    /// Verifica si el cupón está disponible para usar
    /// </summary>
    [NotMapped]
    public bool IsAvailable => IsActive && !IsUsed && ExpiresAt > DateTime.Now;

    /// <summary>
    /// Días restantes hasta la expiración
    /// </summary>
    [NotMapped]
    public int DaysUntilExpiration => IsAvailable ? (int)(ExpiresAt - DateTime.Now).TotalDays : 0;
}