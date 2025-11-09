using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels;

public class UpdateViewModel
{
    [Required]
    [Display(Name = "Nombre de usuario")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener más de 80 caracteres.")]
    [Display(Name = "Nombre Completo")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El domicilio no puede exceder los 100 caracteres.")]
    [Display(Name = "Domicilio")]
    public string? Address { get; set; } = string.Empty;

    [RegularExpression(@"^\d{8,10}$", ErrorMessage = "El teléfono debe tener 8 o 10 dígitos numéricos.")]
    [Display(Name = "Teléfono")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Foto de perfil")]
    public string? ProfilePicturePath { get; set; }

    // Última actualización
    public DateTime? LastUpdated { get; set; } = DateTime.Now;
}
