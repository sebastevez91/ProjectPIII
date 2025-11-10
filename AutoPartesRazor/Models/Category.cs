using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models
{
    public class Category
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Nombre de categoría")]
        public string name { get; set; }

        // Navegación
        public List<Product>? products { get; set; }
    }
}
