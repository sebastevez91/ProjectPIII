using System.ComponentModel.DataAnnotations;

<<<<<<< HEAD
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
=======
namespace AutoPartesRazor.Models
{
    public class Brand
    {
        public int id {  get; set; }

        [Required]
        [Display(Name = "Marca")]
        public string name { get; set; }

        // Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Navegación
        public List<Product>? products { get; set; }
    }
}
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
