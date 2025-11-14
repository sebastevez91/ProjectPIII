using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels;

public class RegisterViewModels
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [Display(Name = "Usuario")]
    public string Username { get; set; }

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatorio")]
    [StringLength(40, MinimumLength = 6, ErrorMessage = "La contraseña debe contener 6 caracteres como minimo")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage = "Las contraseñas no son iguales")]
    [Display(Name = "Contraseña")]
    public string Password { get; set; }

    [Required(ErrorMessage = "La confirmación de contraseña es obligatorio")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmación de contraseña")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener 80 caracteres.")]
    [Display(Name = "Nombre Completo")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El domicilio no puede exceder los 100 caracteres.")]
    [Display(Name = "Domicilio")]
    public string Address { get; set; } = string.Empty;

    [RegularExpression(@"^\d{8,10}$", ErrorMessage = "El teléfono debe tener 8 o 10 dígitos numéricos.")]
    [Display(Name = "Teléfono")]
    public string? PhoneNumber { get; set; }
}
