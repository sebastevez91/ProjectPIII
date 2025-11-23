using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Suppliers;

public class ClaimsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public ClaimsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<SupplierClaim> Claims { get; set; } = new List<SupplierClaim>();

    [BindProperty]
    public ClaimUpdateInput UpdateInput { get; set; } = new ClaimUpdateInput();

    public async Task OnGetAsync(string status = "all")
    {
        var query = _context.SupplierClaims
            .Include(c => c.Supplier)
            .Include(c => c.PurchaseOrder)
                .ThenInclude(po => po.Product)
            .Include(c => c.StockAdjustment)
                .ThenInclude(sa => sa.Product)
            .AsQueryable();

        if (status != "all")
        {
            if (Enum.TryParse<ClaimStatus>(status, out var claimStatus))
            {
                query = query.Where(c => c.Status == claimStatus);
            }
        }

        Claims = await query
            .OrderByDescending(c => c.ClaimDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync()
    {
        var claim = await _context.SupplierClaims.FindAsync(UpdateInput.ClaimId);

        if (claim == null)
        {
            TempData["ErrorMessage"] = "Reclamo no encontrado.";
            return RedirectToPage();
        }

        claim.Status = UpdateInput.NewStatus;

        if (UpdateInput.NewStatus == ClaimStatus.ResolvedAccepted ||
            UpdateInput.NewStatus == ClaimStatus.ResolvedRejected ||
            UpdateInput.NewStatus == ClaimStatus.Cancelled)
        {
            claim.ResolutionDate = DateTime.Now;
            claim.Resolution = UpdateInput.Resolution;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Estado del reclamo actualizado a: {GetStatusText(UpdateInput.NewStatus)}";
        return RedirectToPage();
    }

    private string GetStatusText(ClaimStatus status)
    {
        return status switch
        {
            ClaimStatus.Pending => "Pendiente",
            ClaimStatus.InProgress => "En Proceso",
            ClaimStatus.ResolvedAccepted => "Resuelto - Aceptado",
            ClaimStatus.ResolvedRejected => "Resuelto - Rechazado",
            ClaimStatus.Cancelled => "Cancelado",
            _ => status.ToString()
        };
    }
}

public class ClaimUpdateInput
{
    public int ClaimId { get; set; }
    public ClaimStatus NewStatus { get; set; }
    public string? Resolution { get; set; }
}