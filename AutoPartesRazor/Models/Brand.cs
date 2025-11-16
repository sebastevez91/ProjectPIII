using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Brand
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Marca")]
    public string Name { get; set; }

    // Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navegación
    public List<Product>? Products { get; set; }
}