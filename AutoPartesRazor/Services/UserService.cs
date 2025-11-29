using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace AutoPartesRazor.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    #region Métodos de Usuario

    public async Task<IdentityResult> AddUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task AddUserToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<User> GetUserAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IList<string>> GetUserRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(User user, string roleName)
    {
        return await _userManager.RemoveFromRoleAsync(user, roleName);
    }
    #endregion

    #region Métodos de Autenticación

    public async Task<SignInResult> LoginAsync(LoginViewModel model)
    {
        return await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    #endregion

    #region Métodos de Bloqueo

    public async Task<IdentityResult> ToggleLockoutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            return await _userManager.SetLockoutEndDateAsync(user, null);
        }
        else
        {
            return await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
    }

    public async Task<bool> IsLockedOutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.IsLockedOutAsync(user);
    }

    #endregion

    #region Métodos de Roles

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

    public async Task<List<IdentityRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
    {
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        var role = new IdentityRole(roleName);
        return await _roleManager.CreateAsync(role);
    }

    public async Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Rol no encontrado" });
        }

        role.Name = newRoleName;
        return await _roleManager.UpdateAsync(role);
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Rol no encontrado" });
        }

        return await _roleManager.DeleteAsync(role);
    }

    #endregion

    #region Métodos de Permisos (Claims)

    public async Task<IList<Claim>> GetRoleClaimsAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new List<Claim>();
        }

        return await _roleManager.GetClaimsAsync(role);
    }

    public async Task<IdentityResult> AddClaimToRoleAsync(string roleId, string claimType, string claimValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Rol no encontrado" });
        }

        var claim = new Claim(claimType, claimValue);
        return await _roleManager.AddClaimAsync(role, claim);
    }

    public async Task<IdentityResult> RemoveClaimFromRoleAsync(string roleId, string claimType, string claimValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Rol no encontrado" });
        }

        var claim = new Claim(claimType, claimValue);
        return await _roleManager.RemoveClaimAsync(role, claim);
    }

    public async Task<bool> RoleHasClaimAsync(string roleId, string claimType, string claimValue)
    {
        var claims = await GetRoleClaimsAsync(roleId);
        return claims.Any(c => c.Type == claimType && c.Value == claimValue);
    }

    #endregion
}