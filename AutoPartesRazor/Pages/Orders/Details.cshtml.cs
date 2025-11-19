<<<<<<< HEAD
﻿using AutoPartesRazor.Data;
=======
﻿using System.Threading.Tasks;
using AutoPartesRazor.Data;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Orders;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public DetailsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

<<<<<<< HEAD
        Order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id.Value);
=======
        Order = await _context.Order
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.id == id.Value);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (Order == null) return NotFound();

        return Page();
    }
}
