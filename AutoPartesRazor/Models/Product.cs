
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class Product
{
    public int id {  get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Nombre del producto")]
    public string name { get; set; }

    [Required]
    [StringLength(300)]
    [Display(Name = "Descripción")]
    public string description { get; set; }

    [Required]
    [Display(Name = "Cantidad")]
    public int stock { get; set; } = 0;

    [Required]
    [Display(Name = "Precio")]
    [Column("price", TypeName = "decimal(18, 2)")]
    public decimal price { get; set; }

    // Clave Foreign Key
    public int? idCategory { get; set; }

    // Navegación 
    public Category? Category { get; set; }

    // Clave Foreign Key
    public int? idBrand { get; set; }

    // Navegación 
    public Brand? Brand { get; set; }
}
