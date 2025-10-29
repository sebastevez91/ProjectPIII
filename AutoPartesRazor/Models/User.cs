using AutoPartesRazor.Models.Enums;
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
    public Role Role { get; set; } = Role.Admin;

    [Display(Name = "Fecha de creación")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}
