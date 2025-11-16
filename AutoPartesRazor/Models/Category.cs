using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Nombre de categoría")]
    public string Name { get; set; }

    // Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navegación
    public ICollection<Product>? Products { get; set; }
}