using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;

namespace AutoPartesRazor.Pages.Account
{
    [Authorize]
    public class ManageModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ManageModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public UpdateViewModel UpdateUser { get; set; }

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
    }
}
