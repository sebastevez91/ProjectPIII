using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class ProductReview
{
    public int Id { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5 estrellas")]
    [Display(Name = "Calificación")]
    public int Rating { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres")]
    [Display(Name = "Comentario")]
    public string? Comment { get; set; }

    [Display(Name = "Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    [Required]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    // Sistema de utilidad
    [Display(Name = "Votos útiles")]
    public int HelpfulCount { get; set; } = 0;

    [Display(Name = "Votos no útiles")]
    public int NotHelpfulCount { get; set; } = 0;
}