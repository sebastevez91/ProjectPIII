using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Stock;

public class MovementsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public MovementsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public IList<StockMovement> Movements { get; set; } = new List<StockMovement>();
    public StockSummary Summary { get; set; } = new StockSummary();

    [BindProperty(SupportsGet = true)]
    public MovementFilters Filters { get; set; } = new MovementFilters();

    public async Task OnGetAsync()
    {
        var query = _context.StockMovements
            .Include(m => m.Product)
            .Include(m => m.PurchaseOrder)
                .ThenInclude(po => po.Supplier)
            .AsQueryable();

        // Aplicar filtros
        if (Filters.ProductId.HasValue)
        {
            query = query.Where(m => m.ProductId == Filters.ProductId.Value);
        }

        if (Filters.MovementType.HasValue)
        {
            query = query.Where(m => m.MovementType == Filters.MovementType.Value);
        }

        if (Filters.DateFrom.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= Filters.DateFrom.Value);
        }

        if (Filters.DateTo.HasValue)
        {
            var dateTo = Filters.DateTo.Value.AddDays(1);
            query = query.Where(m => m.CreatedAt < dateTo);
        }

        Movements = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(100)
            .ToListAsync();

        // Calcular resumen
        var allMovements = await query.ToListAsync();

        Summary = new StockSummary
        {
            TotalEntries = allMovements.Where(m =>
                m.MovementType == StockMovementType.PurchaseEntry ||
                m.MovementType == StockMovementType.AdjustmentIncrease ||
                m.MovementType == StockMovementType.Return)
                .Sum(m => m.Quantity),

            TotalExits = allMovements.Where(m =>
                m.MovementType == StockMovementType.SaleExit ||
                m.MovementType == StockMovementType.AdjustmentDecrease)
                .Sum(m => m.Quantity),

            TotalAdjustments = allMovements.Where(m =>
                m.MovementType == StockMovementType.AdjustmentIncrease ||
                m.MovementType == StockMovementType.AdjustmentDecrease)
                .Sum(m => m.Quantity),

            MovementCount = Movements.Count
        };
    }

    public async Task<JsonResult> OnGetProductsAsync()
    {
        var products = await _context.Products
            .Where(p => !p.IsDeleted)
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();

        return new JsonResult(products);
    }
}

public class MovementFilters
{
    public int? ProductId { get; set; }
    public StockMovementType? MovementType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class StockSummary
{
    public int TotalEntries { get; set; }
    public int TotalExits { get; set; }
    public int TotalAdjustments { get; set; }
    public int MovementCount { get; set; }
    public int NetMovement => TotalEntries - TotalExits;
}