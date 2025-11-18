using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Account;

[Authorize]
public class ManageModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IWebHostEnvironment _env;

    public ManageModel(AutoPartesRazor.Data.AutoPartesRazorContext context,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _env = env;
    }

    [BindProperty]
    public UpdateViewModel UpdateUser { get; set; }

    // Para imagen del producto
    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    private async Task LoadAsync(User user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        UpdateUser = new UpdateViewModel
        {
            Username = userName,
            Email = email,
            FullName = user.FullName,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            ProfilePicturePath = user.ProfilePicturePath,
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (UpdateUser.Email != email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, UpdateUser.Email);
            if (!setEmailResult.Succeeded)
            {
                StatusMessage = "Error al actualizar el email.";
                return RedirectToPage();
            }
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (UpdateUser.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, UpdateUser.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Error al actualizar el número de teléfono.";
                return RedirectToPage();
            }
        }

        // Actualizar campos adicionales del usuario
        user.FullName = UpdateUser.FullName;
        user.PhoneNumber = UpdateUser.PhoneNumber;
        user.Address = UpdateUser.Address;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            StatusMessage = "Error inesperado al actualizar el perfil.";
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Tu perfil ha sido actualizado exitosamente.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync(ChangePasswordModel changePassword)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            await LoadAsync(user);
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Tu contraseña ha sido cambiada exitosamente.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdatePhotoProfileAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);

        if (ImageFile != null)
        {
            // Crear carpeta si no existe y usar ruta absoluta del wwwroot
            var uploadsDir = Path.Combine(_env.WebRootPath, "img", "perfil");
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(ImageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var uploadPath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            UpdateUser.ProfilePicturePath = "/img/perfil/" + fileName;

            // Actualizar la ruta de la imagen en la entidad User
            user.ProfilePicturePath = UpdateUser.ProfilePicturePath;
        }
        else
        {
            // Mantener la imagen existente
            _context.Entry(user).Property(p => p.ProfilePicturePath).IsModified = false;
        }
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
