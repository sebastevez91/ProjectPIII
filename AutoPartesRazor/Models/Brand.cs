using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models
{
    public class Brand
    {
        public int id { get; set; }

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
