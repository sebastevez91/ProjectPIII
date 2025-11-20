using AutoPartesRazor.Data;
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Users;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;

    public IndexModel(AutoPartesRazorContext context, UserManager<User> userManager, IUserService userService)
    {
        _context = context;
        _userManager = userManager;
        _userService = userService;
    }

    public IList<User> Users { get; set; } = new List<User>();
    public int TotalUsers { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchString { get; set; }

    [BindProperty(SupportsGet = true)]
    public string RoleFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string StatusFilter { get; set; }

    public SelectList RoleList { get; set; }

    public async Task OnGetAsync()
    {
        // Obtener todos los usuarios
        var query = _context.Users.AsQueryable();

        // Filtro por búsqueda (nombre o email)
        if (!string.IsNullOrEmpty(SearchString))
        {
            query = query.Where(u =>
                u.FullName.Contains(SearchString) ||
                u.Email.Contains(SearchString) ||
                u.UserName.Contains(SearchString));
        }

        // Filtro por rol
        if (!string.IsNullOrEmpty(RoleFilter))
        {
            query = query.Where(u => u.Role == RoleFilter);
        }

        // Filtro por estado
        if (!string.IsNullOrEmpty(StatusFilter))
        {
            if (StatusFilter == "active")
            {
                query = query.Where(u => u.EmailConfirmed);
            }
            else if (StatusFilter == "inactive")
            {
                query = query.Where(u => !u.EmailConfirmed);
            }
        }

        // Obtener usuarios ordenados
        Users = await query
            .OrderByDescending(u => u.RegistrationDate)
            .ToListAsync();

        // Total de usuarios sin filtros
        TotalUsers = await _context.Users.CountAsync();

        // Lista de roles para el filtro
        RoleList = new SelectList(new[]
        {
            new { Value = "Admin", Text = "Administrador" },
            new { Value = "Employee", Text = "Empleado" },
            new { Value = "Client", Text = "Cliente" }
        }, "Value", "Text", RoleFilter);
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            TempData["ErrorMessage"] = "Usuario no encontrado.";
            return RedirectToPage();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Usuario no encontrado.";
            return RedirectToPage();
        }

        // Verificar que no sea el usuario actual
        var currentUser = await _userManager.GetUserAsync(User);
        if (user.Id == currentUser.Id)
        {
            TempData["ErrorMessage"] = "No puedes deshabilitar tu propia cuenta.";
            return RedirectToPage();
        }

        var result = await _userService.ToggleLockoutAsync(id);

        if (result.Succeeded)
        {
            bool isLocked = await _userService.IsLockedOutAsync(id);
            TempData["SuccessMessage"] = isLocked
                ? $"Usuario {user.FullName} deshabilitado exitosamente."
                : $"Usuario {user.FullName} habilitado exitosamente.";
        }
        else
        {
            TempData["ErrorMessage"] = "Error al cambiar el estado del usuario.";
        }

        return RedirectToPage();
    }
}