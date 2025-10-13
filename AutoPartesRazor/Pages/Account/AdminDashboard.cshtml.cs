using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Account;

[Authorize(Roles = "Admin")]
public class AdminDashboardModel : PageModel
{
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public AdminDashboardModel(AutoPartesRazor.Data.AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<User> Users { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if(_context.Users == null)
        {
            Users = await _context.Users.ToListAsync();
        }
    }
}
