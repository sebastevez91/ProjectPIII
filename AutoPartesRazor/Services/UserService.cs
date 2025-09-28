using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AutoPartesRazor.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    public UserService(UserManager<User> userManager,

    RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    //Damos de alta al Usuario
    public async Task<IdentityResult> AddUserAsync(User user, string
    password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    //Agregamos un Rol al Usuario
    public async Task AddUserToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task CheckRoleAsync(string roleName)
    {
        bool existsRole = await _roleManager.RoleExistsAsync(roleName);
        if (!existsRole)
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = roleName
            });
        }

    }

    public async Task<User> GetUserAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<SignInResult> LoginAsync(LoginViewModel model)
    {
        return await _signInManager.PasswordSignInAsync(
        model.Email,
        model.Password,
        model.RememberMe,
        false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

}
