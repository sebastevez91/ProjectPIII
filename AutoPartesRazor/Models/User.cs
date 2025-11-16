using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AutoPartesRazor.Models;

public class User : IdentityUser
{
    [Required]
    [Display(Name = "Rol")]
    public string Role { get; set; } = "Client";

    [Display(Name = "Fecha de registro")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener más de 100 caracteres.")]
    [Display(Name = "Nombre Completo")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El domicilio no puede exceder los 100 caracteres.")]
    [Display(Name = "Domicilio")]
    public string? Address { get; set; } = string.Empty;

    [RegularExpression(@"^\d{8,10}$", ErrorMessage = "El teléfono debe tener 8 o 10 dígitos numéricos.")]
    [Display(Name = "Número de Teléfono")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Foto de perfil")]
    public string? ProfilePicturePath { get; set; }

    // Última actualización
    [Display(Name = "Ultima actualización")]
    public DateTime? LastUpdated { get; set; }

    // Notificación relacionada
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public Cart? Cart { get; set; }
}