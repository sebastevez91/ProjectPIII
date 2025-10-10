using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Correo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
    public string Email { get; set; }

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MinLength(6, ErrorMessage = "El campo {0} debe tener al menos {1} carácteres.")]
    public string Password { get; set; }

    [Display(Name = "Recordarme en este navegador")]
    public bool RememberMe { get; set; }
}
