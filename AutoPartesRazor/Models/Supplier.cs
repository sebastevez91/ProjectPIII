using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del proveédor es obligatorio")]
    [StringLength(100)]
    [Display(Name = "Proveedor")]
    public string Name { get; set; }

    [StringLength(100)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(20)]
    [Display(Name = "Número de Teléfono")]
    public string? Phone { get; set; }

    [StringLength(200)]
    [Display(Name = "Domicilio")]
    public string? Address { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navegación Many-to-Many
    public ICollection<ProductSupplier>? ProductSuppliers { get; set; }
    public ICollection<PurchaseOrder>? PurchaseOrders { get; set; }
}
