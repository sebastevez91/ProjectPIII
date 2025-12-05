using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace AutoPartesRazor.Interfaces;

public interface IUserService
{
    // Métodos existentes
    Task<IdentityResult> AddUserAsync(User user, string password);
    Task AddUserToRoleAsync(User user, string roleName);
    Task CheckRoleAsync(string roleName);
    Task<User> GetUserAsync(string email);
    Task<bool> IsUserInRoleAsync(User user, string roleName);
    Task<SignInResult> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
    Task<IdentityResult> ToggleLockoutAsync(string userId);
    Task<bool> IsLockedOutAsync(string userId);

    // Nuevos métodos para gestión de roles
    Task<List<IdentityRole>> GetAllRolesAsync();
    Task<IdentityRole> GetRoleByIdAsync(string roleId);
    Task<IdentityRole> GetRoleByNameAsync(string roleName);
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName);
    Task<IdentityResult> DeleteRoleAsync(string roleId);

    // Métodos para gestión de permisos (Claims)
    Task<IList<Claim>> GetRoleClaimsAsync(string roleId);
    Task<IdentityResult> AddClaimToRoleAsync(string roleId, string claimType, string claimValue);
    Task<IdentityResult> RemoveClaimFromRoleAsync(string roleId, string claimType, string claimValue);
    Task<bool> RoleHasClaimAsync(string roleId, string claimType, string claimValue);

    // Métodos para usuarios
    Task<User> GetUserByIdAsync(string userId);
    Task<IList<string>> GetUserRolesAsync(User user);
    Task<IdentityResult> RemoveUserFromRoleAsync(User user, string roleName);
}