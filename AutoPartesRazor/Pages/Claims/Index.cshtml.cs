using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoPartesRazor.Pages.Reclamos;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IClaimService _reclamoService;
    private readonly UserManager<User> _userManager;

    public IndexModel(IClaimService reclamoService, UserManager<User> userManager)
    {
        _reclamoService = reclamoService;
        _userManager = userManager;
    }

    public List<Claim> Reclamos { get; set; } = new List<Claim>();

    [TempData]
    public string? MensajeExito { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login");
        }

        Reclamos = await _reclamoService.ObtenerReclamosPorClienteAsync(usuario.Id);

        return Page();
    }
}
