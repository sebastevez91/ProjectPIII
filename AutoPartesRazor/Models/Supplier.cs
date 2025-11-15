using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    [Display(Name = "Proveedor")]
    public string Name { get; set; }

    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    // Navegación Many-to-Many
    public ICollection<ProductSupplier>? ProductSuppliers { get; set; }
}
