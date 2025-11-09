using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "La contraseña actual es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
    [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
    public string ConfirmPassword { get; set; }
}
