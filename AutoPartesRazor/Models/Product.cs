
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class Product
{
    public int id {  get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Producto")]
    public string name { get; set; }

    [Required(ErrorMessage = "La decripción es obligatoria.")]
    [StringLength(300)]
    [Display(Name = "Descripción")]
    public string description { get; set; }

    [Column("stock", TypeName = "int(0,10000)")]
    [Display(Name = "Stock")]
    public int stock { get; set; } = 0;

    [Required]
    [Column("price", TypeName = "decimal(18, 2)")]
    [Display(Name = "Precio")]
    public decimal price { get; set; }

    public string? ImagePath { get; set; }

    // Clave Foreign Key
    public int? idCategory { get; set; }

    // Navegación 
    public Category? Category { get; set; }

    // Clave Foreign Key
    public int? idBrand { get; set; }

    // Navegación 
    public Brand? Brand { get; set; }
}
