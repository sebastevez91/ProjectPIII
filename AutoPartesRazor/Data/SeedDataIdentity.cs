using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.Services;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace AutoPartesRazor.Data;

public class SeedDataIdentity
{
    private readonly AutoPartesRazorContext _context;
    private readonly IUserService _userService;
    public SeedDataIdentity(AutoPartesRazorContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }
    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckRolesAsync();
        // Cambia la contraseña a una que cumpla la política o ajusta la política en Program.cs
        await CheckUserAsync("SuperAdministrador", "Admin", "superadmin@gmail.com","SuperAdmin123!");
    }
    private async Task CheckRolesAsync()
    {
        await _userService.CheckRoleAsync("Admin");
        await _userService.CheckRoleAsync("User");
    }
    private async Task<User> CheckUserAsync(string nombre, string apellido, string correo, string password)
    {
        User user = await _userService.GetUserAsync(correo);
        if (user == null)
        {
            user = new User
            {
                FullName = nombre,
                UserName = correo,
                Email = correo,
                Role = "Admin",
            };
            IdentityResult createResult = await _userService.AddUserAsync(user, password);
            if (!createResult.Succeeded)
            {
                // Construir mensaje de error claro con causas de fallo de Identity
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Code + ": " + e.Description));
                throw new InvalidOperationException($"No se pudo crear el usuario '{correo}': {errors}");
            }

            // Sólo añadir al rol si la creación tuvo éxito
            await _userService.AddUserToRoleAsync(user,"Admin");
        }
        return user;
    }
}
