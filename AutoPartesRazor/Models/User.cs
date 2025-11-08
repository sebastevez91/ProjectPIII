using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AutoPartesRazor.Models;

public class User : IdentityUser
{
    [Required]
    [Display(Name = "Usuario")]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Rol")]
    public string Role { get; set; } = "Client";

    [Display(Name = "Fecha de creación")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El domicilio no puede exceder los 100 caracteres.")]
    [Display(Name = "Domicilio")]
    public string? Address { get; set; } = string.Empty;

    [RegularExpression(@"^\d{8,10}$", ErrorMessage = "El teléfono debe tener 8 o 10 dígitos numéricos.")]
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }
}
