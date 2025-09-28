using AutoPartesRazor.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace AutoPartesRazor.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Role Role { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}
