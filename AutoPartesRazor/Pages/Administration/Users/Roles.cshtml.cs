using AutoPartesRazor.Constants;
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Users;

[Authorize(Roles = "Admin")]
public class RolesModel : PageModel
{
    private readonly IUserService _userService;
    private readonly UserManager<Models.User> _userManager;

    public RolesModel(IUserService userService, UserManager<Models.User> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    public List<RoleViewModel> Roles { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
    public string NewRoleName { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadRolesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRolesAsync();
            return Page();
        }

        // Verificar si el rol ya existe
        var existingRole = await _userService.GetRoleByNameAsync(NewRoleName);
        if (existingRole != null)
        {
            TempData["ErrorMessage"] = $"El rol '{NewRoleName}' ya existe.";
            return RedirectToPage();
        }

        var result = await _userService.CreateRoleAsync(NewRoleName);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Rol '{NewRoleName}' creado exitosamente.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Error al crear el rol: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(string roleId, string newRoleName)
    {
        if (string.IsNullOrWhiteSpace(newRoleName))
        {
            TempData["ErrorMessage"] = "El nombre del rol no puede estar vacío.";
            return RedirectToPage();
        }

        var result = await _userService.UpdateRoleAsync(roleId, newRoleName);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Rol actualizado a '{newRoleName}' exitosamente.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Error al actualizar el rol: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var role = await _userService.GetRoleByIdAsync(id);
        if (role == null)
        {
            TempData["ErrorMessage"] = "Rol no encontrado.";
            return RedirectToPage();
        }

        // Verificar si hay usuarios con este rol
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
        {
            TempData["ErrorMessage"] = $"No se puede eliminar el rol '{role.Name}' porque tiene {usersInRole.Count} usuario(s) asignado(s).";
            return RedirectToPage();
        }

        var result = await _userService.DeleteRoleAsync(id);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Rol '{role.Name}' eliminado exitosamente.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Error al eliminar el rol: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToPage();
    }

    private async Task LoadRolesAsync()
    {
        var roles = await _userService.GetAllRolesAsync();

        Roles = new List<RoleViewModel>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            var claims = await _userService.GetRoleClaimsAsync(role.Id);

            Roles.Add(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                UsersCount = usersInRole.Count,
                PermissionsCount = claims.Count
            });
        }
    }
}