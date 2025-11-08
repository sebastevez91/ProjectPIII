using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Models;

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
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre de usuario es requerido")]
            [Display(Name = "Nombre de usuario")]
            public string Username { get; set; }

            [Required(ErrorMessage = "El email es requerido")]
            [EmailAddress(ErrorMessage = "El email no es válido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Phone(ErrorMessage = "El número de teléfono no es válido")]
            [Display(Name = "Teléfono")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Nombre completo")]
            public string FullName { get; set; }

            [Display(Name = "Dirección")]
            public string Address { get; set; }

            [Display(Name = "Ciudad")]
            public string City { get; set; }

            [Display(Name = "Código Postal")]
            public string PostalCode { get; set; }

            [Display(Name = "País")]
            public string Country { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                Username = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                // Ajusta estos campos según tu modelo User
                FullName = user.FullName,
                Address = user.Address,
                City = user.City,
                PostalCode = user.PostalCode,
                Country = user.Country
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
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    StatusMessage = "Error al actualizar el email.";
                    return RedirectToPage();
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Error al actualizar el número de teléfono.";
                    return RedirectToPage();
                }
            }

            // Actualizar campos adicionales del usuario
            user.FullName = Input.FullName;
            user.Address = Input.Address;
            user.City = Input.City;
            user.PostalCode = Input.PostalCode;
            user.Country = Input.Country;

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
