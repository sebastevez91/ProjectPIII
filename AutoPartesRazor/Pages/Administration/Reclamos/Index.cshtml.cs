using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoPartesRazor.Models.Enum;

namespace AutoPartesRazor.Pages.Admin.Reclamos;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IClaimService _reclamoService;

    public IndexModel(IClaimService reclamoService)
    {
        _reclamoService = reclamoService;
    }

    public List<Claim> Reclamos { get; set; } = new List<Claim>();
    public ReclamoEstadisticas? Estadisticas { get; set; }

    // Filtros
    public StatusClaim? EstadoFiltro { get; set; }
    public LevelUrgency? UrgenciaFiltro { get; set; }
    public string? TextoBusqueda { get; set; }

    public async Task<IActionResult> OnGetAsync(int? estado, int? urgencia, string? buscar)
    {
        // Obtener estadísticas
        Estadisticas = await _reclamoService.ObtenerEstadisticasAsync();

        // Obtener todos los reclamos
        Reclamos = await _reclamoService.ObtenerTodosLosReclamosAsync();

        // Aplicar filtros
        if (estado.HasValue)
        {
            EstadoFiltro = (StatusClaim)estado.Value;
            Reclamos = Reclamos.Where(r => r.Estado == EstadoFiltro).ToList();
        }

        if (urgencia.HasValue)
        {
            UrgenciaFiltro = (LevelUrgency)urgencia.Value;
            Reclamos = Reclamos.Where(r => r.NivelUrgencia == UrgenciaFiltro).ToList();
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            TextoBusqueda = buscar;
            Reclamos = Reclamos.Where(r => 
                r.NumeroTicket.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                r.Asunto.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                (r.Cliente != null && r.Cliente.FullName.Contains(buscar, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        return Page();
    }
}
