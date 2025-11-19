using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Supplier
{
    public int Id { get; set; }

<<<<<<< HEAD
    [Required(ErrorMessage = "El nombre del proveédor es obligatorio")]
    [StringLength(100)]
=======
    [Required]
    [StringLength(80)]
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    [Display(Name = "Proveedor")]
    public string Name { get; set; }

    [StringLength(100)]
<<<<<<< HEAD
    [EmailAddress]
=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(20)]
<<<<<<< HEAD
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
=======
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    // Navegación Many-to-Many
    public ICollection<ProductSupplier>? ProductSuppliers { get; set; }
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
}
