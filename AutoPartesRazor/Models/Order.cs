using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;

public class Order
{
    public int Id { get; set; }

    // Foreign Key - User
    public string? UserId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Nombre de cliente")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Dirección de envio")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Método de pago")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Total")]
    public decimal Total { get; set; }

    [Display(Name = "Estado")]
    public string Status { get; set; } = "Pending";

    [Display(Name = "Fecha de operación")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "Ultima actualización")]
    public DateTime? UpdatedAt { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    [Display(Name = "Confirmado por Cliente")]
    public bool ClientConfirmed { get; set; } = false;

    public DateTime? DeliveryDate { get; set; }

    // ============================================
    // CAMPOS DE CUPÓN
    // ============================================

    [Display(Name = "Cupón aplicado")]
    public int? CouponId { get; set; }

    [StringLength(20)]
    [Display(Name = "Código del cupón")]
    public string? CouponCode { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Descuento aplicado")]
    public decimal DiscountAmount { get; set; } = 0m;

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Total original")]
    public decimal OriginalTotal { get; set; }

    // navegación
    public List<OrderItem> Items { get; set; } = new();
    public User? User { get; set; }
    public Coupon? Coupon { get; set; }

    public int? Calificacion { get; set; }
    public List<OrderEvent>? OrderEvents { get; set; } = new();
}