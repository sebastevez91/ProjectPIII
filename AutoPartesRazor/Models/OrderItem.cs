using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class OrderItem
{
    public int Id { get; set; }

    // Foreign Key - Order
    [Required]
    public int OrderId { get; set; }

    // Foreign Key - Product
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, 999)]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Precio unitario")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Subtotal")]
    public decimal Subtotal { get; set; }

    [Display(Name = "Estado")]
    public string Status { get; set; } = "Pendiente";

    // navegación
    public Product? Product { get; set; }
    public Order? Order { get; set; }
    public DateTime FechaActualizacion { get; set; } = DateTime.Now;

}