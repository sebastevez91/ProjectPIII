<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.RazorPages;
=======
﻿using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AutoPartesRazor.Data.AutoPartesRazorContext _context;

    public IndexModel(ILogger<IndexModel> logger, Data.AutoPartesRazorContext context)
    {
        _logger = logger;
        _context = context;
    }

    public int CartCount { get; set; } = 0;

    public async Task OnGetAsync()
    {
        // Contar el número de items únicos en el carrito
<<<<<<< HEAD
        var count = await _context.Carts.CountAsync();
=======
        var count = await _context.Cart.CountAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        CartCount = count;
    }
}
