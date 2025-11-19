<<<<<<< HEAD
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

=======
namespace AutoPartesRazor.Models
{
    public class OrderItem
    {
        public int id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // navegación
        public Product? Product { get; set; }
        public Order? Order { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

    }
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
}