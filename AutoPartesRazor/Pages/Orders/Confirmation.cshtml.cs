using AutoPartesRazor.Data;
using AutoPartesRazor.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Orders;

[Authorize]
public class ConfirmationModel : PageModel
{
    private readonly AutoPartesRazorContext _context;
    private readonly IPdfService _pdfService;

    public ConfirmationModel(AutoPartesRazorContext context, IPdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [BindProperty(SupportsGet = true)]
    public int OrderId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        OrderId = id;
        // opcional: validar existencia
        var exists = await _context.Order.FindAsync(id);
        if (exists == null) return NotFound();
        return Page();
    }

    // Handler para descargar el comprobante en PDF: /Orders/Confirmation?id=123&handler=Download
    public async Task<IActionResult> OnGetDownloadAsync(int id)
    {
        var order = await _context.Order
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.id == id);

        if (order == null) return NotFound();

        var pdf = _pdfService.GenerateOrderPdf(order);
        return File(pdf, "application/pdf", $"pedido_{order.id}.pdf");
    }
}
