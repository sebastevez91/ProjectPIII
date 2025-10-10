using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models
{
    public class Brand
    {
        public int id {  get; set; }

        [Required]
        [Display(Name = "Marca")]
        public string name { get; set; }

        // Navegación
        public List<Product>? products { get; set; }
    }
}
