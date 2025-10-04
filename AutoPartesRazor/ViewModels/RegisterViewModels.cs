using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels;

public class RegisterViewModels
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [Display(Name = "Nombre")]
    public string Name { get; set; }

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatorio")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "La contraseña debe contener 8 caracteres como minimo")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage = "Las contraseñas no son iguales")]
    [Display(Name = "Contraseña")]
    public string Password { get; set; }

    [Required(ErrorMessage = "La confirmación de contraseña es obligatorio")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmación de contraseña")]
    public string ConfirmPassword { get; set; }
}
