
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class Product
{
    public int id {  get; set; }

    [Required]
    [StringLength(50)]
    public string name { get; set; }

    [Required]
    [StringLength(300)]
    public string description { get; set; }

    [Required]
    public int stock { get; set; } = 0;

    [Required]
    [Column("price", TypeName = "decimal(18, 2)")]
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
