using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels;

public class RoleViewModel
{
    public string Id { get; set; }

    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
    [Display(Name = "Nombre del Rol")]
    public string Name { get; set; }

    [Display(Name = "Número de usuarios")]
    public int UsersCount { get; set; }

    [Display(Name = "Número de permisos")]
    public int PermissionsCount { get; set; }
}