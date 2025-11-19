using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Pages.Users;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _environment;

    public EditModel(
        AutoPartesRazorContext context,
        UserManager<User> userManager,
        IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public SelectList RoleList { get; set; }

    public class InputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{8,10}$", ErrorMessage = "El teléfono debe tener entre 8 y 10 dígitos")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "La dirección no puede exceder los 100 caracteres")]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Display(Name = "Rol")]
        public string Role { get; set; }

        [Display(Name = "Email Confirmado")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Teléfono Confirmado")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "Foto de Perfil")]
        public IFormFile ProfilePicture { get; set; }

        public string CurrentProfilePicturePath { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Role = user.Role,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            CurrentProfilePicturePath = user.ProfilePicturePath
        };

        LoadRoleList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            LoadRoleList();
            return Page();
        }

        var user = await _userManager.FindByIdAsync(Input.Id);
        if (user == null)
        {
            return NotFound();
        }

        // Actualizar información básica
        user.FullName = Input.FullName;
        user.Email = Input.Email;
        user.UserName = Input.Email; // Mantener sincronizado
        user.PhoneNumber = Input.PhoneNumber;
        user.Address = Input.Address;
        user.Role = Input.Role;
        user.EmailConfirmed = Input.EmailConfirmed;
        user.PhoneNumberConfirmed = Input.PhoneNumberConfirmed;
        user.LastUpdated = DateTime.Now;

        // Procesar imagen de perfil si se subió una nueva
        if (Input.ProfilePicture != null)
        {
            var uploadResult = await UploadProfilePictureAsync(Input.ProfilePicture);
            if (uploadResult.Success)
            {
                // Eliminar foto anterior si existe
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    DeleteOldProfilePicture(user.ProfilePicturePath);
                }
                user.ProfilePicturePath = uploadResult.Path;
            }
            else
            {
                ModelState.AddModelError("Input.ProfilePicture", uploadResult.ErrorMessage);
                LoadRoleList();
                return Page();
            }
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Usuario {user.FullName} actualizado exitosamente.";
            return RedirectToPage("./Details", new { id = user.Id });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        LoadRoleList();
        return Page();
    }

    private void LoadRoleList()
    {
        RoleList = new SelectList(new[]
        {
            new { Value = "Admin", Text = "Administrador" },
            new { Value = "Employee", Text = "Empleado" },
            new { Value = "Client", Text = "Cliente" }
        }, "Value", "Text", Input?.Role);
    }

    private async Task<(bool Success, string Path, string ErrorMessage)> UploadProfilePictureAsync(IFormFile file)
    {
        // Validar tamaño (máximo 2MB)
        if (file.Length > 2 * 1024 * 1024)
        {
            return (false, null, "La imagen no debe superar los 2MB");
        }

        // Validar extensión
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return (false, null, "Solo se permiten archivos JPG y PNG");
        }

        try
        {
            // Generar nombre único
            var fileName = $"user_{Input.Id}_{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");

            // Crear carpeta si no existe
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Retornar ruta relativa
            var relativePath = $"/uploads/profiles/{fileName}";
            return (true, relativePath, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al subir la imagen: {ex.Message}");
        }
    }

    private void DeleteOldProfilePicture(string path)
    {
        try
        {
            if (!string.IsNullOrEmpty(path) && path.StartsWith("/uploads/"))
            {
                var fullPath = Path.Combine(_environment.WebRootPath, path.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
        catch (Exception)
        {
            // Log error but don't fail the update
        }
    }
}