using AutoPartesRazor.Constants;
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Administration.Users;

[Authorize(Roles = "Admin")]
public class ManagePermissionsModel : PageModel
{
    private readonly IUserService _userService;

    public ManagePermissionsModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string RoleId { get; set; }

    public string RoleName { get; set; }

    [BindProperty]
    public List<PermissionGroupViewModel> PermissionGroups { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToPage("./Index");
        }

        var role = await _userService.GetRoleByIdAsync(id);
        if (role == null)
        {
            TempData["ErrorMessage"] = "Rol no encontrado.";
            return RedirectToPage("./Index");
        }

        RoleId = role.Id;
        RoleName = role.Name;

        // Obtener los claims actuales del rol
        var roleClaims = await _userService.GetRoleClaimsAsync(role.Id);
        var roleClaimValues = roleClaims
            .Where(c => c.Type == Permissions.ClaimType)
            .Select(c => c.Value)
            .ToList();

        // Obtener todos los permisos disponibles
        var allPermissions = Permissions.GetAllPermissions();

        // Crear los grupos de permisos con el estado seleccionado
        PermissionGroups = allPermissions.Select(group => new PermissionGroupViewModel
        {
            GroupName = group.GroupName,
            Permissions = group.Permissions.Select(p => new PermissionViewModel
            {
                Name = p.Name,
                Description = p.Description,
                IsSelected = roleClaimValues.Contains(p.Name)
            }).ToList()
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(RoleId))
        {
            TempData["ErrorMessage"] = "ID de rol inválido.";
            return RedirectToPage("./Index");
        }

        var role = await _userService.GetRoleByIdAsync(RoleId);
        if (role == null)
        {
            TempData["ErrorMessage"] = "Rol no encontrado.";
            return RedirectToPage("./Index");
        }

        // Obtener todos los permisos seleccionados
        var selectedPermissions = PermissionGroups
            .SelectMany(g => g.Permissions)
            .Where(p => p.IsSelected)
            .Select(p => p.Name)
            .ToList();

        // Obtener los claims actuales del rol
        var currentClaims = await _userService.GetRoleClaimsAsync(RoleId);
        var currentPermissions = currentClaims
            .Where(c => c.Type == Permissions.ClaimType)
            .Select(c => c.Value)
            .ToList();

        // Permisos a agregar (están seleccionados pero no existen actualmente)
        var permissionsToAdd = selectedPermissions
            .Except(currentPermissions)
            .ToList();

        // Permisos a remover (existen actualmente pero no están seleccionados)
        var permissionsToRemove = currentPermissions
            .Except(selectedPermissions)
            .ToList();

        // Agregar nuevos permisos
        foreach (var permission in permissionsToAdd)
        {
            await _userService.AddClaimToRoleAsync(RoleId, Permissions.ClaimType, permission);
        }

        // Remover permisos deseleccionados
        foreach (var permission in permissionsToRemove)
        {
            await _userService.RemoveClaimFromRoleAsync(RoleId, Permissions.ClaimType, permission);
        }

        TempData["SuccessMessage"] = $"Permisos del rol '{role.Name}' actualizados exitosamente. " +
            $"Agregados: {permissionsToAdd.Count}, Removidos: {permissionsToRemove.Count}";

        return RedirectToPage(new { id = RoleId });
    }
}
