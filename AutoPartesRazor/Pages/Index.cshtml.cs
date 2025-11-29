using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;

    public IndexModel(ILogger<IndexModel> logger, Data.AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public int CartCount { get; set; } = 0;
    public string NameUserSesion { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            NameUserSesion = currentUser.FullName;
        }
        // Contar el número de items únicos en el carrito
        var count = await _context.Carts.CountAsync();
        CartCount = count;
    }
}
