using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class OrderEvent
{
    [Key]
    public int Id { get; set; }

    // Clave foránea al pedido
    public int OrderId { get; set; }
    [ForeignKey("OrderId")]
    public Order? Order { get; set; }

    // Estado del pedido en el momento del evento
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    // Mensaje detallado para mostrar en la línea de tiempo
    [Required]
    [StringLength(255)]
    public string Description { get; set; } = string.Empty;

    // Fecha y hora exacta del evento
    public DateTime Timestamp { get; set; } = DateTime.Now;

    // Opcional: Para detalles como Nro. de Tracking, nombre del empleado, etc.
    public string? Reference { get; set; }
}